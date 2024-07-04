
# Ders İçeriği Kazıyıcı

Bu proje, verilen bir dersin içeriğini otomatik olarak tarayıp PDF formatında kaydeden bir Selenium uygulamasıdır. Uygulama, kullanıcı adı ve şifre ile giriş yaparak dersleri listeler ve seçilen dersin içeriğini toplar. İçerikler toplandıktan sonra benzersiz içerikler PDF dosyasına eklenir ve kaydedilir.

## Başlarken

### Gereksinimler

- .NET Core SDK
- Google Chrome
- Visual Studio (isteğe bağlı ancak önerilir)

### Kurulum

1. **Depoyu Klonlayın:**

   ```bash
   git clone <repository_url>
   cd <repository_directory>
   ```

2. **Gerekli Paketleri Yükleyin:**

   ```bash
   dotnet add package Selenium.WebDriver
   dotnet add package Selenium.WebDriver.ChromeDriver
   dotnet add package Selenium.Support
   dotnet add package itextsharp
   ```

3. **Giriş Bilgilerini Güncelleyin:**

   `Login` metodunda, kendi kullanıcı adı ve şifrenizi güncelleyin.

   ```csharp
   driver.FindElement(By.Name("kullanici_adi")).SendKeys("your_username");
   driver.FindElement(By.Name("sifre")).SendKeys("your_password");
   ```

### Projeyi Çalıştırma

1. **Projeyi Çalıştırın:**

   ```bash
   dotnet run
   ```

2. **Ders Seçimi Yapın:**

   Uygulama çalıştırıldığında, derslerin listesi ekranda görünecektir. Hangi dersi taramak istediğinizi seçmek için ilgili numarayı girin ve `Enter` tuşuna basın.

3. **PDF Oluşturma:**

   Uygulama, seçilen dersin içeriğini toplar ve benzersiz içerikleri bir PDF dosyasına kaydeder. PDF dosyası `C:\Turtep` dizinine kaydedilecektir. Dosyanın adı dersin adı olacaktır.

## Sorun Giderme

### Selenium ve ChromeDriver'ı Güncelleme

Eğer proje çalışmazsa, Selenium ve ChromeDriver'ın güncel olup olmadığını kontrol edin. Güncellemeler için aşağıdaki adımları izleyin:

1. **Selenium WebDriver'ı Güncelleyin:**

   ```bash
   dotnet add package Selenium.WebDriver --version <latest_version>
   ```

2. **ChromeDriver'ı Güncelleyin:**

   ```bash
   dotnet add package Selenium.WebDriver.ChromeDriver --version <latest_version>
   ```

3. **Google Chrome'u Güncelleyin:**

   Google Chrome'un en son sürümünü indirip kurarak tarayıcınızı güncelleyin.

4. **Bağımlılıkları Yeniden Yükleyin:**

   ```bash
   dotnet restore
   ```

## Katkıda Bulunma

Herhangi bir sorunla karşılaşırsanız veya iyileştirme önerileriniz varsa, lütfen bir `issue` açın veya bir `pull request` gönderin.

## Lisans

Bu proje MIT Lisansı ile lisanslanmıştır - detaylar için [LICENSE](LICENSE) dosyasına bakın.
