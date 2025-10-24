using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Sniffer.Core;
using SharpPcap;

namespace Sniffer.UI;

// Requires NuGet packages: SharpPcap, PacketDotNet

public partial class FormMain : Form
{
    private readonly CaptureService _captureService = new();
    private readonly BindingList<DeviceDisplay> _deviceBinding = new();
    private readonly BindingList<PacketDisplay> _packetBinding = new();
    private readonly List<PacketInfo> _packetHistory = new();
    private readonly object _packetLock = new();
    private string? _selectedDeviceKey;

    public FormMain()
    {
        InitializeComponent();
        dgvDevices.AutoGenerateColumns = false;
        dgvPackets.AutoGenerateColumns = false;
        dgvDevices.DataSource = _deviceBinding;
        dgvPackets.DataSource = _packetBinding;
        ConfigureDeviceColumns();
        ConfigurePacketColumns();

        _captureService.PacketReceived += CaptureServiceOnPacketReceived;
        _captureService.DevicesUpdated += CaptureServiceOnDevicesUpdated;
    }

    private void ConfigureDeviceColumns()
    {
        dgvDevices.Columns.Clear();
        dgvDevices.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(DeviceDisplay.DisplayName),
            HeaderText = "Cihaz",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });
        dgvDevices.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(DeviceDisplay.MacAddress),
            HeaderText = "MAC",
            Width = 140
        });
        dgvDevices.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(DeviceDisplay.IpAddress),
            HeaderText = "IP",
            Width = 120
        });
        dgvDevices.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(DeviceDisplay.LastSeen),
            HeaderText = "Son Görülme",
            Width = 140,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm:ss" }
        });
    }

    private void ConfigurePacketColumns()
    {
        dgvPackets.Columns.Clear();
        dgvPackets.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PacketDisplay.Timestamp),
            HeaderText = "Zaman",
            Width = 110,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm:ss" }
        });
        dgvPackets.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PacketDisplay.Source),
            HeaderText = "Kaynak",
            Width = 160
        });
        dgvPackets.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PacketDisplay.Destination),
            HeaderText = "Hedef",
            Width = 160
        });
        dgvPackets.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PacketDisplay.Protocol),
            HeaderText = "Protokol",
            Width = 80
        });
        dgvPackets.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PacketDisplay.Length),
            HeaderText = "Uzunluk",
            Width = 80
        });
    }

    private void FormMain_Load(object sender, EventArgs e)
    {
        LoadInterfaces();
        UpdateControls(false);
    }

    private void LoadInterfaces()
    {
        try
        {
            var devices = CaptureDeviceList.Instance
                .Select(d => new InterfaceItem(d.Name, d.Description))
                .ToList();

            cmbInterfaces.DataSource = devices;
            cmbInterfaces.DisplayMember = nameof(InterfaceItem.Display);
            cmbInterfaces.ValueMember = nameof(InterfaceItem.Name);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Arayüzler yüklenemedi: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CaptureServiceOnDevicesUpdated(List<DeviceInfo> devices)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => CaptureServiceOnDevicesUpdated(devices)));
            return;
        }

        _deviceBinding.Clear();
        foreach (var device in devices)
        {
            _deviceBinding.Add(new DeviceDisplay(device));
        }
    }

    private void CaptureServiceOnPacketReceived(PacketInfo packet)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => CaptureServiceOnPacketReceived(packet)));
            return;
        }

        lock (_packetLock)
        {
            _packetHistory.Add(packet);
            if (_packetHistory.Count > 500)
            {
                _packetHistory.RemoveAt(0);
            }
        }

        RefreshPacketView();
    }

    private void RefreshPacketView()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        var filter = txtFilter.Text.Trim();
        var selectedKey = _selectedDeviceKey;
        var filtered = new List<PacketDisplay>();

        lock (_packetLock)
        {
            foreach (var packet in _packetHistory.AsEnumerable().Reverse())
            {
                if (!MatchesDevice(packet, selectedKey))
                {
                    continue;
                }

                if (!MatchesFilter(packet, filter))
                {
                    continue;
                }

                filtered.Add(new PacketDisplay(packet));
                if (filtered.Count >= 300)
                {
                    break;
                }
            }
        }

        _packetBinding.RaiseListChangedEvents = false;
        _packetBinding.Clear();
        foreach (var item in filtered)
        {
            _packetBinding.Add(item);
        }
        _packetBinding.RaiseListChangedEvents = true;
        _packetBinding.ResetBindings();
    }

    private static bool MatchesFilter(PacketInfo packet, string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return true;
        }

        return packet.SrcIp.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
               packet.DstIp.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
               packet.SrcMac.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
               packet.DstMac.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchesDevice(PacketInfo packet, string? selectedKey)
    {
        if (string.IsNullOrWhiteSpace(selectedKey))
        {
            return true;
        }

        bool Matches(string? value) => !string.IsNullOrWhiteSpace(value) &&
                                       value.Equals(selectedKey, StringComparison.OrdinalIgnoreCase);

        return Matches(packet.SrcMac) || Matches(packet.DstMac) || Matches(packet.SrcIp) || Matches(packet.DstIp);
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        if (cmbInterfaces.SelectedValue is not string deviceName)
        {
            MessageBox.Show(this, "Lütfen bir arayüz seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _captureService.StartCapture(deviceName);
            _packetHistory.Clear();
            _packetBinding.Clear();
            UpdateControls(true);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Yakalama başlatılamadı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnStop_Click(object sender, EventArgs e)
    {
        _captureService.StopCapture();
        UpdateControls(false);
    }

    private void dgvDevices_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvDevices.CurrentRow?.DataBoundItem is DeviceDisplay device)
        {
            _selectedDeviceKey = !string.IsNullOrWhiteSpace(device.IpAddress)
                ? device.IpAddress
                : device.MacAddress;
        }
        else
        {
            _selectedDeviceKey = null;
        }

        RefreshPacketView();
    }

    private void dgvPackets_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvPackets.CurrentRow?.DataBoundItem is PacketDisplay packet)
        {
            txtPacketDetails.Text = packet.Raw.ToDetailedString();
        }
        else
        {
            txtPacketDetails.Clear();
        }
    }

    private void txtFilter_TextChanged(object sender, EventArgs e)
    {
        RefreshPacketView();
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        _captureService.StopCapture();
    }

    private void UpdateControls(bool capturing)
    {
        btnStart.Enabled = !capturing;
        btnStop.Enabled = capturing;
        cmbInterfaces.Enabled = !capturing;
    }

    private sealed record InterfaceItem(string Name, string Description)
    {
        public string Display => string.IsNullOrWhiteSpace(Description) ? Name : $"{Name} - {Description}";
    }

    private sealed class DeviceDisplay
    {
        public DeviceDisplay(DeviceInfo info)
        {
            DisplayName = info.DisplayName;
            MacAddress = info.MacAddress;
            IpAddress = info.IpAddress;
            LastSeen = info.LastSeen;
        }

        public string DisplayName { get; }
        public string MacAddress { get; }
        public string IpAddress { get; }
        public DateTime LastSeen { get; }
    }

    private sealed class PacketDisplay
    {
        public PacketDisplay(PacketInfo info)
        {
            Raw = info;
            Timestamp = info.Timestamp;
            Source = string.IsNullOrWhiteSpace(info.SrcIp)
                ? info.SrcMac
                : $"{info.SrcIp} ({info.SrcMac})";
            Destination = string.IsNullOrWhiteSpace(info.DstIp)
                ? info.DstMac
                : $"{info.DstIp} ({info.DstMac})";
            Protocol = info.Protocol;
            Length = info.Length;
        }

        public PacketInfo Raw { get; }
        public DateTime Timestamp { get; }
        public string Source { get; }
        public string Destination { get; }
        public string Protocol { get; }
        public int Length { get; }
    }
}

public static class PacketInfoExtensions
{
    public static string ToDetailedString(this PacketInfo packet)
    {
        return $"Zaman: {packet.Timestamp:O}\r\n" +
               $"Kaynak IP: {packet.SrcIp}\r\n" +
               $"Kaynak MAC: {packet.SrcMac}\r\n" +
               $"Hedef IP: {packet.DstIp}\r\n" +
               $"Hedef MAC: {packet.DstMac}\r\n" +
               $"Protokol: {packet.Protocol}\r\n" +
               $"Uzunluk: {packet.Length}\r\n";
    }
}
