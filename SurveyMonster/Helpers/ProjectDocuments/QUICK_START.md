# ?? Survey Monster - H?zl? Ba?lang?ç

## 1?? Projeyi Çal??t?r

```bash
cd SurveyMonster
dotnet run
```

Taray?c?da aç: **https://localhost:5001**

## 2?? Giri? Yap

**Test Hesab?:**
```
Email: cem.kurt@ideaelearning.net
?ifre: Idea.123!
```

## 3?? Anket Ak???

1. ? Giri? yap ? Otomatik olarak anket sayfas?na yönlendirileceksin
2. ? "Ankete Ba?la" butonuna t?kla
3. ? 2 soruyu cevapla:
   - Soru 1: "Akp seçimi kazan?r m??"
   - Soru 2: "CHP yine seçimi kaybeder mi?"
4. ? "Anketi Tamamla" butonuna t?kla
5. ? Ba?ar? mesaj?n? gör!

## ?? Detayl? Dokümantasyon

- **KULLANIM_KILAVUZU.md** - Tam kullan?m rehberi
- **TEST_SCENARIOS.md** - Test senaryolar?
- **PROJE_OZETI.md** - Proje detaylar?
- **API_SPEC.md** - API dökümanlar?

## ?? Konfigürasyon

API URL ve di?er ayarlar? de?i?tirmek için:
**appsettings.json** dosyas?n? düzenle

```json
{
  "ApiSettings": {
    "BaseUrl": "https://test-api.elearningsolutions.net",
    "Timeout": 30
  },
  "Survey": {
    "DefaultSurveyId": 534
  }
}
```

## ? Sorun mu ya??yorsun?

1. **NuGet hatas?:** `dotnet restore`
2. **Build hatas?:** `dotnet clean` ? `dotnet build`
3. **Port kullan?mda:** appsettings.json'da port de?i?tir

## ?? Özellikler

? JWT Authentication  
? Anket Görüntüleme  
? Anket Doldurma  
? Cevap Kaydetme  
? Türkçe Arayüz  
? Responsive Design  

**Keyifli kullan?mlar!** ??
