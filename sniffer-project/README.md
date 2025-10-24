# Raspi Sniffer

Raspi Sniffer, Raspberry Pi üzerinde çalışacak .NET 8 tabanlı, SharpPcap ve PacketDotNet kullanarak ağ paketlerini yakalayan bir araçtır. Uygulama hem WinForms arayüzü hem de terminal tabanlı bir konsol arayüzü sağlar.

## Özellikler
- SharpPcap ile ağ arayüzlerinden paket yakalama
- PacketDotNet ile paket içeriğini ayrıştırma
- WinForms arayüzü ile gerçek zamanlı paket listesi ve detay görüntüleme
- Konsol uygulamasıyla terminal üzerinden kullanım
- Raspberry Pi üzerinde publish edilip çalıştırılabilirlik

## Geliştirme Ortamı
- .NET 8 SDK
- Visual Studio 2022 (WinForms için)
- SharpPcap 6.x
- PacketDotNet 1.x

### Geliştirme Adımları
1. Bu depoyu klonlayın ve `sniffer-project` klasörüne geçin.
2. Gerekli NuGet paketlerini ekleyin:
   ```bash
   dotnet add src/Sniffer.Core/Sniffer.Core.csproj package SharpPcap
   dotnet add src/Sniffer.Core/Sniffer.Core.csproj package PacketDotNet
   ```
3. WinForms projesi için Visual Studio'da `Sniffer.UI` projesini açın ve tasarımı düzenleyin.
4. Konsol uygulamasını `dotnet run --project src/Sniffer.Console` komutuyla test edin.

## Deploy
Raspberry Pi üzerine publish ve deploy adımları için [`docs/deploy_to_pi.md`](docs/deploy_to_pi.md) dosyasını takip edin.
