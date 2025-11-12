namespace Lms.Survey.Application.Dto.Survey.Response
{
    public class SurveyAssignmentTakerEntryGivenAnswersResponse
    {
        public int? Id { get; set; }
        public int SurveyAssignmentTakerEntryId { get; set; }
        public int SurveyQuestionId { get; set; }
        public string Answer { get; set; }
        public bool IsEmpty { get; set; }
        public int? TenantId { get; set; }
        // Add other properties as needed
        //public SurveyAssignmentTakerEntriesResponse SurveyAssignmentTakerEntry { get; set; }
        //public SurveyQuestionResponse SurveyQuestion { get; set; }
    }
}
