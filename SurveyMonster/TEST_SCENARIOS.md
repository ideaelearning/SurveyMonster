# Test Senaryolar? ve Do?rulama

## ? Manuel Test Ad?mlar?

### 1. Authentication Testleri

#### Test 1.1: Ba?ar?l? Giri?
**Ad?mlar:**
1. Uygulamay? çal??t?r: `dotnet run`
2. Taray?c?da `https://localhost:5001` adresine git
3. Otomatik `/Auth/Login` sayfas?na yönlendirilmelisin
4. Test bilgilerini gir:
   - Email: `cem.kurt@ideaelearning.net`
   - ?ifre: `Idea.123!`
5. "Giri? Yap" butonuna t?kla

**Beklenen Sonuç:**
- Ba?ar?l? giri? yap?l?r
- `/Survey/Index` sayfas?na yönlendirilir
- Navbar'da "Ç?k??" butonu görünür

#### Test 1.2: Hatal? Giri?
**Ad?mlar:**
1. Login sayfas?na git
2. Hatal? bilgiler gir:
   - Email: `test@test.com`
   - ?ifre: `wrongpassword`
3. "Giri? Yap" butonuna t?kla

**Beklenen Sonuç:**
- Hata mesaj? görünür
- Login sayfas?nda kal?r

#### Test 1.3: Validasyon Kontrolü
**Ad?mlar:**
1. Login sayfas?nda bo? form submit et

**Beklenen Sonuç:**
- "Kullan?c? ad? gereklidir" ve "?ifre gereklidir" mesajlar? görünür
- Form submit edilmez

### 2. Anket Görüntüleme Testleri

#### Test 2.1: Anket Bilgilerini Görme
**Ad?mlar:**
1. Ba?ar?l? giri? yap
2. `/Survey/Index` sayfas?na yönlendirildi?ini kontrol et

**Beklenen Sonuç:**
- Anket ad?: "2026 seçimi vatanda?a soruyoruz"
- Bilgi metni görünür
- Soru say?s?: 2
- Son tarih: 12.12.2026
- "Ankete Ba?la" butonu aktif

#### Test 2.2: Authentication Kontrolü
**Ad?mlar:**
1. Logout yap
2. Taray?c? adres çubu?una `/Survey/Index` yaz
3. Enter'a bas

**Beklenen Sonuç:**
- Login sayfas?na yönlendirilir
- ReturnUrl parametresi ile

### 3. Anket Ba?latma Testleri

#### Test 3.1: Anket Ba?latma
**Ad?mlar:**
1. Login ol
2. Survey Index sayfas?nda "Ankete Ba?la" butonuna t?kla

**Beklenen Sonuç:**
- Survey Assignment olu?turulur (API'den ID dönmeli)
- Survey Assignment Taker olu?turulur
- Survey Entry olu?turulur
- `/Survey/TakeSurvey` sayfas?na yönlendirilir
- Entry ID session'a kaydedilir

### 4. Anket Doldurma Testleri

#### Test 4.1: Sorular? Görüntüleme
**Ad?mlar:**
1. Ankete ba?la
2. TakeSurvey sayfas?n? incele

**Beklenen Sonuç:**
- Tüm sorular tek sayfada görünür
- Soru 1: "Akp seçimi kazan?r m??"
  - Seçenek 1: Evet
  - Seçenek 2: Hay?r
- Soru 2: "CHP yine seçimi kaybeder mi?"
  - Seçenek 1: Hay?r
  - Seçenek 2: Evet

#### Test 4.2: Eksik Cevap Kontrolü
**Ad?mlar:**
1. Sadece 1. soruyu cevapla
2. "Anketi Tamamla" butonuna t?kla

**Beklenen Sonuç:**
- JavaScript alert: "Lütfen tüm sorular? cevaplay?n. (1/2 cevapland?)"
- Form submit edilmez

#### Test 4.3: Tüm Sorular? Cevaplama
**Ad?mlar:**
1. Her iki soruyu da cevapla
2. "Anketi Tamamla" butonuna t?kla
3. Confirmation dialog'da "OK" t?kla

**Beklenen Sonuç:**
- Her cevap için API'ye POST request gider
- Console'da success loglar? görünür
- `/Survey/Completed` sayfas?na yönlendirilir
- Session'dan entry ID temizlenir

### 5. Anket Tamamlama Testleri

#### Test 5.1: Tamamlama Sayfas?
**Ad?mlar:**
1. Anketi ba?ar?yla tamamla

**Beklenen Sonuç:**
- Ba?ar? mesaj? görünür: "Anket ba?ar?yla tamamland?..."
- "Yeni Anket" butonu var
- "Ç?k?? Yap" butonu var

#### Test 5.2: Yeni Anket
**Ad?mlar:**
1. Tamamlama sayfas?nda "Yeni Anket" butonuna t?kla

**Beklenen Sonuç:**
- `/Survey/Index` sayfas?na gider
- Yeni bir anket ba?lat?labilir

### 6. Logout Testleri

#### Test 6.1: Ç?k?? Yapma
**Ad?mlar:**
1. Login ol
2. Navbar'da "Ç?k??" butonuna t?kla

**Beklenen Sonuç:**
- Session temizlenir
- Login sayfas?na yönlendirilir
- Navbar'da "Giri? Yap" butonu görünür

## ?? API ?steklerini ?zleme

### Browser DevTools ile ?zleme
1. F12 tu?una bas
2. Network tab'?na git
3. Filter: XHR
4. ??lemleri gerçekle?tir

**Kontrol edilmesi gerekenler:**
- POST `/api/identity/auth/login` - 200 OK, token döner
- GET `/api/Surveys/survey/534` - 200 OK, survey detaylar?
- POST `/api/surveys/surveyAssignment` - 200 OK, assignment ID
- POST `/api/surveys/surveyAssignmentTaker` - 200 OK, taker ID
- POST `/api/surveys/surveyAssignmentTakerEntry` - 200 OK, entry ID
- POST `/api/surveys/surveyAssignmentTakerEntryGivenAnswer` (x2) - 200 OK, answer ID

## ?? Console Loglar?

### Application Logs (Visual Studio Output)
Program çal???rken a?a??daki loglar görülmeli:

```
info: SurveyMonster.Services.AuthService[0]
      Attempting login for user: cem.kurt@ideaelearning.net
info: SurveyMonster.Services.ApiClient[0]
      POST request to /api/identity/auth/login
info: SurveyMonster.Services.ApiClient[0]
      POST request successful for /api/identity/auth/login
info: SurveyMonster.Services.AuthService[0]
      Login successful for user: cem.kurt@ideaelearning.net, UserId: 74805
info: SurveyMonster.Services.SurveyService[0]
      Fetching survey details for surveyId: 534
info: SurveyMonster.Services.ApiClient[0]
      GET request to /api/Surveys/survey/534
info: SurveyMonster.Services.ApiClient[0]
      GET request successful for /api/Surveys/survey/534
```

## ?? Test Checklist

### Functional Tests
- [ ] Login with valid credentials
- [ ] Login with invalid credentials
- [ ] View survey information
- [ ] Start survey (creates assignment, taker, entry)
- [ ] Display all questions on one page
- [ ] Validate required answers
- [ ] Submit survey answers
- [ ] Complete survey successfully
- [ ] Logout

### UI/UX Tests
- [ ] Responsive design on mobile
- [ ] Responsive design on tablet
- [ ] Responsive design on desktop
- [ ] All icons display correctly
- [ ] Bootstrap styling applied
- [ ] Turkish language throughout

### Security Tests
- [ ] Cannot access survey without login
- [ ] Token stored in session
- [ ] Token sent with API requests
- [ ] CSRF protection on forms
- [ ] Logout clears session

### Error Handling Tests
- [ ] API timeout handling
- [ ] Network error handling
- [ ] Invalid survey ID
- [ ] Missing survey data
- [ ] API error responses

## ?? Debugging Tips

### 1. Enable Detailed Logging
`appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
 "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

### 2. Check Session Data
Controllers'da:
```csharp
var token = HttpContext.Session.GetString("AuthToken");
var userId = HttpContext.Session.GetInt32("UserId");
var entryId = HttpContext.Session.GetInt32("SurveyEntryId");
_logger.LogInformation($"Token: {token}, UserId: {userId}, EntryId: {entryId}");
```

### 3. Inspect API Responses
ApiClient.cs'de response content'i logla:
```csharp
var content = await response.Content.ReadAsStringAsync();
_logger.LogInformation("Response: {Content}", content);
```

## ?? Known Issues & Solutions

### Issue 1: Session Lost on Page Refresh
**Çözüm:** Session timeout'u art?r (appsettings.json)

### Issue 2: CORS Errors
**Çözüm:** API taraf?nda CORS ayarlar? yap?lmal?

### Issue 3: JWT Parsing Error
**Çözüm:** System.IdentityModel.Tokens.Jwt package yüklü olmal?

## ? Production Checklist

Proje production'a ç?kmadan önce:
- [ ] appsettings.Production.json olu?turuldu
- [ ] Gerçek API base URL ayarland?
- [ ] Logging seviyeleri ayarland?
- [ ] Error handling tamamland?
- [ ] HTTPS zorunlu
- [ ] Session timeout uygun
- [ ] Security headers eklendi
- [ ] Performance test yap?ld?
- [ ] Load test yap?ld?

## ?? Ba?ar? Kriterleri

Proje ba?ar?l? say?l?r e?er:
1. ? Kullan?c? giri? yapabilir
2. ? Anket bilgileri görüntülenebilir
3. ? Anket ba?lat?labilir
4. ? Tüm sorular tek sayfada görüntülenir
5. ? Cevaplar kaydedilebilir
6. ? Anket tamamlanabilir
7. ? Tüm API ça?r?lar? ba?ar?l?
8. ? Error handling çal???yor
9. ? UI/UX kullan?c? dostu
10. ? Türkçe dil deste?i tam
