# Design Document - Modern Survey Interface

## Overview

Bu tasarım dokümanı, SurveyMonster uygulaması için modern ve şık bir anket arayüzü geliştirme sürecini detaylandırır. Sistem, ASP.NET Core MVC mimarisi üzerine inşa edilecek, JWT tabanlı kimlik doğrulama kullanacak ve API-first yaklaşımıyla backend servisleriyle iletişim kuracaktır.

Temel özellikler:
- Modern, responsive ve kullanıcı dostu arayüz
- JWT tabanlı güvenli kimlik doğrulama
- API-driven veri yönetimi
- Dependency Injection mimarisi
- Sıralı soru gösterimi ve cevap kaydetme

## Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │   Landing    │  │    Login     │  │    Survey    │  │
│  │     View     │  │     View     │  │     View     │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                   Controller Layer                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │    Home      │  │     Auth     │  │    Survey    │  │
│  │  Controller  │  │  Controller  │  │  Controller  │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                    Service Layer (DI)                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │     Auth     │  │    Survey    │  │   HttpClient │  │
│  │   Service    │  │   Service    │  │   Service    │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                    External API Layer                    │
│              test-api.elearningsolutions.net             │
└─────────────────────────────────────────────────────────┘
```

### Technology Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Authentication**: JWT (JSON Web Token)
- **HTTP Client**: HttpClient with IHttpClientFactory
- **DI Container**: Built-in ASP.NET Core DI
- **Frontend**: Razor Views + Modern CSS (Bootstrap 5 / Custom)
- **JavaScript**: Vanilla JS / jQuery for interactivity

## Components and Interfaces

### 1. Service Layer

#### IAuthService
```csharp
public interface IAuthService
{
    Task<AuthResult> LoginAsync(string username, string password);
    Task LogoutAsync();
    string? GetCurrentToken();
    bool IsAuthenticated();
}
```

#### ISurveyService
```csharp
public interface ISurveyService
{
    Task<SurveyDetailDto> GetSurveyByIdAsync(int surveyId);
    Task<int> CreateSurveyEntryAsync(CreateSurveyEntryRequest request);
    Task<int> SaveAnswerAsync(SaveAnswerRequest request);
    Task<SurveyAssignmentDto> GetUserSurveyAssignmentAsync(int userId);
}
```

#### IApiClient
```csharp
public interface IApiClient
{
    Task<T> GetAsync<T>(string endpoint);
    Task<T> PostAsync<T>(string endpoint, object data);
    void SetAuthToken(string token);
}
```

### 2. Controllers

#### HomeController
- **Index**: Landing page with Anonymous/Login options
- Responsibilities: Display initial access options

#### AuthController
- **Login (GET)**: Display login form
- **Login (POST)**: Process authentication
- **Logout**: Clear session and redirect
- Responsibilities: Handle authentication flow

#### SurveyController
- **Details (GET)**: Display survey information page
- **Start (POST)**: Initialize survey entry
- **Question (GET)**: Display current question
- **Answer (POST)**: Save answer and navigate
- **Complete (GET)**: Display completion message
- Responsibilities: Manage survey flow and question navigation

### 3. View Models

#### LoginViewModel
```csharp
public class LoginViewModel
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
```

#### SurveyInfoViewModel
```csharp
public class SurveyInfoViewModel
{
    public int SurveyId { get; set; }
    public string Name { get; set; }
    public string InformationText { get; set; }
    public DateTime ExpireDate { get; set; }
}
```

#### QuestionViewModel
```csharp
public class QuestionViewModel
{
    public int SurveyId { get; set; }
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public List<OptionDto> Options { get; set; }
    public int CurrentQuestionIndex { get; set; }
    public int TotalQuestions { get; set; }
    public bool IsLastQuestion { get; set; }
}
```

## Data Models

### DTOs (Data Transfer Objects)

#### SurveyDetailDto
```csharp
public class SurveyDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string InformationText { get; set; }
    public DateTime ExpireDate { get; set; }
    public List<SurveyQuestionOrderDto> SurveySurveyQuestionOrders { get; set; }
}
```

#### SurveyQuestionOrderDto
```csharp
public class SurveyQuestionOrderDto
{
    public int Id { get; set; }
    public int Order { get; set; }
    public int SurveyQuestionId { get; set; }
    public SurveyQuestionDto SurveyQuestion { get; set; }
}
```

#### SurveyQuestionDto
```csharp
public class SurveyQuestionDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public List<SurveyQuestionOptionDto> SurveySurveyQuestionOptions { get; set; }
}
```

#### SurveyQuestionOptionDto
```csharp
public class SurveyQuestionOptionDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int Value { get; set; }
}
```

#### AuthResult
```csharp
public class AuthResult
{
    public bool Success { get; set; }
    public string Token { get; set; }
    public int UserId { get; set; }
    public string ErrorMessage { get; set; }
}
```

### Request Models

#### CreateSurveyEntryRequest
```csharp
public class CreateSurveyEntryRequest
{
    public int SurveyAssignmentTakerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public int SurveyState { get; set; }
}
```

#### SaveAnswerRequest
```csharp
public class SaveAnswerRequest
{
    public int SurveyAssignmentTakerEntryId { get; set; }
    public int SurveyQuestionId { get; set; }
    public string Answer { get; set; }
    public bool IsEmpty { get; set; }
    public int TenantId { get; set; }
}
```

## User Flow

### Authentication Flow
```
1. User lands on Home/Index
2. User clicks "Login" button
3. System displays Auth/Login view
4. User enters credentials
5. System calls AuthService.LoginAsync()
6. AuthService calls API authentication endpoint
7. On success: Store JWT token in session/cookie
8. Redirect to Survey/Details
```

### Survey Taking Flow
```
1. User views Survey/Details (survey info page)
2. User clicks "Start Survey" button
3. System creates survey entry via API
4. System stores entry ID in session
5. System redirects to Survey/Question?index=0
6. User sees first question with options
7. User selects an option
8. User clicks "Next"
9. System saves answer via API
10. System increments question index
11. Repeat steps 6-10 for each question
12. On last question, show "Submit" button
13. After submission, redirect to Survey/Complete
```

## Error Handling

### API Error Handling
- **Network Errors**: Display user-friendly message with retry option
- **Authentication Errors**: Clear token and redirect to login
- **Validation Errors**: Display field-specific error messages
- **Server Errors (5xx)**: Display generic error message and log details

### Error Response Model
```csharp
public class ApiErrorResponse
{
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public Dictionary<string, string[]> Errors { get; set; }
}
```

### Error Handling Strategy
1. Wrap all API calls in try-catch blocks
2. Log errors using ILogger
3. Return user-friendly error messages
4. Maintain application state on errors
5. Provide retry mechanisms where appropriate

## Testing Strategy

### Unit Tests
- **Service Layer**: Mock IApiClient, test business logic
- **Controllers**: Mock services, test action results
- **ViewModels**: Test validation attributes

### Integration Tests
- **API Communication**: Test actual API endpoints (with test environment)
- **Authentication Flow**: Test login/logout cycle
- **Survey Flow**: Test complete survey taking process

### UI Tests
- **Responsive Design**: Test on multiple screen sizes
- **Form Validation**: Test client-side validation
- **Navigation**: Test question navigation flow

### Test Coverage Goals
- Service Layer: 80%+ coverage
- Controllers: 70%+ coverage
- Critical paths (auth, survey submission): 90%+ coverage

## Security Considerations

### JWT Token Management
- Store token in HttpOnly cookie (preferred) or session storage
- Include token in Authorization header for all API requests
- Implement token expiration handling
- Clear token on logout

### Input Validation
- Validate all user inputs on client and server side
- Sanitize HTML content in survey text
- Prevent XSS attacks in question/answer display

### API Security
- Always use HTTPS for API communication
- Validate API responses before processing
- Implement request timeout mechanisms
- Handle CORS appropriately

## UI/UX Design Guidelines

### Design Principles
- **Modern**: Clean, minimalist design with contemporary aesthetics
- **Responsive**: Mobile-first approach, works on all devices
- **Accessible**: WCAG 2.1 AA compliance
- **Intuitive**: Clear navigation and user feedback

### Color Scheme (Suggested)
- Primary: #4F46E5 (Indigo)
- Secondary: #10B981 (Green)
- Background: #F9FAFB (Light Gray)
- Text: #111827 (Dark Gray)
- Error: #EF4444 (Red)

### Typography
- Headings: Inter or Poppins (Bold)
- Body: Inter or Roboto (Regular)
- Font sizes: Responsive scale (16px base)

### Component Styling
- **Cards**: Elevated with subtle shadows
- **Buttons**: Rounded corners, hover effects, clear CTAs
- **Forms**: Clean inputs with focus states
- **Progress**: Visual indicator for question progress

### Animation
- Smooth transitions between questions (fade/slide)
- Loading spinners for API calls
- Button hover/click feedback
- Form validation feedback animations

## Configuration

### appsettings.json
```json
{
  "ApiSettings": {
    "BaseUrl": "https://test-api.elearningsolutions.net",
    "Timeout": 30
  },
  "Authentication": {
    "TokenExpirationMinutes": 60
  },
  "Survey": {
    "DefaultTenantId": 1
  }
}
```

### Dependency Injection Registration
```csharp
// Program.cs
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
```

## Performance Considerations

### Optimization Strategies
- Cache survey data in session to reduce API calls
- Lazy load question data (fetch on-demand)
- Minimize JavaScript bundle size
- Optimize images and assets
- Use CDN for static resources

### API Call Optimization
- Fetch all survey questions once at start
- Store answers locally before submission
- Batch answer submissions if possible
- Implement request debouncing

## Deployment Considerations

### Environment Configuration
- Development: Local API or test environment
- Staging: Test API endpoint
- Production: Production API endpoint

### Build Process
- Minify CSS/JS for production
- Enable response compression
- Configure appropriate logging levels
- Set secure cookie policies
