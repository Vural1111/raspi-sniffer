using PacketDotNet;
using SharpPcap;
using Sniffer.Core;

Console.WriteLine("Raspi Sniffer - Konsol Arayüzü\n");

using var captureService = new CaptureService();
var devices = captureService.GetAvailableDevices();

if (devices.Count == 0)
{
    Console.WriteLine("Uygun ağ arayüzü bulunamadı. Lütfen SharpPcap'ın desteklediği bir ortamda çalıştırın.");
    return;
}

for (var i = 0; i < devices.Count; i++)
{
    Console.WriteLine($"[{i}] {devices[i].Description} ({devices[i].Name})");
}

Console.Write("\nDinlenecek cihaz numarasını girin: ");
var selection = Console.ReadLine();

if (!int.TryParse(selection, out var index) || index < 0 || index >= devices.Count)
{
    Console.WriteLine("Geçersiz seçim.");
    return;
}

var selectedDevice = devices[index];
Console.WriteLine($"\n{selectedDevice.Description} cihazı üzerinde paket yakalanıyor... Çıkmak için ENTER'a basın.\n");

var consoleLock = new object();

captureService.PacketReceived += (_, args) =>
{
    lock (consoleLock)
    {
        var ipPacket = args.Packet?.Extract<PacketDotNet.IpPacket>();
        var tcpPacket = args.Packet?.Extract<TcpPacket>();
        var udpPacket = args.Packet?.Extract<UdpPacket>();

        var source = ipPacket?.SourceAddress?.ToString() ?? "Unknown";
        var destination = ipPacket?.DestinationAddress?.ToString() ?? "Unknown";
        string protocol = ipPacket?.Protocol.ToString() ?? args.Packet?.GetType().Name ?? "N/A";
        string extra = tcpPacket is not null
            ? $"TCP {tcpPacket.SourcePort}->{tcpPacket.DestinationPort}"
            : udpPacket is not null
                ? $"UDP {udpPacket.SourcePort}->{udpPacket.DestinationPort}"
                : string.Empty;

        Console.WriteLine(
            $"[{args.Timestamp:HH:mm:ss}] {protocol,-8} {source} -> {destination} {extra}".Trim());
    }
};

try
{
    captureService.StartCapture(selectedDevice);
    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine($"Yakalama başlatılamadı: {ex.Message}");
}
finally
{
    captureService.StopCapture();
    Console.WriteLine("\nYakalama durduruldu.");
}
