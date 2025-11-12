using Lms.Survey.Application.Dto.SurveyQuestion.Response;

namespace Lms.Survey.Application.Dto.Survey.Response
{
    public class SurveyQuestionOrdersResponse
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public int SurveyId { get; set; }
        public int SurveyQuestionId { get; set; }
        public int? Order { get; set; }
        public int? SurveyChapterId { get; set; }
        // Add other properties as needed
        public virtual SurveyQuestionResponse SurveyQuestion { get; set; }
        public virtual SurveyChaptersResponse SurveyChapter { get; set; }
    }
}
