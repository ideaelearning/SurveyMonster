# Survey Monster - Kullan?m K?lavuzu

## ?? Proje Özeti

Survey Monster, kullan?c?lar?n JWT authentication ile giri? yaparak ankete kat?labildi?i bir ASP.NET Core Razor Pages uygulamas?d?r. Tüm veri i?lemleri harici API üzerinden gerçekle?tirilir.

## ?? Kurulum ve Çal??t?rma

### Gereksinimler
- .NET 8.0 SDK
- Visual Studio 2022 veya VS Code

### Projeyi Çal??t?rma

```bash
cd SurveyMonster
dotnet restore
dotnet run
```

Uygulama https://localhost:5001 adresinde çal??acakt?r.

## ?? Özellikler

### ? Tamamlanan Özellikler

1. **Authentication System**
   - JWT tabanl? giri? sistemi
   - Token yönetimi ve session storage
   - Güvenli logout i?lemi

2. **Survey Management**
   - Anket bilgilerini görüntüleme
   - Anket atamas? olu?turma
 - Kullan?c? anket kat?l?mc?s? olu?turma
   - Anket giri?i (entry) olu?turma

3. **Survey Taking**
   - Tüm sorular? tek sayfada gösterme
   - Çoktan seçmeli sorular için radio button'lar
   - Form validasyonu (tüm sorular?n cevaplanmas? zorunlu)
   - Cevaplar? API'ye kaydetme

4. **User Interface**
   - Modern Bootstrap 5 tasar?m
   - Türkçe dil deste?i
   - Responsive tasar?m
   - Icon deste?i (Bootstrap Icons)

## ?? Test Hesab?

```
Email: cem.kurt@ideaelearning.net
?ifre: Idea.123!
```

## ?? Proje Yap?s?

```
SurveyMonster/
??? Controllers/
?   ??? AuthController.cs    # Giri?/Ç?k?? i?lemleri
?   ??? SurveyController.cs        # Anket i?lemleri
?   ??? HomeController.cs          # Ana sayfa
??? Models/
?   ??? ApiResponse.cs     # API response wrapper
?   ??? AuthResult.cs  # Auth sonuç modeli
?   ??? DTOs/    # API DTO'lar?
?   ?   ??? LoginResponseDto.cs
?   ?   ??? IdResponseDto.cs
?   ?   ??? SurveyDetailDto.cs
?   ?   ??? SurveyQuestionDto.cs
?   ?   ??? SurveyQuestionOptionDto.cs
?   ?   ??? SurveyQuestionOrderDto.cs
?   ??? Requests/       # API request modelleri
?   ?   ??? LoginRequest.cs
?   ?   ??? CreateSurveyAssignmentRequest.cs
? ?   ??? CreateSurveyAssignmentTakerRequest.cs
?   ?   ??? CreateSurveyEntryRequest.cs
?   ?   ??? SaveAnswerRequest.cs
?   ??? ViewModels/           # View modelleri
?   ??? SurveyInfoViewModel.cs
?  ??? SurveyTakingViewModel.cs
?       ??? QuestionViewModel.cs
?       ??? OptionViewModel.cs
??? Services/
?   ??? IApiClient.cs        # API client interface
?   ??? ApiClient.cs           # HTTP istekleri
?   ??? IAuthService.cs # Auth service interface
?   ??? AuthService.cs    # Authentication logic
?   ??? ISurveyService.cs    # Survey service interface
?   ??? SurveyService.cs      # Survey i?lemleri
??? Views/
?   ??? Auth/
?   ?   ??? Login.cshtml    # Giri? sayfas?
?   ??? Survey/
? ?   ??? Index.cshtml    # Anket bilgisi
? ?   ??? TakeSurvey.cshtml      # Anket doldurma
?   ?   ??? Completed.cshtml       # Tamamlama sayfas?
?   ??? Home/
?   ?   ??? Index.cshtml           # Ana sayfa
?   ??? Shared/
? ??? _Layout.cshtml         # Layout
??? Program.cs          # DI configuration
??? appsettings.json          # Konfigürasyon

```

## ?? ??leyi? Ak???

1. **Giri? Yap** (`/Auth/Login`)
   - Email ve ?ifre ile giri?
   - JWT token al?n?r
   - Token session'a kaydedilir
   - Anket sayfas?na yönlendirilir

2. **Anket Bilgisi** (`/Survey/Index`)
   - API'den anket detaylar? çekilir
   - Anket bilgileri gösterilir
   - "Ankete Ba?la" butonuna t?klan?r

3. **Anket Ba?latma** (`/Survey/StartSurvey` POST)
   - Survey Assignment olu?turulur
   - Survey Assignment Taker olu?turulur
   - Survey Entry olu?turulur
   - Entry ID session'a kaydedilir
   - Anket sayfas?na yönlendirilir

4. **Anket Doldurma** (`/Survey/TakeSurvey`)
   - Tüm sorular tek sayfada gösterilir
   - Her soru için seçenekler radio button olarak
   - Kullan?c? tüm sorular? cevaplar

5. **Anket Gönderme** (`/Survey/SubmitSurvey` POST)
   - Her cevap için API'ye istek at?l?r
   - Survey entry tamamland? olarak i?aretlenir
   - Tamamlama sayfas?na yönlendirilir

## ??? API Entegrasyonu

### Kullan?lan Endpoint'ler

1. **Authentication**
   - `POST /api/identity/auth/login` - Giri?

2. **Survey Operations**
   - `GET /api/Surveys/survey/{id}` - Anket detaylar?
   - `POST /api/surveys/surveyAssignment` - Anket atamas? olu?tur
   - `POST /api/surveys/surveyAssignmentTaker` - Kat?l?mc? olu?tur
   - `POST /api/surveys/surveyAssignmentTakerEntry` - Giri? olu?tur
   - `POST /api/surveys/surveyAssignmentTakerEntryGivenAnswer` - Cevap kaydet

## ?? Konfigürasyon

`appsettings.json` dosyas?nda:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://test-api.elearningsolutions.net",
    "Timeout": 30
  },
  "Survey": {
    "DefaultTenantId": 1,
    "DefaultSurveyId": 534
  }
}
```

## ?? Dependency Injection

Tüm servisler DI container'a kay?tl?:

```csharp
builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();
```

## ?? Hata Yönetimi

- Try-catch bloklar? ile exception handling
- Structured logging (ILogger)
- Kullan?c? dostu hata mesajlar?
- TempData ile hata/ba?ar? mesajlar?

## ?? Session Yönetimi

- **AuthToken**: JWT token
- **UserId**: Kullan?c? ID
- **SurveyEntryId**: Aktif anket giri?i ID

## ?? UI/UX

- Bootstrap 5 framework
- Bootstrap Icons
- Responsive design
- Türkçe arayüz
- Form validasyonlar?
- Kullan?c? dostu mesajlar

## ?? Notlar

- Her kullan?c? ankete sadece bir kez kat?labilir
- Tüm sorular cevaplanmadan anket gönderilemez
- Token expire oldu?unda kullan?c? otomatik login sayfas?na yönlendirilir
- API timeout süresi 30 saniye

## ?? Güvenlik

- JWT authentication
- Session-based token storage
- HTTPS zorunlu
- Anti-forgery tokens
- XSS protection

## ?? Gelecek ?yile?tirmeler

- [ ] Anket sonuçlar?n? görüntüleme
- [ ] Çoklu anket deste?i
- [ ] Anket geçmi?i
- [ ] Profil yönetimi
- [ ] Admin paneli
- [ ] Anket istatistikleri

## ?? Katk?da Bulunma

1. Fork yap?n
2. Feature branch olu?turun (`git checkout -b feature/AmazingFeature`)
3. De?i?ikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'inizi push edin (`git push origin feature/AmazingFeature`)
5. Pull Request olu?turun

## ?? ?leti?im

Proje Sahibi: Survey Monster Team
