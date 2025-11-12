# Anonim Anket Kat?l?m? Özelli?i

## ?? Genel Bak??

Kullan?c?lar art?k ankete **iki farkl? ?ekilde** kat?labilir:
1. **Giri? Yaparak** - Kay?tl? kullan?c? hesab?yla
2. **Anonim Olarak** - Giri? yapmadan, kimlik bilgisi vermeden

---

## ?? Özellikler

### 1. Login Ekran? (Auth/Login)
- Kullan?c?ya iki seçenek sunulur:
  - **"Anonim Olarak Kat?l"** - Ye?il buton (Bootstrap success)
  - **"Giri? Yap"** - Mavi buton ile email/?ifre formu

### 2. Anonim Kullan?c? Ak???
```
Login Sayfas? 
    ?
"Anonim Olarak Kat?l" t?kla
    ?
AnonymousEntry action ça?r?l?r
 ?
Session'a IsAnonymous = "true" kaydedilir
    ?
Anket sayfas?na yönlendirilir
    ?
Anket ba?lat?l?r (UserId = null, AnonymousId = IP+UserAgent hash)
```

### 3. Kay?tl? Kullan?c? Ak???
```
Login Sayfas?
    ?
Email/?ifre gir
    ?
API'ye giri? iste?i
    ?
Session'a IsAnonymous = "false" kaydedilir
    ?
Anket sayfas?na yönlendirilir
    ?
Anket ba?lat?l?r (UserId dolu, AnonymousId = IP+UserAgent hash)
```

---

## ?? Teknik Detaylar

### Session Keys
```csharp
const string IsAnonymousKey = "IsAnonymous";  // "true" veya "false"
const string UserIdKey = "UserId";           // int? (anonim için null)
const string SurveyEntryIdKey = "SurveyEntryId";
```

### AnonymousId Üretimi
```csharp
// IP adresi + UserAgent birle?tirilerek SHA256 hash üretilir
var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
var userAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "";
string merged = ip + userAgent;
string cleaned = Regex.Replace(merged, @"\s+", "");
string hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(cleaned)));
```

### CreateSurveyAssignmentTakerRequest
```csharp
public class CreateSurveyAssignmentTakerRequest
{
    public int SurveyAssignmentId { get; set; }
    public int? UserId { get; set; }   // Anonim için null
    public string? AnonymousId { get; set; }  // Her zaman dolu
    public string Discriminator { get; set; } = "SurveyAssignmentTaker";
}
```

---

## ?? K?s?tlamalar

### Anonim Kullan?c?lar için:
? **Anketlerim** sayfas?n? görüntüleyemez  
? **Anket Raporlar?**'n? görüntüleyemez  
? **Sadece anket doldurabilir**

### ?lgili Metodlar
```csharp
// MySurveys - Anonim kullan?c?lar engellenmi?tir
if (IsAnonymousUser())
{
    TempData["Error"] = "Anonim kullan?c?lar anket geçmi?ini görüntüleyemez.";
    return RedirectToAction("Index", "Survey");
}

// SurveyReport - Anonim kullan?c?lar engellenmi?tir
if (IsAnonymousUser())
{
    TempData["Error"] = "Anonim kullan?c?lar rapor görüntüleyemez.";
    return RedirectToAction("Index", "Survey");
}
```

---

## ?? UI/UX De?i?iklikleri

### Navbar (Layout)
**Anonim Kullan?c? için:**
```
[Survey Monster] [Anket] [??? Anonim Kullan?c?] [Ç?k??]
```

**Kay?tl? Kullan?c? için:**
```
[Survey Monster] [Ana Sayfa] [Anketlerim] [Yeni Anket] [Ç?k??]
```

### Login Sayfas?
- Büyük ye?il "Anonim Olarak Kat?l" butonu
- "veya" ay?r?c?
- Email/?ifre formu ve "Giri? Yap" butonu

---

## ?? API ?stekleri

### Anonim Kullan?c? için Survey Assignment
```json
{
    "surveyId": 534,
    "startDate": "2025-01-15",
    "endDate": "2025-02-14",
    "eventCategoryId": 5,
    "title": "Anonim Anket Kat?l?m?",
    "imperative": false,
    "surveyMaxTakeCount": 1,
    "examSecurityType": 0,
    "isAnonymous": true
}
```

### Anonim Kullan?c? için Survey Taker
```json
{
"surveyAssignmentId": 30909,
    "userId": null,
    "anonymousId": "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855",
    "discriminator": "SurveyAssignmentTaker"
}
```

---

## ? Test Senaryolar?

### 1. Anonim Kat?l?m
1. Uygulamay? aç
2. "Anonim Olarak Kat?l" butonuna t?kla
3. Anket sayfas?na yönlendirildi?ini do?rula
4. Anketi doldur ve gönder
5. "Anketlerim" linkinin görünmedi?ini do?rula

### 2. Kay?tl? Kullan?c? Kat?l?m?
1. Uygulamay? aç
2. Email/?ifre girerek giri? yap
3. Anket sayfas?na yönlendirildi?ini do?rula
4. Anketi doldur ve gönder
5. "Anketlerim" linkini görüntüle

### 3. Ç?k?? Yapma
1. Anonim veya kay?tl? kullan?c? olarak giri? yap
2. "Ç?k??" butonuna t?kla
3. Login sayfas?na yönlendirildi?ini do?rula
4. Session temizlendi?ini do?rula

---

## ?? Güvenlik

- **AnonymousId**: Ayn? IP+UserAgent her zaman ayn? hash'i üretir
- **Session bazl?**: Her kullan?c? session'? ba??ms?zd?r
- **Token kontrolü**: Hem anonim hem kay?tl? kullan?c?lar için authentication check yap?l?r
- **Yetkilendirme**: Anonim kullan?c?lar sadece anket doldurabilir

---

## ?? Notlar

- Anonim kullan?c?lar için dummy token: `"ANONYMOUS_USER"` kullan?l?r
- Her anonim kullan?c? benzersiz `AnonymousId` ile takip edilir
- Anonim kullan?c?lar anket geçmi?i ve raporlar? görüntüleyemez
- Ç?k?? yap?ld???nda tüm session bilgileri temizlenir
