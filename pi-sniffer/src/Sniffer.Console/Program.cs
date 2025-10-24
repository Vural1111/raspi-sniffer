using System;
using System.Threading;
using Sniffer.Core;
using SharpPcap;

// Requires NuGet packages: SharpPcap, PacketDotNet

Console.WriteLine("Pi Sniffer Console - Raspberry Pi hotspot monitoring");
Console.WriteLine("Root yetkisi gerekebilir. Linux üzerinde sudo ile çalıştırmanız önerilir.\n");

var devices = CaptureDeviceList.Instance;
if (devices.Count == 0)
{
    Console.WriteLine("Hiçbir ağ arayüzü bulunamadı.");
    return;
}

Console.WriteLine("Mevcut ağ arayüzleri:");
for (var i = 0; i < devices.Count; i++)
{
    var d = devices[i];
    Console.WriteLine($"[{i}] {d.Name} - {d.Description}");
}

Console.Write("Dinlemek istediğiniz arayüzün numarasını girin: ");
var input = Console.ReadLine();
if (!int.TryParse(input, out var index) || index < 0 || index >= devices.Count)
{
    Console.WriteLine("Geçersiz seçim.");
    return;
}

Console.Write("Opsiyonel filtre (IP veya MAC) girin (boş bırakabilirsiniz): ");
var filter = (Console.ReadLine() ?? string.Empty).Trim();

var captureService = new CaptureService();
var quitEvent = new ManualResetEventSlim(false);

captureService.PacketReceived += packet =>
{
    try
    {
        if (!string.IsNullOrWhiteSpace(filter))
        {
            if (!packet.SrcIp.Contains(filter, StringComparison.OrdinalIgnoreCase) &&
                !packet.DstIp.Contains(filter, StringComparison.OrdinalIgnoreCase) &&
                !packet.SrcMac.Contains(filter, StringComparison.OrdinalIgnoreCase) &&
                !packet.DstMac.Contains(filter, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }

        Console.WriteLine($"[{packet.Timestamp:HH:mm:ss}] {packet.SrcIp} -> {packet.DstIp} {packet.Protocol} {packet.Length} bytes");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"[Console] Paket yazdırma hatası: {ex.Message}");
    }
};

captureService.DevicesUpdated += devices =>
{
    try
    {
        Console.Title = $"Bağlı cihazlar: {devices.Count}";
    }
    catch
    {
        // ignored - konsolda başlık desteklenmeyebilir
    }
};

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    quitEvent.Set();
};

try
{
    var device = devices[index];
    Console.WriteLine($"\n{device.Name} üzerinde dinleme başlatılıyor...");
    captureService.StartCapture(device.Name);
    Console.WriteLine("Paket yakalama başladı. Çıkmak için Ctrl+C.");

    quitEvent.Wait();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Hata: {ex.Message}");
}
finally
{
    captureService.StopCapture();
    Console.WriteLine("Paket yakalama durduruldu.");
}
