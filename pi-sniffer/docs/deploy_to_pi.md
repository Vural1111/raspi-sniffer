# Raspberry Pi'ye Dağıtım

Bu belgede Sniffer.Console uygulamasını .NET 8 ile yayınlama ve Raspberry Pi üzerinde çalıştırma adımları yer alır.

## Gereksinimler

- .NET 8 SDK (geliştirme makinesinde)
- Raspberry Pi üzerinde .NET 8 Runtime veya self-contained dağıtım
- SharpPcap ve PacketDotNet paketleri publish sırasında uygulamaya dahil edilir

## Publish Komutları

Self-contained yayın, hedef cihazda .NET runtime kurulumuna gerek bırakmaz. Mimarinize göre aşağıdaki komutlardan birini kullanın:

```bash
cd src/Sniffer.Console

dotnet publish -c Release -r linux-arm --self-contained true -p:PublishSingleFile=true -o ../../publish/linux-arm

dotnet publish -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true -o ../../publish/linux-arm64
```

Sadece runtime paylaşılmış (framework-dependent) bir çıktı istiyorsanız `--self-contained false` parametresini kullanabilir ve Pi üzerinde .NET 8 runtime kurabilirsiniz.

## Dosyaları Raspberry Pi'ye Kopyalama

```bash
scp ../../publish/linux-arm64/Sniffer.Console pi@raspberrypi.local:/home/pi/sniffer/
```

Pi üzerinde dosyaların çalıştırılabilir olduğundan emin olun:

```bash
ssh pi@raspberrypi.local
chmod +x /home/pi/sniffer/Sniffer.Console
```

## Uygulamayı Çalıştırma

Paket yakalamak için genellikle root yetkisi gerekir:

```bash
sudo ./Sniffer.Console
```

Root olmadan çalıştırmak için gerekli yetkileri `setcap` ile atayabilirsiniz. Ancak bu yöntemin güvenlik etkilerini dikkatle değerlendirin.

```bash
sudo setcap cap_net_raw,cap_net_admin+eip ./Sniffer.Console
./Sniffer.Console
```

> Not: Birçok senaryoda root olarak çalıştırmak daha güvenilir ve basittir. `setcap` kullanımı bazı dağıtımlarda ek paketler gerektirebilir.

## Hizmeti Durdurma

Uygulamayı sonlandırmak için `Ctrl+C` kullanabilirsiniz.
