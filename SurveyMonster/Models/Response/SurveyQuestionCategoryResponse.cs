using Lms.Survey.Application.Dto.SurveyCategory.Response;
using Lms.Survey.Application.Dto.SurveyQuestion.Response;
using Lms.Survey.Application.Dto.SurveyQuestionBank.Response;

namespace Lms.Survey.Application.Dto.SurveyQuestionCategory.Response
{
    public class SurveyQuestionCategoryResponse
    {
        public int? Id { get; set; }
        public int? TenantId { get; set; }
        public int? SurveyQuestionId { get; set; }
        public SurveyQuestionResponse SurveyQuestion { get; set; }
        public int SurveyCategoryId { get; set; }
        public SurveyCategoryResponse SurveyCategory { get; set; }
        public int? SurveyQuestionBankId { get; set; }
        public SurveyQuestionBankResponse SurveyQuestionBank { get; set; }
    }
}
