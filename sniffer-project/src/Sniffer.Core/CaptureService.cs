using PacketDotNet;
using SharpPcap;

namespace Sniffer.Core;

public class PacketCapturedEventArgs : EventArgs
{
    public PacketCapturedEventArgs(ICaptureDevice device, RawCapture rawCapture, Packet? packet)
    {
        Device = device;
        RawCapture = rawCapture;
        Packet = packet;
        Timestamp = rawCapture.Timeval.Date;
    }

    public ICaptureDevice Device { get; }

    public RawCapture RawCapture { get; }

    public Packet? Packet { get; }

    public DateTime Timestamp { get; }
}

public class CaptureService : IDisposable
{
    private readonly object _sync = new();
    private ICaptureDevice? _device;
    private bool _disposed;

    public event EventHandler<PacketCapturedEventArgs>? PacketReceived;

    public bool IsCapturing => _device is not null;

    public IReadOnlyList<ICaptureDevice> GetAvailableDevices()
    {
        return CaptureDeviceList.Instance.ToList();
    }

    public void StartCapture(ICaptureDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);

        lock (_sync)
        {
            EnsureNotDisposed();

            if (_device is not null)
            {
                throw new InvalidOperationException("Capture already in progress.");
            }

            _device = device;
            _device.OnPacketArrival += HandlePacketArrival;
            _device.Open(DeviceModes.Promiscuous, read_timeout: 1000);
            _device.StartCapture();
        }
    }

    public void StartCapture(string deviceName)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceName);

        var device = GetAvailableDevices().FirstOrDefault(d => d.Name == deviceName || d.Description == deviceName);
        if (device is null)
        {
            throw new ArgumentException($"Device '{deviceName}' not found.", nameof(deviceName));
        }

        StartCapture(device);
    }

    public void StopCapture()
    {
        lock (_sync)
        {
            if (_device is null)
            {
                return;
            }

            _device.OnPacketArrival -= HandlePacketArrival;
            _device.StopCapture();
            _device.Close();
            _device = null;
        }
    }

    private void HandlePacketArrival(object? sender, PacketCapture e)
    {
        if (_device is null)
        {
            return;
        }

        var rawCapture = e.GetPacket();
        var packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
        var args = new PacketCapturedEventArgs(_device, rawCapture, packet);
        PacketReceived?.Invoke(this, args);
    }

    private void EnsureNotDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(CaptureService));
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        StopCapture();
        _disposed = true;
    }
}
