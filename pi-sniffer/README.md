# Pi Sniffer

Pi Sniffer, Raspberry Pi üzerinde hotspot olarak çalışan ağ arayüzündeki trafik paketlerini gerçek zamanlı izlemek için geliştirilmiş bir C# aracıdır. SharpPcap ve PacketDotNet kullanılarak paket yakalama yapılır ve bağlı cihazlar pasif olarak keşfedilir. Proje, Visual Studio kullanılarak geliştirilebilen WinForms arayüzü ve terminal tabanlı bir konsol uygulaması içerir.

## Proje Yapısı

```
pi-sniffer/
├─ src/
│  ├─ Sniffer.Core/
│  ├─ Sniffer.Console/
│  └─ Sniffer.UI/
└─ docs/
```

## Geliştirme Ortamı

1. .NET 8 SDK'yı kurun.
2. Depoyu klonlayın ve `pi-sniffer` klasörüne geçin.
3. Visual Studio veya tercih ettiğiniz editörde çözümü açın. Proje dosyaları bağımsız olduğundan `src` altındaki `.csproj` dosyalarını açabilirsiniz.
4. NuGet paketleri:
   - [SharpPcap](https://www.nuget.org/packages/SharpPcap)
   - [PacketDotNet](https://www.nuget.org/packages/PacketDotNet)

> Kod dosyalarındaki yorumlar gerekli NuGet paketlerini hatırlatır.

## Konsol Uygulamasını Çalıştırma

```bash
cd src/Sniffer.Console
dotnet run
```

Paket yakalamak için çoğu zaman `sudo` yetkisi gerekir. Linux üzerinde `sudo dotnet run` komutunu tercih edin veya `setcap` ile gerekli yetkileri tanımlayın.

## WinForms Uygulamasını Çalıştırma

```bash
cd src/Sniffer.UI
dotnet run
```

WinForms uygulaması, bağlı cihazları ve canlı paket akışını görsel olarak listeler. Seçilen cihaza göre paketler filtrelenebilir.

## Yayınlama (Publish) ve Raspberry Pi'ye Dağıtım

Self-contained publish için örnek komutlar `docs/deploy_to_pi.md` içinde yer alır. Özetle:

```bash
cd src/Sniffer.Console
dotnet publish -c Release -r linux-arm64 --self-contained true -o ../../publish/linux-arm64
```

Daha sonra `scp` ile Raspberry Pi'ye kopyalayabilir ve `sudo ./Sniffer.Console` komutuyla çalıştırabilirsiniz.

## Kullanım

1. Uygulamayı başlatın ve paket yakalama yapılacak ağ arayüzünü seçin.
2. `Start` butonuna basarak paket yakalamayı başlatın.
3. Konsol uygulamasında benzer şekilde arayüz seçip canlı paketleri izleyin.
4. Bağlı cihaz listesi pasif olarak güncellenir; arayüzde seçim yaparak paketleri filtreleyebilirsiniz.

## Lisans

Bu depo örnek amaçlıdır ve belirli bir lisans içermez. Kullanım öncesinde güvenlik ve gizlilik gereksinimlerinizi değerlendirin.
