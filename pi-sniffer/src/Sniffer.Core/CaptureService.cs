using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PacketDotNet;
using SharpPcap;

namespace Sniffer.Core;

// Requires NuGet packages: SharpPcap, PacketDotNet

public class CaptureService
{
    private readonly object _deviceLock = new();
    private ICaptureDevice? _device;
    private readonly ConcurrentDictionary<string, DeviceInfo> _connectedDevices = new();

    public event Action<PacketInfo>? PacketReceived;
    public event Action<List<DeviceInfo>>? DevicesUpdated;

    public void StartCapture(string deviceName)
    {
        try
        {
            lock (_deviceLock)
            {
                if (_device is not null)
                {
                    Console.Error.WriteLine("Capture already running.");
                    return;
                }

                var device = CaptureDeviceList.Instance
                    .FirstOrDefault(d => d.Name == deviceName || d.Description == deviceName);

                if (device is null)
                {
                    throw new InvalidOperationException($"Device '{deviceName}' not found.");
                }

                device.OnPacketArrival += DeviceOnPacketArrival;
                device.Open(DeviceModes.Promiscuous, readTimeoutMilliseconds: 1000);
                device.StartCapture();
                _device = device;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[CaptureService] StartCapture failed: {ex.Message}");
            StopCapture();
            throw;
        }
    }

    public void StopCapture()
    {
        lock (_deviceLock)
        {
            if (_device is null)
            {
                return;
            }

            try
            {
                _device.OnPacketArrival -= DeviceOnPacketArrival;
                if (_device.Started)
                {
                    _device.StopCapture();
                }
                _device.Close();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[CaptureService] StopCapture warning: {ex.Message}");
            }
            finally
            {
                _device = null;
            }
        }
    }

    public List<DeviceInfo> GetConnectedDevices()
    {
        return _connectedDevices.Values
            .OrderByDescending(d => d.LastSeen)
            .ToList();
    }

    private void DeviceOnPacketArrival(object sender, CaptureEventArgs e)
    {
        try
        {
            var rawPacket = e.Packet;
            var time = rawPacket.Timeval.Date;
            var length = rawPacket.Data?.Length ?? 0;

            var packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
            var ethernetPacket = packet.Extract<EthernetPacket>();
            if (ethernetPacket is null)
            {
                return;
            }

            var srcMac = ethernetPacket.SourceHardwareAddress?.ToString() ?? string.Empty;
            var dstMac = ethernetPacket.DestinationHardwareAddress?.ToString() ?? string.Empty;

            var ipPacket = packet.Extract<IPPacket>();
            var srcIp = ipPacket?.SourceAddress?.ToString() ?? string.Empty;
            var dstIp = ipPacket?.DestinationAddress?.ToString() ?? string.Empty;
            var protocol = ipPacket?.Protocol.ToString() ?? ethernetPacket.Type.ToString();

            var packetInfo = new PacketInfo
            {
                Timestamp = time,
                SrcIp = srcIp,
                SrcMac = srcMac,
                DstIp = dstIp,
                DstMac = dstMac,
                Protocol = protocol,
                Length = length,
                DisplayName = $"{srcIp} -> {dstIp} ({protocol})"
            };

            PacketReceived?.Invoke(packetInfo);

            UpdateConnectedDevices(packetInfo);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[CaptureService] Packet processing error: {ex.Message}");
        }
    }

    private void UpdateConnectedDevices(PacketInfo packet)
    {
        void UpdateDevice(string mac, string ip)
        {
            if (string.IsNullOrWhiteSpace(mac))
            {
                return;
            }

            _connectedDevices.AddOrUpdate(
                mac,
                _ => new DeviceInfo
                {
                    MacAddress = mac,
                    IpAddress = ip,
                    LastSeen = packet.Timestamp,
                    DisplayName = string.IsNullOrWhiteSpace(ip) ? mac : $"{ip} ({mac})"
                },
                (_, existing) =>
                {
                    existing.LastSeen = packet.Timestamp;
                    if (!string.IsNullOrWhiteSpace(ip))
                    {
                        existing.IpAddress = ip;
                        existing.DisplayName = $"{ip} ({mac})";
                    }

                    return existing;
                });
        }

        UpdateDevice(packet.SrcMac, packet.SrcIp);
        UpdateDevice(packet.DstMac, packet.DstIp);

        DevicesUpdated?.Invoke(GetConnectedDevices());
    }
}

public class PacketInfo
{
    public DateTime Timestamp { get; set; }
    public string SrcIp { get; set; } = string.Empty;
    public string SrcMac { get; set; } = string.Empty;
    public string DstIp { get; set; } = string.Empty;
    public string DstMac { get; set; } = string.Empty;
    public string Protocol { get; set; } = string.Empty;
    public int Length { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}

public class DeviceInfo
{
    public string MacAddress { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LastSeen { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}
