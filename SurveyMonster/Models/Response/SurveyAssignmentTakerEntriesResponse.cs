
namespace Lms.Survey.Application.Dto.Survey.Response
{
    public class SurveyAssignmentTakerEntriesResponse
    {
        public int? Id { get; set; }
        public int SurveyAssignmentTakerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public float? Score { get; set; }
        public bool IsLive { get; set; }
        public int? TenantId { get; set; }
        // Add other properties as needed
        // public SurveyAssignmentTakersResponse SurveyAssignmentTaker { get; set; }
        public List<SurveyAssignmentTakerEntryGivenAnswersResponse> Answers { get; set; }

    }
}
