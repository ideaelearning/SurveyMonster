# Implementation Plan

- [ ] 1. Set up project infrastructure and configuration
  - Configure appsettings.json with API base URL and authentication settings
  - Register HttpClient, services, and session middleware in Program.cs with DI
  - Add necessary NuGet packages (System.IdentityModel.Tokens.Jwt if needed)
  - _Requirements: 7.4, 7.5_

- [ ] 2. Implement core data models and DTOs
  - Create DTOs for API responses (SurveyDetailDto, SurveyQuestionDto, SurveyQuestionOptionDto, etc.)
  - Create request models (CreateSurveyEntryRequest, SaveAnswerRequest)
  - Create AuthResult model for authentication responses
  - Create ViewModels (LoginViewModel, SurveyInfoViewModel, QuestionViewModel)
  - _Requirements: 2.2, 3.1, 4.1, 5.1_

- [ ] 3. Implement API client service
  - [ ] 3.1 Create IApiClient interface and ApiClient implementation
    - Implement GetAsync<T> and PostAsync<T> methods
    - Add JWT token management (SetAuthToken method)
    - Implement error handling and logging
    - _Requirements: 5.3, 6.5_
  
  - [ ]* 3.2 Write unit tests for ApiClient
    - Test successful API calls with mock HttpClient
    - Test error handling scenarios
    - _Requirements: 5.6_

- [ ] 4. Implement authentication service
  - [ ] 4.1 Create IAuthService interface and AuthService implementation
    - Implement LoginAsync method to call authentication API
    - Implement token storage in session/cookie
    - Implement GetCurrentToken and IsAuthenticated methods
    - Implement LogoutAsync to clear session
    - _Requirements: 2.2, 2.3, 2.4_
  
  - [ ]* 4.2 Write unit tests for AuthService
    - Test successful login flow
    - Test failed authentication scenarios
    - Test token retrieval and validation
    - _Requirements: 2.4_

- [ ] 5. Implement survey service
  - [ ] 5.1 Create ISurveyService interface and SurveyService implementation
    - Implement GetSurveyByIdAsync to fetch survey details
    - Implement CreateSurveyEntryAsync to initialize survey participation
    - Implement SaveAnswerAsync to save user answers
    - Add error handling for API failures
    - _Requirements: 3.1, 4.2, 5.1, 5.2_
  
  - [ ]* 5.2 Write unit tests for SurveyService
    - Test survey data retrieval
    - Test survey entry creation
    - Test answer saving
    - _Requirements: 5.6_

- [ ] 6. Implement authentication controllers and views
  - [ ] 6.1 Create AuthController with Login actions
    - Implement Login GET action to display login form
    - Implement Login POST action to process authentication
    - Implement Logout action to clear session
    - Add model validation and error handling
    - _Requirements: 2.1, 2.2, 2.4, 2.5_
  
  - [ ] 6.2 Create Login view with modern styling
    - Design responsive login form with username and password fields
    - Add client-side validation
    - Style with modern CSS (cards, buttons, inputs)
    - Add error message display area
    - _Requirements: 2.1, 6.1, 6.2, 6.3, 6.6_

- [ ] 7. Implement landing page
  - [ ] 7.1 Update HomeController Index action
    - Return landing page view
    - _Requirements: 1.1_
  
  - [ ] 7.2 Create landing page view
    - Design modern landing page with two prominent buttons
    - Add "Login" button that redirects to Auth/Login
    - Add "Anonymous" button with disabled state and "Coming Soon" label
    - Apply modern, elegant styling with responsive design
    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 6.1, 6.2, 6.3_

- [ ] 8. Implement survey information page
  - [ ] 8.1 Create SurveyController with Details action
    - Fetch survey details using SurveyService
    - Pass data to view via SurveyInfoViewModel
    - Add authorization check for authenticated users
    - Handle API errors gracefully
    - _Requirements: 3.1, 3.2, 5.3_
  
  - [ ] 8.2 Create survey details view
    - Display survey name prominently at the top
    - Display survey information text
    - Display formatted expiration date
    - Add "Start Survey" button at the bottom
    - Apply modern card-based design with proper spacing
    - Add loading indicator for API calls
    - _Requirements: 3.2, 3.3, 3.4, 3.5, 3.6, 6.1, 6.2, 6.3, 6.5_

- [ ] 9. Implement survey question navigation
  - [ ] 9.1 Create Start action in SurveyController
    - Create survey entry via SurveyService
    - Store entry ID and survey data in session
    - Redirect to first question
    - _Requirements: 5.1, 4.1_
  
  - [ ] 9.2 Create Question action in SurveyController
    - Retrieve current question based on index from session
    - Build QuestionViewModel with question data
    - Determine if current question is the last one
    - Return question view
    - _Requirements: 4.1, 4.2, 4.3, 4.4_
  
  - [ ] 9.3 Create question view with modern UI
    - Display question text clearly
    - Display all answer options as radio buttons or cards
    - Show progress indicator (e.g., "Question 2 of 5")
    - Add "Next" button (enabled only when answer selected)
    - Show "Submit" button on last question instead of "Next"
    - Apply smooth transitions and modern styling
    - _Requirements: 4.3, 4.4, 4.5, 4.6, 4.7, 6.1, 6.2, 6.3, 6.4, 6.5, 6.6_

- [ ] 10. Implement answer submission
  - [ ] 10.1 Create Answer action in SurveyController
    - Receive selected answer from form POST
    - Save answer using SurveyService
    - Handle API errors with retry option
    - If not last question, redirect to next question
    - If last question, redirect to completion page
    - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.6_
  
  - [ ] 10.2 Create completion view
    - Display thank you message
    - Add option to return to home or view another survey
    - Apply consistent styling
    - _Requirements: 5.5, 6.1, 6.2, 6.3_

- [ ] 11. Implement global styling and responsive design
  - Create or update site.css with modern design system
  - Define color scheme, typography, and spacing variables
  - Ensure all pages are fully responsive (mobile, tablet, desktop)
  - Add smooth transitions and animations
  - Implement loading indicators and hover states
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6_

- [ ] 12. Add error handling and user feedback
  - Implement global error handling middleware
  - Create error view for unexpected errors
  - Add user-friendly error messages for API failures
  - Implement retry mechanisms for failed API calls
  - Add validation feedback on forms
  - _Requirements: 5.6, 6.5_

- [ ]* 13. Integration testing
  - Test complete authentication flow
  - Test complete survey taking flow from start to finish
  - Test error scenarios and recovery
  - Test responsive design on multiple devices
  - _Requirements: All_
