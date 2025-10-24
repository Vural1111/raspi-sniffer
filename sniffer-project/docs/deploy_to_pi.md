# Raspberry Pi'ye Publish ve Çalıştırma

Bu doküman, WinForms ve konsol bileşenlerini içeren Raspi Sniffer çözümünü Raspberry Pi üzerinde çalıştırmak için gerekli adımları açıklar. Raspberry Pi üzerinde GUI olmadan konsol sürümü çalıştırılır.

## Ön Koşullar
- Raspberry Pi üzerinde 64-bit Raspberry Pi OS (Debian tabanlı) yüklü olmalı.
- Cihazda .NET 8 Runtime kurulmalı.
- SSH erişimi açık olmalı.

## 1. Projeyi Hazırlama
1. Geliştirme makinenizde tüm bağımlılıkları yükleyin ve çözümleri restore edin:
   ```bash
   dotnet restore sniffer-project.sln
   ```
2. Projeyi publish için temizleyin (opsiyonel):
   ```bash
   dotnet clean sniffer-project.sln
   ```

## 2. Konsol Uygulamasını Publish Etme
1. `sniffer-project` klasöründe aşağıdaki komutu çalıştırarak konsol sürümünü self-contained olarak publish edin:
   ```bash
   dotnet publish src/Sniffer.Console/Sniffer.Console.csproj -c Release -r linux-arm64 --self-contained true -o publish/console
   ```
   Bu komut, Raspberry Pi'nin 64-bit ARM mimarisine uygun bağımsız bir dağıtım oluşturur.

2. Eğer WinForms arayüzünü de Linux üzerinde kullanmak istiyorsanız (ör. Wine ile), benzer şekilde `Sniffer.UI` projesini `-r win-arm64` hedefiyle publish edebilirsiniz.

## 3. Paketleri Raspberry Pi'ye Kopyalama
1. Publish klasörünü SCP ile Raspberry Pi'ye aktarın:
   ```bash
   scp -r publish/console pi@raspberrypi.local:/home/pi/raspi-sniffer
   ```
2. Raspberry Pi üzerinde hedef klasör yoksa oluşturun:
   ```bash
   ssh pi@raspberrypi.local "mkdir -p ~/raspi-sniffer"
   ```
3. Dosyaların aktarımının tamamlandığından emin olun.

## 4. Raspberry Pi'de Çalıştırma
1. Raspberry Pi'ye SSH ile bağlanın:
   ```bash
   ssh pi@raspberrypi.local
   ```
2. Çalışma klasörüne geçin ve uygulamaya çalıştırma izni verin:
   ```bash
   cd ~/raspi-sniffer
   chmod +x Sniffer.Console
   ```
3. Root yetkileri ile çalıştırın (paket yakalama için gerekli olabilir):
   ```bash
   sudo ./Sniffer.Console
   ```
4. Uygulama açıldığında listelenen ağ arayüzlerinden birini seçin ve paketleri izlemeye başlayın.

## 5. Servis Olarak Çalıştırma (Opsiyonel)
1. Systemd servis dosyası oluşturun (`/etc/systemd/system/raspi-sniffer.service`):
   ```ini
   [Unit]
   Description=Raspi Sniffer Console Service
   After=network.target

   [Service]
   WorkingDirectory=/home/pi/raspi-sniffer
   ExecStart=/home/pi/raspi-sniffer/Sniffer.Console
   Restart=on-failure
   User=root

   [Install]
   WantedBy=multi-user.target
   ```
2. Servisi etkinleştirin ve başlatın:
   ```bash
   sudo systemctl enable raspi-sniffer.service
   sudo systemctl start raspi-sniffer.service
   ```
3. Logları kontrol edin:
   ```bash
   sudo journalctl -u raspi-sniffer.service -f
   ```

## 6. Sorun Giderme
- `SharpPcap`'ın ağ arayüzlerine erişebilmesi için kullanıcıya gerekli izinleri verin veya root yetkisiyle çalıştırın.
- Ağ arayüzleri listelenmiyorsa `libpcap` paketinin kurulu olduğundan emin olun:
  ```bash
  sudo apt-get install libpcap-dev
  ```
- Performans problemlerinde `--self-contained false` ve `-p:PublishReadyToRun=true` gibi seçeneklerle publish ayarlarınızı optimize edin.
