using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lms.Survey.Application.Dto.Survey.Response
{
    public class CoreEventAssignmentSurveyAssignmentResponse
    {

        public long Id { get; set; }
        public int? TenantId { get; set; }
        public long? OrganizationUnitId { get; set; }
        public long UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public long? GroupId { get; set; }
        public string GroupName { get; set; }
        public float? Grade { get; set; }
        public int CompletionStatus { get; set; }
        public DateTime? CompletionTime { get; set; }
        public DateTime? LastEnterDate { get; set; }
        public float? CompletionPercentage { get; set; }
        public decimal? Point { get; set; }
        public long? SurveyAssignmentId { get; set; }
        public long? SurveyAssignmentTrainingParticipantId { get; set; }
        public bool? IsAutoAssign { get; set; }
        public int? Status { get; set; }
        public string? ReasonForReject { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation: SurveyAssignmentTaker entries
        public List<SurveySurveyAssignmentTakerEntryResponse> SurveyAssignmentTakerEntries { get; set; }

    }
}
