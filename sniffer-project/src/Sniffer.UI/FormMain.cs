using System.ComponentModel;
using PacketDotNet;
using SharpPcap;
using Sniffer.Core;

namespace Sniffer.UI;

public partial class FormMain : Form
{
    private readonly CaptureService _captureService = new();
    private readonly BindingList<PacketViewModel> _packets = new();
    private IReadOnlyList<ICaptureDevice> _devices = Array.Empty<ICaptureDevice>();
    private bool _capturing;

    public FormMain()
    {
        InitializeComponent();

        gridPackets.AutoGenerateColumns = false;
        gridPackets.DataSource = _packets;

        _captureService.PacketReceived += HandlePacketReceived;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        LoadDevices();
    }

    private void LoadDevices()
    {
        try
        {
            _devices = _captureService.GetAvailableDevices();
            comboDevices.Items.Clear();

            foreach (var device in _devices)
            {
                comboDevices.Items.Add($"{device.Description} ({device.Name})");
            }

            if (comboDevices.Items.Count > 0)
            {
                comboDevices.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Aygıt listesi yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void HandlePacketReceived(object? sender, PacketCapturedEventArgs e)
    {
        if (IsDisposed)
        {
            return;
        }

        BeginInvoke(() =>
        {
            var ipPacket = e.Packet?.Extract<PacketDotNet.IpPacket>();
            var tcpPacket = e.Packet?.Extract<TcpPacket>();
            var udpPacket = e.Packet?.Extract<UdpPacket>();

            var source = ipPacket?.SourceAddress?.ToString() ?? "Unknown";
            var destination = ipPacket?.DestinationAddress?.ToString() ?? "Unknown";
            var protocol = ipPacket?.Protocol.ToString() ?? e.Packet?.GetType().Name ?? "N/A";
            var info = tcpPacket is not null
                ? $"TCP {tcpPacket.SourcePort}->{tcpPacket.DestinationPort}"
                : udpPacket is not null
                    ? $"UDP {udpPacket.SourcePort}->{udpPacket.DestinationPort}"
                    : string.Empty;

            var model = new PacketViewModel
            {
                Timestamp = e.Timestamp,
                Protocol = protocol,
                Source = source,
                Destination = destination,
                Info = info,
                Hex = BitConverter.ToString(e.RawCapture.Data),
                Length = e.RawCapture.Data.Length
            };

            _packets.Insert(0, model);
            if (_packets.Count > 500)
            {
                _packets.RemoveAt(_packets.Count - 1);
            }
        });
    }

    private void BtnStartStopClick(object? sender, EventArgs e)
    {
        if (_capturing)
        {
            StopCapture();
        }
        else
        {
            StartCapture();
        }
    }

    private void StartCapture()
    {
        if (comboDevices.SelectedIndex < 0 || comboDevices.SelectedIndex >= _devices.Count)
        {
            MessageBox.Show(this, "Lütfen önce bir ağ arayüzü seçin.", "Bilgi", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        try
        {
            var device = _devices[comboDevices.SelectedIndex];
            _packets.Clear();
            _captureService.StartCapture(device);
            _capturing = true;
            btnStartStop.Text = "Durdur";
            comboDevices.Enabled = false;
            btnRefresh.Enabled = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Yakalama başlatılamadı: {ex.Message}", "Hata", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void StopCapture()
    {
        _captureService.StopCapture();
        _capturing = false;
        btnStartStop.Text = "Başlat";
        comboDevices.Enabled = true;
        btnRefresh.Enabled = true;
    }

    private void BtnRefreshClick(object? sender, EventArgs e)
    {
        if (_capturing)
        {
            return;
        }

        LoadDevices();
    }

    private void GridPacketsSelectionChanged(object? sender, EventArgs e)
    {
        if (gridPackets.CurrentRow?.DataBoundItem is PacketViewModel model)
        {
            txtDetails.Text =
                $"Zaman: {model.Timestamp:yyyy-MM-dd HH:mm:ss.fff}\r\n" +
                $"Protokol: {model.Protocol}\r\n" +
                $"Kaynak: {model.Source}\r\n" +
                $"Hedef: {model.Destination}\r\n" +
                $"Bilgi: {model.Info}\r\n" +
                $"Uzunluk: {model.Length} bayt\r\n\r\n" +
                $"Ham Veri:\r\n{model.Hex}";
        }
        else
        {
            txtDetails.Clear();
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _captureService.Dispose();
        base.OnFormClosing(e);
    }

    private sealed class PacketViewModel
    {
        public DateTime Timestamp { get; set; }
        public string Protocol { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Info { get; set; } = string.Empty;
        public string Hex { get; set; } = string.Empty;
        public int Length { get; set; }
    }
}
