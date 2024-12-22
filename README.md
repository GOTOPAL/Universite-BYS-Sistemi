# KTU BYS SYSTEM

## Proje Açıklaması

KTU BYS SYSTEM, öğrenciler ve danışmanlar için geliştirilen bir otomasyon sistemidir. Bu sistem sayesinde kullanıcılar aşağıdaki işlemleri gerçekleştirebilir:

- **Öğrenci Girişi**
- **Danışman Girişi**

## Özellikler

### Öğrenci Girişi

1. **Başarılı Giriş Durumu**:

   - Öğrenci, giriş yaptıktan sonra aşağıdaki işlemleri gerçekleştirebilir:
     - Kişisel bilgilerini güncelleyebilir.
     - Ders seçimi yapabilir ve bu seçimleri danışman onayına gönderebilir.
     - Onaylanan dersler, "Derslerim" sayfasında görüntülenir.

2. **Ders Seçimi Süreci**:

   - Öğrenci, ders seçimi yapar ve seçilen dersler danışman onayına sunulur.
   - Danışman onayladığında, dersler öğrencinin "Derslerim" sayfasında görünür.

3. **Danışman Öğretmen Bilgisi**:

   - Öğrenci, danışman öğretmeninin bilgilerini görüntüleyebilir.

### Danışman Girişi

1. **Başarılı Giriş Durumu**:

   - Danışman, giriş yaptıktan sonra aşağıdaki işlemleri gerçekleştirebilir:
     - Kişisel bilgilerini güncelleyebilir.
     - Kendisine bağlı öğrencilerin bilgilerini görebilir.
     - Öğrencilerin seçtiği dersleri onaylayabilir.

2. **Öğrenci Yönetimi**:

   - Danışman, kendisine bağlı olan tüm öğrencilerin bilgilerini ve durumlarını görüntüleyebilir.

3. **Ders Onayı**:

   - Danışman, öğrenciler tarafından seçilen dersleri inceleyip onaylayabilir.

## Kurulum Talimatları

1. **Projenin İndirilmesi**:

   - Projeyi indirmek veya klonlamak için aşağıdaki komutu kullanın:
     ```bash
     https://github.com/WuenxDEV/Universite-BYS-Sistemi.git
     ```

2. **Visual Studio ile Projeyi Açma**:

   - Visual Studio'yu açın ve **File** > **Open** > **Project/Solution** yolunu izleyerek `qBYS.sln` dosyasını seçin.

3. **Bağımlılıkların Yüklenmesi**:

   - Projenin bağımlılıklarını yüklemek için Visual Studio'da **NuGet Package Manager** kullanarak eksik paketleri yükleyin:
     - **Tools** > **NuGet Package Manager** > **Manage NuGet Packages for Solution...** yolunu izleyin.
     - Tüm paketlerin güncel olduğundan emin olun.

4. **Veritabanını Yükleme**:

   - Sağlanan `UniversityCourseSelection.bak` dosyasını SQL Server Management Studio (SSMS) kullanarak yükleyin:
     1. SSMS'i açın ve bir sunucuya bağlanın.
     2. Yeni bir veritabanı oluşturun veya mevcut bir veritabanını seçin.
     3. **File** > **Open** > **File...** yolunu izleyerek `UniversityCourseSelection.bak` dosyasını seçin.
     4. Dosyayı çalıştırarak tabloları ve verileri yükleyin.
   
   - Veritabanı bağlantı bilgilerini `appsettings.json` dosyasındaki `<connectionStrings>` bölümünde şu şekilde düzenleyin:
     ```xml
     <connectionStrings>
         <add name="DefaultConnection" connectionString="Server=YOUR_SERVER_NAME;Database=UniversityCourseSelection;User Id=YOUR_USER_ID;Password=YOUR_PASSWORD;" providerName="System.Data.SqlClient" />
     </connectionStrings>
     ```

5. **Projeyi Derleme**:

   - Visual Studio'da **Build** > **Build Solution** seçeneğine tıklayarak projeyi derleyin.
   - Derleme sırasında herhangi bir hata alırsanız, hataları kontrol edin ve gerekli düzenlemeleri yaparak işlemi tekrarlayın.

6. **Uygulamayı Çalıştırma**:

   - Projeyi çalıştırmak için Visual Studio'da aşağıdaki adımları izleyin:
     - **Debug** > **Start Without Debugging** seçeneğini seçin veya `Ctrl + F5` tuşlarına basın.
     - Proje, varsayılan olarak IIS Express üzerinde çalışacaktır.

7. **Veritabanı Bağlantısını Doğrulama**:

   - Uygulama giriş ekranına gidin ve kullanıcı adı/parola ile giriş yaparak bağlantının çalıştığından emin olun.

## Kullanım Talimatları

### Giriş Yapma:

- Sisteme giriş yapmak için öğrenci veya danışman giriş ekranını kullanın.

### Öğrenci İşlemleri:

- Kişisel bilgilerinizi güncelleyin.
- Ders seçimi yaparak onaya gönderin.
- Onaylanan derslerinizi "Derslerim" sekmesinde takip edin.

### Danışman İşlemleri:

- Kendi bilgilerinizi güncelleyin.
- Kendisine bağlı öğrencilerin bilgilerini görüntüleyin.
- Ders onayı verin.

## Teknolojiler

- **Backend**: ASP.NET Core
- **Frontend**: HTML, CSS, JavaScript
- **Veritabanı**: SQL Server

## Katkıda Bulunanlar

- **Geliştirici**: Göktuğ Oğuzhan TOPAL

## Lisans

Bu proje MIT Lisansı ile lisanslanmıştır. Daha fazla bilgi için `LICENSE` dosyasına göz atın.


