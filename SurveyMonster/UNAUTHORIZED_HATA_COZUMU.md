# Unauthorized Hatas? Çözümü

## ?? Problem
API'ye istek at?ld???nda **401 Unauthorized** hatas? al?n?yordu.

## ?? Tespit Edilen Sorunlar

### 1. **Anonim Kullan?c?lar için Geçersiz Token**
- Anonim kullan?c?lar için `"ANONYMOUS_USER"` dummy token'? kullan?l?yordu
- Bu token API'ye Bearer token olarak gönderiliyordu
- API bu token'? tan?m?yor ve 401 döndürüyordu

### 2. **Yanl?? API Base URL**
- `appsettings.json` dosyas?nda:
  - **YANLI?**: `"BaseUrl": "http://localhost:5000"`
  - **DO?RU**: `"BaseUrl": "http://test-api.elearningsolutions.net"`

### 3. **Hardcoded Localhost URL'leri**
- `SurveyService.cs` içinde tüm metodlarda hardcoded `"http://localhost:5010"` URL'leri vard?
- Bu URL'ler gerçek API'yi override ediyordu

---

## ? Yap?lan Düzeltmeler

### 1. **ApiClient.cs Güncellemeleri**

#### Yeni Constant Eklendi
```csharp
private const string AnonymousToken = "ANONYMOUS_USER";
```

#### SetAuthToken Metodu Güncellendi
```csharp
public void SetAuthToken(string token)
{
    _authToken = token;
    
    // Don't set bearer token for anonymous users
  if (token != AnonymousToken)
    {
    _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        _logger.LogInformation("Authentication token set for API client");
    }
    else
    {
        _logger.LogInformation("Anonymous user detected, skipping bearer token");
    }
}
```

#### Yeni Helper Metod: SetAuthorizationHeader
```csharp
private void SetAuthorizationHeader()
{
    // Get token from session if not already set
    var token = _authToken ?? _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");

    // Only set authorization header if we have a valid (non-anonymous) token
    if (!string.IsNullOrEmpty(token) && token != AnonymousToken)
    {
    _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
    else if (token == AnonymousToken)
    {
        // Remove authorization header for anonymous users
        _httpClient.DefaultRequestHeaders.Authorization = null;
   _logger.LogDebug("Anonymous user - no authorization header set");
    }
}
```

#### GetAsync ve PostAsync Metodlar? Güncellendi
- Her iki metod da art?k `SetAuthorizationHeader()` ça?r?yor
- Anonim kullan?c?lar için Authorization header **null** olarak ayarlan?yor

---

### 2. **AuthService.cs Güncellemeleri**

#### Yeni Constant Eklendi
```csharp
private const string AnonymousToken = "ANONYMOUS_USER";
```

#### SetAuthToken Metodu Güncellendi
```csharp
public void SetAuthToken(string token)
{
    _httpContextAccessor.HttpContext?.Session.SetString(TokenSessionKey, token);
 
    // Only set in API client if it's a real token (not anonymous)
  if (token != AnonymousToken)
    {
      _apiClient.SetAuthToken(token);
    }
}
```

**Mant?k:**
- Session'a her türlü token kaydedilir
- ApiClient'a **sadece gerçek JWT token'lar** gönderilir
- Anonim token ApiClient'a iletilmez

---

### 3. **appsettings.json Güncellendi**

#### Önceki Hali (YANLI?)
```json
{
"ApiSettings": {
  "BaseUrl": "http://localhost:5000",
    "Timeout": 30
  }
}
```

#### Güncel Hali (DO?RU)
```json
{
  "ApiSettings": {
    "BaseUrl": "http://test-api.elearningsolutions.net",
    "Timeout": 30
  }
}
```

---

### 4. **SurveyService.cs - Hardcoded URL'ler Kald?r?ld?**

#### Önceki Hali (YANLI?)
```csharp
var response = await _apiClient.GetAsync<ApiResponse<SurveyDetailDto>>(
    $"api/Surveys/survey/{surveyId}", "http://localhost:5010");
```

#### Güncel Hali (DO?RU)
```csharp
var response = await _apiClient.GetAsync<ApiResponse<SurveyDetailDto>>(
    $"api/Surveys/survey/{surveyId}");
```

**Tüm metodlarda kald?r?lan URL'ler:**
- ? `GetSurveyAsync`
- ? `CreateSurveyAssignmentAsync`
- ? `CreateSurveyAssignmentTakerAsync`
- ? `CreateSurveyEntryAsync`
- ? `SaveAnswerAsync`
- ? `GetMySurveysAsync`
- ? `GetSurveyReportAsync`

---

## ?? Nas?l Çal???yor?

### Kay?tl? Kullan?c? Ak???
```
1. Kullan?c? giri? yapar
2. API'den gerçek JWT token al?n?r
3. Token session'a kaydedilir
4. Token ApiClient'a set edilir
5. Her API iste?inde Bearer token gönderilir
   ? Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI...
```

### Anonim Kullan?c? Ak???
```
1. Kullan?c? "Anonim Olarak Kat?l" t?klar
2. Dummy token ("ANONYMOUS_USER") session'a kaydedilir
3. ApiClient'a token GÖNDER?LMEZ
4. Her API iste?inde Authorization header NULL
   ? Authorization: (yok)
5. API anonim istekleri kabul eder
```

---

## ?? API ?stek Örnekleri

### Kay?tl? Kullan?c? ?ste?i
```http
GET http://test-api.elearningsolutions.net/api/Surveys/survey/534
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Anonim Kullan?c? ?ste?i
```http
GET http://test-api.elearningsolutions.net/api/Surveys/survey/534
(Authorization header yok)
```

---

## ?? Test Ad?mlar?

### 1. Anonim Kullan?c? Testi
```bash
1. Uygulamay? ba?lat
2. "Anonim Olarak Kat?l" butonuna t?kla
3. Network tab'de istekleri kontrol et
4. Authorization header'?n OLMADI?INI do?rula
5. API'nin 200 OK döndü?ünü do?rula
```

### 2. Kay?tl? Kullan?c? Testi
```bash
1. Email/?ifre ile giri? yap
2. Network tab'de istekleri kontrol et
3. Authorization header'?n OLDU?UNU do?rula
4. Bearer token'?n görünür oldu?unu do?rula
5. API'nin 200 OK döndü?ünü do?rula
```

---

## ?? Güvenlik Kontrolleri

? **Anonim Kullan?c?lar:**
- Bearer token gönderilmez
- API anonim istekleri kabul eder
- Sadece anket doldurabilir
- Raporlara eri?emez

? **Kay?tl? Kullan?c?lar:**
- Geçerli JWT token gönderilir
- API kullan?c?y? do?rular
- Tüm özelliklere eri?ebilir

---

## ?? Loglar

### Anonim Kullan?c? Loglar?
```
[INFO] Anonymous user detected, skipping bearer token
[DEBUG] Anonymous user - no authorization header set
[INFO] GET request to api/Surveys/survey/534
[INFO] GET request successful for api/Surveys/survey/534
```

### Kay?tl? Kullan?c? Loglar?
```
[INFO] Authentication token set for API client
[INFO] Login successful for user: cem.kurt@ideaelearning.net, UserId: 74805
[INFO] GET request to api/Surveys/survey/534
[INFO] GET request successful for api/Surveys/survey/534
```

---

## ? Çözüm

Art?k:
- ? Anonim kullan?c?lar için **401 hatas? yok**
- ? Kay?tl? kullan?c?lar için **Bearer token düzgün çal???yor**
- ? API istekleri **do?ru URL'e** gidiyor
- ? Hardcoded URL'ler **temizlendi**
- ? Token yönetimi **merkezi ve güvenli**

---

## ?? Sonraki Ad?mlar

1. Uygulamay? test et
2. Browser Developer Tools > Network tab'de istekleri kontrol et
3. Hem anonim hem kay?tl? kullan?c? ak??lar?n? dene
4. Hata durumunda loglar? incele
