# Design Document

## Overview

Bu tasarım dokümanı, SurveyMonster anket uygulamasına eklenecek yeni özelliklerin teknik tasarımını içerir. Mevcut ASP.NET Core MVC mimarisine uygun olarak, kullanıcı deneyimini iyileştiren özellikler eklenecektir.

### Temel Hedefler

1. Kullanıcıların ankete başlamadan önce bilgilendirilmesi ve onay alınması
2. Anket sorularının önizleme özelliği
3. Anonim kullanıcılardan bilgi toplama
4. Soru bazlı navigasyon ile tek tek soru gösterimi
5. Farklı soru tiplerini (çoktan seçmeli tek/çoklu, açık uçlu) destekleme

## Architecture

### Mevcut Mimari

Proje ASP.NET Core MVC mimarisi kullanmaktadır:
- **Controllers**: SurveyController, AuthController
- **Services**: SurveyService, AuthService, ApiClient
- **Models**: DTOs, ViewModels, Requests, Responses
- **Views**: Razor views (.cshtml)

### Yeni Bileşenler

1. **AnonymousUserInfo Model**: Anonim kullanıcı bilgilerini tutacak
2. **SurveyPreviewViewModel**: Önizleme için view model
3. **QuestionNavigationViewModel**: Soru bazlı navigasyon için view model
4. **JavaScript Modülleri**: İstemci tarafı etkileşimler için

## Components and Interfaces

### 1. Ön Bilgilendirme ve Onay Sistemi

#### View Değişiklikleri (Index.cshtml)

**Mevcut Durum:**
- InformationText gösteriliyor
- Doğrudan "Ankete Başla" butonu aktif

**Yeni Tasarım:**
```html
<div class="information-section">
    <div class="information-text">
        @Html.Raw(Model.InformationText)
    </div>
    
    <div class="consent-section">
        <div class="form-check">
            <input type="checkbox" 
                   class="form-check-input" 
                   id="consentCheckbox" 
                   required />
            <label class="form-check-label" for="consentCheckbox">
                Yukarıdaki bilgilendirme metnini okudum ve anladım
            </label>
        </div>
    </div>
    
    <button type="submit" 
            class="start-btn" 
            id="startSurveyBtn" 
            disabled>
        Ankete Başla
    </button>
</div>
```

**JavaScript Logic:**
```javascript
document.getElementById('consentCheckbox').addEventListener('change', function() {
    document.getElementById('startSurveyBtn').disabled = !this.checked;
});
```

### 2. Anket Önizleme Sistemi

#### Yeni ViewModel

```csharp
public class SurveyPreviewViewModel
{
    public int SurveyId { get; set; }
    public string SurveyName { get; set; }
    public List<PreviewQuestionViewModel> Questions { get; set; }
}

public class PreviewQuestionViewModel
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public int Order { get; set; }
    public int QuestionTypeId { get; set; }
    public string QuestionTypeName { get; set; }
    public List<OptionViewModel> Options { get; set; }
}
```

#### Controller Action

```csharp
[HttpGet]
public async Task<IActionResult> PreviewSurvey(int surveyId)
{
    var survey = await _surveyService.GetSurveyAsync(surveyId);
    
    var viewModel = new SurveyPreviewViewModel
    {
        SurveyId = survey.Id.Value,
        SurveyName = survey.Name,
        Questions = survey.SurveySurveyQuestionOrders
            .OrderBy(q => q.Order)
            .Select(q => new PreviewQuestionViewModel
            {
                QuestionId = q.SurveyQuestion.Id.Value,
                QuestionText = q.SurveyQuestion.Text,
                Order = q.Order.Value,
                QuestionTypeId = (int)q.SurveyQuestion.SurveyQuestionTypeId,
                QuestionTypeName = q.SurveyQuestion.SurveyQuestionType.Name,
                Options = q.SurveyQuestion.SurveySurveyQuestionOptions
                    .Select(o => new OptionViewModel
                    {
                        OptionId = o.Id.Value,
                        OptionText = o.Text
                    }).ToList()
            }).ToList()
    };
    
    return PartialView("_PreviewModal", viewModel);
}
```

#### Modal View (_PreviewModal.cshtml)

```html
<div class="modal fade" id="previewModal" tabindex="-1">
    <div class="modal-dialog modal-lg modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Anket Önizleme: @Model.SurveyName</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                @foreach (var question in Model.Questions)
                {
                    <div class="preview-question">
                        <h6>Soru @question.Order: @question.QuestionText</h6>
                        <p class="text-muted">Tip: @question.QuestionTypeName</p>
                        
                        @if (question.QuestionTypeId == 1 || question.QuestionTypeId == 2)
                        {
                            <ul class="list-unstyled">
                                @foreach (var option in question.Options)
                                {
                                    <li>• @option.OptionText</li>
                                }
                            </ul>
                        }
                        else if (question.QuestionTypeId == 3)
                        {
                            <div class="form-control" disabled>
                                [Açık uçlu metin alanı]
                            </div>
                        }
                    </div>
                }
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                    Kapat
                </button>
            </div>
        </div>
    </div>
</div>
```

### 3. Anonim Kullanıcı Bilgi Toplama

#### Yeni Model

```csharp
public class AnonymousUserInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
    
    public static AnonymousUserInfo FromJson(string json)
    {
        return JsonSerializer.Deserialize<AnonymousUserInfo>(json);
    }
}
```

#### Controller Logic

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> StartSurvey(int surveyId, AnonymousUserInfo anonymousInfo)
{
    var isAnonymous = IsAnonymousUser();
    
    if (isAnonymous)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Lütfen tüm alanları doldurun.";
            return RedirectToAction("Index", new { surveyId });
        }
        
        // Store anonymous info in session for later use
        HttpContext.Session.SetString("AnonymousUserInfo", anonymousInfo.ToJson());
    }
    
    // Continue with existing logic...
}
```

#### View Değişiklikleri

Anonim kullanıcılar için Index.cshtml'de form alanları:

```html
@if (User.Identity?.IsAuthenticated == false || IsAnonymous)
{
    <div class="anonymous-info-section">
        <h6>Lütfen bilgilerinizi girin:</h6>
        <div class="mb-3">
            <label for="firstName" class="form-label">Ad *</label>
            <input type="text" class="form-control" id="firstName" 
                   name="anonymousInfo.FirstName" required />
        </div>
        <div class="mb-3">
            <label for="lastName" class="form-label">Soyad *</label>
            <input type="text" class="form-control" id="lastName" 
                   name="anonymousInfo.LastName" required />
        </div>
        <div class="mb-3">
            <label for="email" class="form-label">E-posta *</label>
            <input type="email" class="form-control" id="email" 
                   name="anonymousInfo.Email" required />
        </div>
    </div>
}
```

### 4. Soru Bazlı Navigasyon

#### Yeni ViewModel

```csharp
public class QuestionNavigationViewModel
{
    public int SurveyId { get; set; }
    public string SurveyName { get; set; }
    public int EntryId { get; set; }
    public int CurrentQuestionIndex { get; set; }
    public int TotalQuestions { get; set; }
    public QuestionViewModel CurrentQuestion { get; set; }
    public Dictionary<int, string> SavedAnswers { get; set; } = new();
}
```

#### Controller Actions

```csharp
[HttpGet]
public async Task<IActionResult> TakeSurvey(int entryId, int questionIndex = 0)
{
    var surveyId = _configuration.GetValue<int>("Survey:DefaultSurveyId", 534);
    var survey = await _surveyService.GetSurveyAsync(surveyId);
    
    var questions = survey.SurveySurveyQuestionOrders
        .OrderBy(q => q.Order)
        .Select(q => new QuestionViewModel
        {
            QuestionId = q.SurveyQuestion.Id.Value,
            QuestionText = q.SurveyQuestion.Text,
            Order = q.Order.Value,
            QuestionTypeId = (int)q.SurveyQuestion.SurveyQuestionTypeId,
            Options = q.SurveyQuestion.SurveySurveyQuestionOptions
                .Select(o => new OptionViewModel
                {
                    OptionId = o.Id.Value,
                    OptionText = o.Text,
                    Value = o.Value
                }).ToList()
        }).ToList();
    
    // Get saved answers from session
    var savedAnswersJson = HttpContext.Session.GetString($"Answers_{entryId}");
    var savedAnswers = string.IsNullOrEmpty(savedAnswersJson) 
        ? new Dictionary<int, string>() 
        : JsonSerializer.Deserialize<Dictionary<int, string>>(savedAnswersJson);
    
    var viewModel = new QuestionNavigationViewModel
    {
        SurveyId = survey.Id.Value,
        SurveyName = survey.Name,
        EntryId = entryId,
        CurrentQuestionIndex = questionIndex,
        TotalQuestions = questions.Count,
        CurrentQuestion = questions[questionIndex],
        SavedAnswers = savedAnswers
    };
    
    return View(viewModel);
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult SaveAnswer(int entryId, int questionId, string answer, int nextIndex)
{
    // Get saved answers from session
    var savedAnswersJson = HttpContext.Session.GetString($"Answers_{entryId}");
    var savedAnswers = string.IsNullOrEmpty(savedAnswersJson) 
        ? new Dictionary<int, string>() 
        : JsonSerializer.Deserialize<Dictionary<int, string>>(savedAnswersJson);
    
    // Update answer
    savedAnswers[questionId] = answer;
    
    // Save back to session
    HttpContext.Session.SetString($"Answers_{entryId}", 
        JsonSerializer.Serialize(savedAnswers));
    
    return RedirectToAction("TakeSurvey", new { entryId, questionIndex = nextIndex });
}
```

#### View (TakeSurvey.cshtml - Yeni Tasarım)

```html
<div class="question-navigation">
    <div class="progress-indicator">
        Soru @(Model.CurrentQuestionIndex + 1) / @Model.TotalQuestions
    </div>
    
    <div class="question-card">
        <h5>@Model.CurrentQuestion.QuestionText</h5>
        
        <form asp-action="SaveAnswer" method="post">
            @Html.AntiForgeryToken()
            <input type="hidden" name="entryId" value="@Model.EntryId" />
            <input type="hidden" name="questionId" value="@Model.CurrentQuestion.QuestionId" />
            
            @* Question type specific rendering *@
            @if (Model.CurrentQuestion.QuestionTypeId == 1)
            {
                @* Single choice *@
                @foreach (var option in Model.CurrentQuestion.Options)
                {
                    <div class="form-check">
                        <input type="radio" name="answer" value="@option.OptionId" 
                               id="option_@option.OptionId" 
                               @(Model.SavedAnswers.ContainsKey(Model.CurrentQuestion.QuestionId) && 
                                 Model.SavedAnswers[Model.CurrentQuestion.QuestionId] == option.OptionId.ToString() 
                                 ? "checked" : "") />
                        <label for="option_@option.OptionId">@option.OptionText</label>
                    </div>
                }
            }
            else if (Model.CurrentQuestion.QuestionTypeId == 2)
            {
                @* Multiple choice *@
                @foreach (var option in Model.CurrentQuestion.Options)
                {
                    <div class="form-check">
                        <input type="checkbox" name="answer" value="@option.OptionId" 
                               id="option_@option.OptionId" />
                        <label for="option_@option.OptionId">@option.OptionText</label>
                    </div>
                }
            }
            else if (Model.CurrentQuestion.QuestionTypeId == 3)
            {
                @* Open ended *@
                <textarea name="answer" class="form-control" rows="5" 
                          placeholder="Cevabınızı buraya yazın...">@(Model.SavedAnswers.ContainsKey(Model.CurrentQuestion.QuestionId) ? Model.SavedAnswers[Model.CurrentQuestion.QuestionId] : "")</textarea>
            }
            
            <div class="navigation-buttons">
                @if (Model.CurrentQuestionIndex > 0)
                {
                    <button type="submit" name="nextIndex" value="@(Model.CurrentQuestionIndex - 1)" 
                            class="btn btn-secondary">
                        ← Geri
                    </button>
                }
                
                @if (Model.CurrentQuestionIndex < Model.TotalQuestions - 1)
                {
                    <button type="submit" name="nextIndex" value="@(Model.CurrentQuestionIndex + 1)" 
                            class="btn btn-primary">
                        İleri →
                    </button>
                }
                else
                {
                    <button type="button" class="btn btn-success" 
                            onclick="submitSurvey(@Model.EntryId)">
                        Anketi Tamamla
                    </button>
                }
            </div>
        </form>
    </div>
</div>
```

### 5. Farklı Soru Tiplerini Destekleme

#### QuestionViewModel Güncellemesi

```csharp
public class QuestionViewModel
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int Order { get; set; }
    public int QuestionTypeId { get; set; } // 1: Single, 2: Multiple, 3: Open
    public string QuestionTypeName { get; set; } = string.Empty;
    public List<OptionViewModel> Options { get; set; } = new();
}
```

#### JavaScript Helper

```javascript
function renderQuestionByType(question) {
    const container = document.getElementById('questionContainer');
    
    switch(question.questionTypeId) {
        case 1: // Single choice
            return renderRadioButtons(question);
        case 2: // Multiple choice
            return renderCheckboxes(question);
        case 3: // Open ended
            return renderTextarea(question);
        default:
            return '<p>Bilinmeyen soru tipi</p>';
    }
}

function renderRadioButtons(question) {
    let html = '';
    question.options.forEach(option => {
        html += `
            <div class="form-check">
                <input type="radio" name="answer" value="${option.optionId}" 
                       id="option_${option.optionId}" class="form-check-input" />
                <label for="option_${option.optionId}" class="form-check-label">
                    ${option.optionText}
                </label>
            </div>
        `;
    });
    return html;
}

function renderCheckboxes(question) {
    let html = '';
    question.options.forEach(option => {
        html += `
            <div class="form-check">
                <input type="checkbox" name="answer[]" value="${option.optionId}" 
                       id="option_${option.optionId}" class="form-check-input" />
                <label for="option_${option.optionId}" class="form-check-label">
                    ${option.optionText}
                </label>
            </div>
        `;
    });
    return html;
}

function renderTextarea(question) {
    return `
        <textarea name="answer" class="form-control" rows="5" 
                  placeholder="Cevabınızı buraya yazın..."></textarea>
    `;
}
```

### 6. Cevap İşleme Sistemi

#### Answer Processing Logic

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> SubmitSurvey(int entryId)
{
    // Get saved answers from session
    var savedAnswersJson = HttpContext.Session.GetString($"Answers_{entryId}");
    var savedAnswers = JsonSerializer.Deserialize<Dictionary<int, string>>(savedAnswersJson);
    
    // Add anonymous user info if exists
    var anonymousInfoJson = HttpContext.Session.GetString("AnonymousUserInfo");
    if (!string.IsNullOrEmpty(anonymousInfoJson))
    {
        // Add as a special question answer
        savedAnswers[-1] = anonymousInfoJson; // Use -1 as special key for user info
    }
    
    var tenantId = _configuration.GetValue<int>("Survey:DefaultTenantId", 1);
    
    // Save all answers
    foreach (var answer in savedAnswers)
    {
        if (answer.Key == -1) continue; // Skip user info, handle separately
        
        var saveRequest = new SaveAnswerRequest
        {
            SurveyAssignmentTakerEntryId = entryId,
            SurveyQuestionId = answer.Key,
            Answer = answer.Value,
            IsEmpty = string.IsNullOrEmpty(answer.Value),
            TenantId = tenantId
        };
        
        await _surveyService.SaveAnswerAsync(saveRequest);
    }
    
    // Save anonymous user info if exists
    if (!string.IsNullOrEmpty(anonymousInfoJson))
    {
        var userInfoRequest = new SaveAnswerRequest
        {
            SurveyAssignmentTakerEntryId = entryId,
            SurveyQuestionId = 0, // Special ID for user info
            Answer = anonymousInfoJson,
            IsEmpty = false,
            TenantId = tenantId
        };
        
        await _surveyService.SaveAnswerAsync(userInfoRequest);
    }
    
    // Finish survey
    await _surveyService.FinishSurveyEntryAsync(entryId);
    
    // Clear session
    HttpContext.Session.Remove($"Answers_{entryId}");
    HttpContext.Session.Remove("AnonymousUserInfo");
    HttpContext.Session.Remove(SurveyEntryIdKey);
    
    return RedirectToAction("Completed");
}
```

#### Multiple Choice Answer Processing

```javascript
function collectMultipleChoiceAnswers() {
    const checkboxes = document.querySelectorAll('input[name="answer[]"]:checked');
    const selectedIds = Array.from(checkboxes).map(cb => cb.value);
    return selectedIds.join(';'); // "11814;11815;11816"
}
```

## Data Models

### Existing Models (No Changes)

- `SurveyDetailDto`
- `SurveyQuestionResponse`
- `SaveAnswerRequest`
- `CreateSurveyEntryRequest`

### New Models

```csharp
// Models/AnonymousUserInfo.cs
public class AnonymousUserInfo
{
    [Required(ErrorMessage = "Ad alanı zorunludur")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Soyad alanı zorunludur")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "E-posta alanı zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin")]
    public string Email { get; set; }
    
    public string ToJson() => JsonSerializer.Serialize(this);
    public static AnonymousUserInfo FromJson(string json) => 
        JsonSerializer.Deserialize<AnonymousUserInfo>(json);
}

// Models/ViewModels/SurveyPreviewViewModel.cs
public class SurveyPreviewViewModel
{
    public int SurveyId { get; set; }
    public string SurveyName { get; set; }
    public List<PreviewQuestionViewModel> Questions { get; set; }
}

public class PreviewQuestionViewModel
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public int Order { get; set; }
    public int QuestionTypeId { get; set; }
    public string QuestionTypeName { get; set; }
    public List<OptionViewModel> Options { get; set; }
}

// Models/ViewModels/QuestionNavigationViewModel.cs
public class QuestionNavigationViewModel
{
    public int SurveyId { get; set; }
    public string SurveyName { get; set; }
    public int EntryId { get; set; }
    public int CurrentQuestionIndex { get; set; }
    public int TotalQuestions { get; set; }
    public QuestionViewModel CurrentQuestion { get; set; }
    public Dictionary<int, string> SavedAnswers { get; set; } = new();
}
```

### Updated Models

```csharp
// Models/ViewModels/QuestionViewModel.cs (Updated)
public class QuestionViewModel
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int Order { get; set; }
    public int QuestionTypeId { get; set; } // NEW: 1=Single, 2=Multiple, 3=Open
    public string QuestionTypeName { get; set; } = string.Empty; // NEW
    public List<OptionViewModel> Options { get; set; } = new();
}

// Models/ViewModels/SurveyInfoViewModel.cs (Updated)
public class SurveyInfoViewModel
{
    public int SurveyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InformationText { get; set; } = string.Empty;
    public DateTime ExpireDate { get; set; }
    public int QuestionCount { get; set; }
    public bool RequiresAnonymousInfo { get; set; } // NEW
}
```

## Error Handling

### Validation Errors

1. **Anonim Kullanıcı Bilgileri**
   - Boş alan kontrolü
   - E-posta format kontrolü
   - Model validation attributes kullanılacak

2. **Soru Cevaplama**
   - Zorunlu soru kontrolü
   - Cevap format kontrolü
   - Session timeout kontrolü

### Error Messages

```csharp
public static class ErrorMessages
{
    public const string RequiredField = "Bu alan zorunludur";
    public const string InvalidEmail = "Geçerli bir e-posta adresi girin";
    public const string SessionExpired = "Oturumunuz sona erdi. Lütfen tekrar başlayın";
    public const string AnswerRequired = "Lütfen soruyu cevaplayın";
    public const string SurveyNotFound = "Anket bulunamadı";
}
```

### Exception Handling

```csharp
try
{
    // Operation
}
catch (JsonException ex)
{
    _logger.LogError(ex, "JSON parsing error");
    TempData["Error"] = "Veri işleme hatası oluştu";
    return RedirectToAction("Index");
}
catch (HttpRequestException ex)
{
    _logger.LogError(ex, "API request failed");
    TempData["Error"] = "Sunucu ile bağlantı kurulamadı";
    return RedirectToAction("Index");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error");
    TempData["Error"] = "Beklenmeyen bir hata oluştu";
    return RedirectToAction("Index");
}
```

## Testing Strategy

### Unit Tests

1. **AnonymousUserInfo Model Tests**
   - JSON serialization/deserialization
   - Validation attributes

2. **Answer Processing Tests**
   - Single choice answer format
   - Multiple choice answer format (semicolon separated)
   - Open ended answer format

3. **Session Management Tests**
   - Answer storage
   - Answer retrieval
   - Session cleanup

### Integration Tests

1. **Survey Flow Tests**
   - Complete survey flow from start to finish
   - Navigation between questions
   - Answer persistence

2. **API Integration Tests**
   - SaveAnswerAsync with different answer formats
   - CreateSurveyEntryAsync
   - FinishSurveyEntryAsync

### UI Tests

1. **Consent Checkbox Tests**
   - Button disabled state
   - Button enabled on checkbox

2. **Preview Modal Tests**
   - Modal open/close
   - Question rendering by type

3. **Question Navigation Tests**
   - Forward navigation
   - Backward navigation
   - Answer persistence across navigation

### Manual Testing Checklist

- [ ] Ön bilgilendirme metni görüntüleniyor
- [ ] Checkbox işaretlenmeden buton devre dışı
- [ ] Checkbox işaretlenince buton aktif
- [ ] Önizleme butonu çalışıyor
- [ ] Modal'da tüm sorular görüntüleniyor
- [ ] Anonim kullanıcı bilgi formu görüntüleniyor
- [ ] Form validasyonu çalışıyor
- [ ] Tek soru gösterimi çalışıyor
- [ ] İleri/Geri navigasyon çalışıyor
- [ ] Cevaplar kaydediliyor
- [ ] Çoktan seçmeli tek seçim (radio) çalışıyor
- [ ] Çoktan seçmeli çoklu seçim (checkbox) çalışıyor
- [ ] Açık uçlu (textarea) çalışıyor
- [ ] Çoklu seçim cevapları noktalı virgülle birleşiyor
- [ ] Anket tamamlama çalışıyor

## Implementation Notes

### Session Management

- Cevaplar session'da `Answers_{entryId}` key'i ile saklanacak
- Anonim kullanıcı bilgileri `AnonymousUserInfo` key'i ile saklanacak
- Session timeout: 30 dakika (appsettings.json'da yapılandırılabilir)

### Performance Considerations

- Session kullanımı minimize edilecek
- Büyük anketler için pagination düşünülebilir
- API çağrıları optimize edilecek (batch operations)

### Security Considerations

- CSRF token kullanımı devam edecek
- XSS koruması için HTML encoding
- Session hijacking koruması
- Input validation her seviyede

### Browser Compatibility

- Modern browsers (Chrome, Firefox, Safari, Edge)
- JavaScript ES6+ features
- Bootstrap 5 components
- Responsive design

### Accessibility

- ARIA labels
- Keyboard navigation
- Screen reader support
- Color contrast compliance

## Survey Report Improvements

### Problem Statement

Mevcut `SurveyReportViewModel` ve `ReportAnswerViewModel` servisten dönen tüm verileri karşılamıyor. Özellikle:

1. **Eksik Alanlar**: `QuestionTypeId`, `QuestionTypeName`, `AnswerOrj`, `AnswerOther`, `PersonnelNo`, `EntryId`, `EntryFinishDate`
2. **Çoklu Seçim Gösterimi**: Aynı kullanıcının aynı soruya birden fazla seçenek işaretlemesi düzgün gösterilmiyor
3. **Açık Uçlu Sorular**: Gösterim kontrol edilmeli ve iyileştirilmeli

### Updated ViewModels

```csharp
// Models/ViewModels/SurveyReportViewModel.cs (Updated)
public class SurveyReportViewModel
{
    public long AssignmentId { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public int SurveyId { get; set; }
    public string SurveyName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ReportAnswerViewModel> Answers { get; set; } = new();
    public Dictionary<string, List<ReportAnswerViewModel>> GroupedByQuestion { get; set; } = new();
    
    // NEW: Group by question and user for better display
    public Dictionary<string, Dictionary<string, List<ReportAnswerViewModel>>> GroupedByQuestionAndUser { get; set; } = new();
}

// Models/ViewModels/ReportAnswerViewModel.cs (Updated)
public class ReportAnswerViewModel
{
    public long AnswerId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string PersonnelNo { get; set; } = string.Empty; // NEW
    public int QuestionOrder { get; set; }
    public long SurveyQuestionId { get; set; } // NEW
    public string QuestionText { get; set; } = string.Empty;
    public long QuestionTypeId { get; set; } // NEW
    public string QuestionTypeName { get; set; } = string.Empty; // NEW
    public string Answer { get; set; } = string.Empty;
    public string AnswerOrj { get; set; } = string.Empty; // NEW - Original answer
    public string AnswerOther { get; set; } = string.Empty; // NEW - "Other" option text
    public long? EntryId { get; set; } // NEW
    public DateTime EntryDate { get; set; }
    public DateTime? EntryFinishDate { get; set; } // NEW
    public int CompletionStatus { get; set; }
}
```

### Controller Updates

```csharp
[HttpGet]
public async Task<IActionResult> SurveyReport(long assignmentId)
{
    if (!CheckAuthentication())
    {
        return RedirectToAction("Login", "Auth");
    }

    if (IsAnonymousUser())
    {
        TempData["Error"] = ErrorMessages.AnonymousUserRestriction;
        return RedirectToAction("Index", "Survey");
    }

    try
    {
        var reportData = await _surveyService.GetSurveyReportAsync(assignmentId);
        if (reportData == null || !reportData.Any())
        {
            TempData["Error"] = "Rapor bulunamadı.";
            return RedirectToAction("MySurveys");
        }

        var firstEntry = reportData.First();
        var viewModel = new SurveyReportViewModel
        {
            AssignmentId = firstEntry.AsgId,
            AssignmentTitle = firstEntry.AsgTitle ?? "",
            SurveyId = (int)firstEntry.SurveyId,
            SurveyName = firstEntry.SurveyName ?? "",
            StartDate = firstEntry.AsgStartDate.GetValueOrDefault(),
            EndDate = firstEntry.AsgEndDate.GetValueOrDefault(),
            Answers = reportData.Select(r => new ReportAnswerViewModel
            {
                AnswerId = r.AnswerId.GetValueOrDefault(),
                UserName = r.UserName ?? "",
                FullName = $"{r.Name} {r.Surname}".Trim(),
                EmailAddress = r.EmailAddress ?? "",
                PersonnelNo = r.PersonnelNo ?? "", // NEW
                QuestionOrder = r.QuestionOrder,
                SurveyQuestionId = r.SurveyQuestionId, // NEW
                QuestionText = r.QuestionText ?? "",
                QuestionTypeId = r.QuestionTypeId, // NEW
                QuestionTypeName = r.QuestionTypeName ?? "", // NEW
                Answer = r.Answer ?? r.AnswerOrj ?? "",
                AnswerOrj = r.AnswerOrj ?? "", // NEW
                AnswerOther = r.AnswerOther ?? "", // NEW
                EntryId = r.EntryId, // NEW
                EntryDate = r.EntryStartDate.GetValueOrDefault(),
                EntryFinishDate = r.EntryFinishDate, // NEW
                CompletionStatus = r.CompletionStatus
            }).ToList()
        };

        // Group by question for better display
        viewModel.GroupedByQuestion = viewModel.Answers
            .GroupBy(a => a.QuestionText)
            .ToDictionary(g => g.Key, g => g.ToList());

        // NEW: Group by question and user for multiple choice display
        viewModel.GroupedByQuestionAndUser = viewModel.Answers
            .GroupBy(a => a.QuestionText)
            .ToDictionary(
                g => g.Key,
                g => g.GroupBy(a => a.UserName)
                      .ToDictionary(ug => ug.Key, ug => ug.ToList())
            );

        return View(viewModel);
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "API request failed while loading survey report");
        TempData["Error"] = ErrorMessages.ApiConnectionError;
        return RedirectToAction("MySurveys");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error loading survey report");
        TempData["Error"] = "Rapor yüklenirken bir hata oluştu.";
        return RedirectToAction("MySurveys");
    }
}
```

### View Updates

#### Question Type Based Display

```razor
@foreach (var question in Model.GroupedByQuestion.OrderBy(q => q.Value.First().QuestionOrder))
{
    var answers = question.Value;
    var firstAnswer = answers.First();
    var questionTypeId = firstAnswer.QuestionTypeId;
    var totalCount = answers.Count;

    <div class="question-section animated">
        <h5 class="mb-3">
            <span class="badge bg-primary me-2">Soru @firstAnswer.QuestionOrder</span>
            @question.Key
            <span class="badge bg-secondary ms-2">@firstAnswer.QuestionTypeName</span>
        </h5>

        @if (questionTypeId == 1 || questionTypeId == 2)
        {
            <!-- Multiple Choice Questions -->
            <div class="answer-chart">
                <h6 class="mb-3">Cevap Dağılımı:</h6>
                @{
                    var answerCounts = new Dictionary<string, int>();
                    
                    foreach (var answer in answers)
                    {
                        var displayAnswer = string.IsNullOrEmpty(answer.AnswerOther) 
                            ? answer.Answer 
                            : $"{answer.Answer} (Diğer: {answer.AnswerOther})";
                        
                        if (answerCounts.ContainsKey(displayAnswer))
                        {
                            answerCounts[displayAnswer]++;
                        }
                        else
                        {
                            answerCounts[displayAnswer] = 1;
                        }
                    }
                    
                    var sortedAnswers = answerCounts.OrderByDescending(a => a.Value);
                }
                
                @foreach (var answerItem in sortedAnswers)
                {
                    var count = answerItem.Value;
                    var percentage = (count * 100.0 / totalCount);

                    <div class="answer-bar">
                        <div class="answer-bar-label" title="@answerItem.Key">
                            @answerItem.Key
                        </div>
                        <div class="answer-bar-fill" style="width: @(percentage)%; min-width: 60px;">
                            <span style="padding-left: 8px; color: white; font-weight: 600;">
                                @percentage.ToString("F0")%
                            </span>
                        </div>
                        <div class="answer-bar-count">@count kişi</div>
                    </div>
                }
            </div>

            <hr class="my-3" />

            <!-- Detailed Answers for Multiple Choice -->
            <h6 class="mb-3">Detaylı Cevaplar:</h6>
            
            @if (questionTypeId == 2)
            {
                <!-- Multiple Selection - Group by user -->
                @foreach (var userGroup in Model.GroupedByQuestionAndUser[question.Key])
                {
                    var userAnswers = userGroup.Value;
                    var firstUserAnswer = userAnswers.First();
                    
                    <div class="answer-row">
                        <div class="row g-2">
                            <div class="col-md-3">
                                <strong>@firstUserAnswer.FullName</strong>
                                <br />
                                <small class="text-muted">@firstUserAnswer.EmailAddress</small>
                                @if (!string.IsNullOrEmpty(firstUserAnswer.PersonnelNo))
                                {
                                    <br />
                                    <small class="text-muted">Personel No: @firstUserAnswer.PersonnelNo</small>
                                }
                            </div>
                            <div class="col-md-6">
                                <i class="bi bi-check-square text-success me-2"></i>
                                <strong>Seçilen Cevaplar:</strong>
                                <ul class="mb-0 mt-1">
                                    @foreach (var answer in userAnswers)
                                    {
                                        <li>
                                            @answer.Answer
                                            @if (!string.IsNullOrEmpty(answer.AnswerOther))
                                            {
                                                <span class="badge bg-info ms-2">
                                                    Diğer: @answer.AnswerOther
                                                </span>
                                            }
                                        </li>
                                    }
                                </ul>
                            </div>
                            <div class="col-md-3 text-end">
                                <small class="text-muted">
                                    <i class="bi bi-clock me-1"></i>
                                    @firstUserAnswer.EntryDate.ToString("dd.MM.yyyy HH:mm")
                                </small>
                            </div>
                        </div>
                    </div>
                }
            }
            else
            {
                <!-- Single Selection -->
                @foreach (var answer in answers.OrderBy(a => a.UserName))
                {
                    <div class="answer-row">
                        <div class="row g-2">
                            <div class="col-md-3">
                                <strong>@answer.FullName</strong>
                                <br />
                                <small class="text-muted">@answer.EmailAddress</small>
                                @if (!string.IsNullOrEmpty(answer.PersonnelNo))
                                {
                                    <br />
                                    <small class="text-muted">Personel No: @answer.PersonnelNo</small>
                                }
                            </div>
                            <div class="col-md-6">
                                <i class="bi bi-check-circle text-success me-2"></i>
                                <strong>@answer.Answer</strong>
                                @if (!string.IsNullOrEmpty(answer.AnswerOther))
                                {
                                    <br />
                                    <span class="badge bg-info mt-1">
                                        Diğer: @answer.AnswerOther
                                    </span>
                                }
                            </div>
                            <div class="col-md-3 text-end">
                                <small class="text-muted">
                                    <i class="bi bi-clock me-1"></i>
                                    @answer.EntryDate.ToString("dd.MM.yyyy HH:mm")
                                </small>
                            </div>
                        </div>
                    </div>
                }
            }
        }
        else if (questionTypeId == 3)
        {
            <!-- Open Ended Questions -->
            <h6 class="mb-3">Cevaplar:</h6>
            @foreach (var answer in answers.OrderBy(a => a.UserName))
            {
                <div class="answer-row">
                    <div class="row g-2">
                        <div class="col-md-3">
                            <strong>@answer.FullName</strong>
                            <br />
                            <small class="text-muted">@answer.EmailAddress</small>
                            @if (!string.IsNullOrEmpty(answer.PersonnelNo))
                            {
                                <br />
                                <small class="text-muted">Personel No: @answer.PersonnelNo</small>
                            }
                        </div>
                        <div class="col-md-6">
                            <div class="open-ended-answer">
                                <i class="bi bi-chat-left-text text-primary me-2"></i>
                                <div class="answer-text">
                                    @if (!string.IsNullOrEmpty(answer.Answer))
                                    {
                                        <p class="mb-0">@answer.Answer</p>
                                    }
                                    else
                                    {
                                        <p class="text-muted mb-0"><i>Cevap verilmedi</i></p>
                                    }
                                </div>
                            </div>
                        </div>
                        <div class="col-md-3 text-end">
                            <small class="text-muted">
                                <i class="bi bi-clock me-1"></i>
                                @answer.EntryDate.ToString("dd.MM.yyyy HH:mm")
                            </small>
                        </div>
                    </div>
                </div>
            }
        }
    </div>
}
```

#### Additional CSS

```css
.open-ended-answer {
    display: flex;
    align-items: flex-start;
}

.answer-text {
    flex: 1;
    padding: 0.75rem;
    background: #f8f9fa;
    border-radius: 8px;
    border-left: 3px solid #667eea;
}

.answer-text p {
    white-space: pre-wrap;
    word-wrap: break-word;
}

.question-section .badge {
    font-size: 0.85rem;
}
```

### Data Flow

1. **Service Layer**: `GetSurveyReportAsync` returns complete data including all fields
2. **Controller**: Maps all service data to updated ViewModels
3. **View**: Displays data based on `QuestionTypeId`:
   - Type 1 (Single Choice): Shows single answer per user
   - Type 2 (Multiple Choice): Groups multiple answers per user
   - Type 3 (Open Ended): Shows text answers with proper formatting

### Benefits

1. **Complete Data**: All service fields are now captured in ViewModels
2. **Better UX**: Multiple choice answers are grouped by user
3. **Clear Display**: Question types are clearly indicated
4. **"Other" Support**: AnswerOther field is displayed separately
5. **User Info**: Personnel number and other user details are shown
