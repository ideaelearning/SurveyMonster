using Lms.Survey.Application.Dto.SurveyType.Response;
using SurveyMonster.Models.Response;

namespace Lms.Survey.Application.Dto.Survey.Response
{
    public class MySurveyAssignmentResponse
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long? SurveyAssignmentId { get; set; }
        public int CompletionStatus { get; set; }
        public DateTime? CompletionTime { get; set; }
        public DateTime? LastEnterDate { get; set; }
        public float? CompletionPercentage { get; set; }
        public decimal? Point { get; set; }
        public bool? IsAutoAssign { get; set; }
        public DateTime CreationTime { get; set; }
        public int EntryCount { get; set; }
        public int SurveyEntryCompletedCount { get; set; }
        public SurveyAssignmentsResponse? SurveyAssignment { get; set; }

    }
    
     

  
}
