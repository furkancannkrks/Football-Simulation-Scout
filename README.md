# Football Analytics & Match Simulation Dashboard

Bu proje, modern bir futbol veri analitiği ve maç simülasyonu uygulamasıdır. Başlangıçta Python ile prototiplenmiş, daha sonra ölçeklenebilirlik ve performans gereksinimleri doğrultusunda **ASP.NET Core 10** mimarisine taşınmıştır.

## 🚀 Öne Çıkan Özellikler

* **Gelişmiş Scouting Sistemi:** Veritabanındaki binlerce oyuncuyu; pozisyon, yaş (opsiyonel), şut, pas, savunma metrikleri gibi 30'dan fazla parametre ile filtreleme imkanı.
* **Sunucu Taraflı Reyting (Weighted Rating):** Oyuncu performanslarını hesaplarken yükü uygulama katmanından alıp veritabanına devreden, PostgreSQL `PERCENT_RANK()` pencere fonksiyonlarını kullanan performans odaklı mimari.
* **İnteraktif Maç Simülasyonu:**
    * Sürükle-bırak (Drag & Drop) destekli futbol sahası arayüzü.
    * Dinamik diziliş (Formation) seçimi (4-3-3, 4-4-2 vb.).
    * xG (Gol Beklentisi), anahtar pas ve savunma aksiyonlarını temel alan olasılıksal simülasyon algoritması.
* **Responsive Tasarım:** Dark mode temalı, mobil uyumlu ve modern kullanıcı arayüzü.

## 🛠️ Teknoloji Yığını

### Backend
* **Framework:** ASP.NET Core 10 (Web API)
* **ORM:** Entity Framework Core (Database-First yaklaşımı)
* **Veritabanı:** PostgreSQL
* **Mimari:** N-Tier Architecture (Repository, Service, Controller, DTO)
* **Asenkron Programlama:** Tüm I/O işlemleri `async/await` yapısı ile thread-safe olarak kurgulanmıştır.

### Frontend
* **Dil:** Saf JavaScript (Vanilla JS), HTML5, CSS3
* **UI Mantığı:** DOM Manipülasyonu, Fetch API, HTML5 Drag and Drop API
* **Tasarım:** CSS Grid ve Absolute Positioning ile dinamik saha yerleşimi.

## 📊 Algoritma Detayları

### Oyuncu Reyting Sistemi
Reytingler, oyuncunun tüm sezon istatistiklerinin ağırlıklandırılarak normalize edilmesiyle hesaplanır:
`raw_score = (xg * 2.5) + (key_passes * 0.8) + (defensive_actions * 0.15)`
Bu değer PostgreSQL üzerinde `PERCENT_RANK()` ile 1.0 - 10.0 arasına sıkıştırılarak veritabanı seviyesinde hesaplanır.

### Simülasyon Motoru
Seçilen 11'lerin toplam hücum ve savunma güçleri oranlanarak;
* Galibiyet/Beraberlik/Mağlubiyet ihtimalleri,
* 2.5 Alt/Üst baremleri,
* Karşılıklı Gol (KG) durumu hesaplanır.

## ⚙️ Kurulum ve Çalıştırma

1.  **Veritabanı Hazırlığı:**
    * PostgreSQL sunucunuzda gerekli tabloların (`player_season_stats`, `team_match_stats`) hazır olduğundan emin olun.
2.  **Bağlantı Ayarı:**
    * `appsettings.json` dosyasındaki `ConnectionStrings` bölümünü kendi PostgreSQL bilgilerinizle güncelleyin.
3.  **Çalıştırma:**
    ```bash
    dotnet restore
    dotnet run
    ```
4.  **Erişim:**
    * Tarayıcıdan `http://localhost:5000` (veya ilgili port) adresine gidin.
