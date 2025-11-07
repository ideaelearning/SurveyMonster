# Requirements Document

## Introduction

Bu doküman, SurveyMonster uygulaması için modern ve şık bir anket arayüzü geliştirme gereksinimlerini tanımlar. Sistem, kullanıcıların giriş yaparak veya anonim olarak anketlere katılmasını sağlayacak, anket bilgilerini görsel olarak sunacak ve soruları sıralı bir şekilde gösterecektir.

## Glossary

- **Survey System**: Anket yönetim ve katılım sistemi
- **Authentication Module**: Kullanıcı kimlik doğrulama modülü
- **Survey Interface**: Anket gösterim ve etkileşim arayüzü
- **Question Navigator**: Soru sıralama ve gezinme bileşeni
- **API Service**: Backend API ile iletişim servisi
- **JWT Token**: Kimlik doğrulama için kullanılan JSON Web Token
- **Survey Assignment**: Kullanıcıya atanmış anket
- **Survey Entry**: Anket katılım kaydı
- **Given Answer**: Kullanıcının verdiği cevap

## Requirements

### Requirement 1

**User Story:** As a user, I want to choose between anonymous and authenticated access on the landing page, so that I can participate in surveys according to my preference

#### Acceptance Criteria

1. WHEN THE user navigates to the application, THE Survey System SHALL display a landing page with two distinct access options
2. THE Survey System SHALL provide a "Login" button that redirects to the authentication flow
3. THE Survey System SHALL provide an "Anonymous" button that is visually disabled with a "Coming Soon" indicator
4. THE Survey System SHALL apply modern and elegant styling to the landing page with responsive design
5. THE Survey System SHALL ensure both buttons are clearly distinguishable and accessible

### Requirement 2

**User Story:** As an authenticated user, I want to log in with my credentials, so that I can access assigned surveys

#### Acceptance Criteria

1. WHEN THE user clicks the "Login" button, THE Authentication Module SHALL display a login form with username and password fields
2. WHEN THE user submits valid credentials, THE Authentication Module SHALL send a POST request to the API authentication endpoint
3. IF THE authentication is successful, THEN THE Authentication Module SHALL store the JWT token securely
4. WHEN THE authentication fails, THE Authentication Module SHALL display a clear error message to the user
5. AFTER successful authentication, THE Survey System SHALL redirect the user to the survey list or active survey page

### Requirement 3

**User Story:** As an authenticated user, I want to see the active survey information before starting, so that I understand what the survey is about

#### Acceptance Criteria

1. WHEN THE authenticated user accesses the survey page, THE Survey Interface SHALL fetch survey details from the API endpoint `/api/Surveys/survey/{id}`
2. THE Survey Interface SHALL display the survey name prominently at the top of the page
3. THE Survey Interface SHALL display the survey information text below the survey name
4. THE Survey Interface SHALL display the survey expiration date in a readable format
5. THE Survey Interface SHALL display a "Start Survey" button at the bottom of the information section
6. THE Survey Interface SHALL apply modern card-based design with appropriate spacing and typography

### Requirement 4

**User Story:** As a user taking a survey, I want to see questions one at a time in order, so that I can focus on each question individually

#### Acceptance Criteria

1. WHEN THE user clicks the "Start Survey" button, THE Question Navigator SHALL display the first question from the survey
2. THE Question Navigator SHALL display questions in the order specified by the `order` field in the API response
3. THE Question Navigator SHALL display the question text clearly with appropriate formatting
4. THE Question Navigator SHALL display all available answer options for the current question
5. THE Question Navigator SHALL allow the user to select one answer option per question
6. WHEN THE user selects an answer, THE Question Navigator SHALL enable a "Next" button to proceed to the next question
7. WHEN THE user is on the last question, THE Question Navigator SHALL display a "Submit" button instead of "Next"

### Requirement 5

**User Story:** As a user taking a survey, I want my answers to be saved when I submit, so that my participation is recorded

#### Acceptance Criteria

1. WHEN THE user clicks "Next" on a question, THE API Service SHALL create a survey entry if not already created using `/api/surveys/surveyAssignmentTakerEntry`
2. WHEN THE user selects an answer, THE API Service SHALL save the answer using `/api/surveys/surveyAssignmentTakerEntryGivenAnswer`
3. THE API Service SHALL include the JWT token in all authenticated API requests
4. WHEN THE user clicks "Submit" on the last question, THE Survey System SHALL save the final answer and mark the survey as complete
5. AFTER successful submission, THE Survey System SHALL display a thank you message to the user
6. IF ANY API request fails, THEN THE Survey System SHALL display an appropriate error message and allow the user to retry

### Requirement 6

**User Story:** As a user, I want the survey interface to be visually appealing and responsive, so that I have a pleasant experience on any device

#### Acceptance Criteria

1. THE Survey Interface SHALL use modern CSS frameworks or custom styles for a contemporary look
2. THE Survey Interface SHALL be fully responsive and work on mobile, tablet, and desktop screen sizes
3. THE Survey Interface SHALL use consistent color schemes, typography, and spacing throughout
4. THE Survey Interface SHALL provide smooth transitions between questions
5. THE Survey Interface SHALL display loading indicators during API calls
6. THE Survey Interface SHALL ensure all interactive elements have appropriate hover and focus states

### Requirement 7

**User Story:** As a developer, I want the application to follow dependency injection principles, so that the code is maintainable and testable

#### Acceptance Criteria

1. THE Survey System SHALL implement all services using dependency injection pattern
2. THE Survey System SHALL register API services in the DI container
3. THE Survey System SHALL register authentication services in the DI container
4. THE Survey System SHALL retrieve the base URL from appsettings configuration
5. THE Survey System SHALL separate concerns between controllers, services, and models
