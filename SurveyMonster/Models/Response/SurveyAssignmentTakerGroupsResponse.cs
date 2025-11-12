using SurveyMonster.Models.Response;

namespace Lms.Survey.Application.Dto.Survey.Response
{
    public class SurveyAssignmentTakerGroupsResponse
    {
        public int Id { get; set; }
        public int SurveyAssignmentId { get; set; }
        public bool IsAutoAssign { get; set; }
        public int? TenantId { get; set; }
        // Add other properties as needed
        public SurveyAssignmentsResponse SurveyAssignment { get; set; }

    }
}
