# 🔐 ASP.NET Core Identity & JWT Authentication Projesi

Bu proje, Murat Yücedağ'ın Udemy'deki **"AspNet Core ile Güvenlik: Identity & JWT Masterclass"** eğitimini temel alarak geliştirilmiştir. Projede ASP.NET Core Identity ile kullanıcı yönetimi ve JWT (JSON Web Token) ile kimlik doğrulama işlemleri uygulanmıştır.

## 🚀 Özellikler

- ✅ ASP.NET Core MVC mimarisi
- ✅ ASP.NET Core Identity ile kullanıcı yönetimi
- ✅ Aktivasyon kodu ile mail doğrulama
- ✅ JSON Web Token (JWT) ile Token üretimi ve doğrulama
- ✅ Google hesabı ile giriş (Google OAuth)
- ✅ Kullanıcı kayıt ve giriş işlemleri
- ✅ Yetkilendirme ve kimlik doğrulama yapıları
- ✅ Temiz kod yaklaşımı

## 🛠️ Kullanılan Teknolojiler

- ASP.NET Core 9.0
- ASP.NET Core Identity
- JWT (System.IdentityModel.Tokens.Jwt)
- Entity Framework Core
- SQL Server
- Google OAuth
- MVC (Model-View-Controller)
- .NET Dependency Injection

## 🔐 Giriş Yöntemleri

1. **Kullanıcı Adı ve Şifre ile Giriş:**
   - Login ekranından kayıt olma işlemi gerçekleştirir.
   - Kayıt işleminden sonra mailine gelen doğrulama kodu ile mail doğrulaması gerçekleştirir.
   - Mail doğrulama işleminden sonra giriş yapabilir.

2. **Google Hesabı ile Giriş:**
   - Google OAuth üzerinden kullanıcı doğrulaması yapılır.
   - Google’dan alınan bilgiler ile otomatik kullanıcı kaydı.

## 🔧 Kurulum

1. EmailContext sınıfına kendi Connection String adresinizi girdikten sonra migration yapabilirsiniz.
2. Google aktivasyon kodu için ve google ile giriş ekranı için gerekli kişisel key leri eklemeniz gerekir
