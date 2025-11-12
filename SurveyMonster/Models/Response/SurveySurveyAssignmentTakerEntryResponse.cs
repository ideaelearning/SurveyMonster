using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lms.Survey.Application.Dto.Survey.Response
{
    public class SurveySurveyAssignmentTakerEntryResponse
    {
        public long Id { get; set; }
        public long SurveyAssignmentTakerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public float? Score { get; set; }
        public bool IsLive { get; set; }
        public int? TenantId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation: Given answers
        public List<SurveyAssignmentTakerEntryGivenAnswersResponse> GivenAnswers { get; set; }

    }
}
