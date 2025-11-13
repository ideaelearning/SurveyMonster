using Lms.Survey.Application.Dto.Survey.Response;
using Lms.Survey.Application.Dto.SurveyType.Response;
using SurveyMonster.Models.Response;

namespace SurveyMonster.Models.DTOs
{
   
    //public class RootResponse
    //{
    //    public SurveyAssignmentData Data { get; set; }
    //}

    public class SurveyAssignmentResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? EventCode { get; set; }
        public string? Details { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SurveyMaxTakeCount { get; set; }
        public bool? Imperative { get; set; }
        public int DefaultGamificationPoint { get; set; }
        public string RequiredUserInformations { get; set; }
        public RequiredUserInformationsRequest? RequiredUserInformationsList =>
            !string.IsNullOrEmpty(RequiredUserInformations)
                ? System.Text.Json.JsonSerializer.Deserialize<RequiredUserInformationsRequest>(RequiredUserInformations)
                : null;
        public SurveyResponse Survey { get; set; }
        public bool IsExpired { get; set; }
        public bool IsActive { get; set; }
    }
    
}
