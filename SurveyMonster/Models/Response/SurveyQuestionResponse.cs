using Lms.Survey.Application.Dto.Survey.Response;
using Lms.Survey.Application.Dto.SurveyCategory.Response;
using Lms.Survey.Application.Dto.SurveyQuestionType.Response;

namespace Lms.Survey.Application.Dto.SurveyQuestion.Response
{
    public class SurveyQuestionResponse
    {
        public int? Id { get; set; }
        public const int maxLength = 90;
        public virtual bool UseNotMaxLength { get; set; }
        public virtual int? TenantId { get; set; }
        public virtual string Text { get; set; }

        public virtual long SurveyQuestionTypeId { get; set; }
        public virtual SurveyQuestionTypeResponse SurveyQuestionType { get; set; }
        public virtual string SurveyQuestionTypeText
        {
            get
            {
                if (SurveyQuestionType != null)
                {
                    return SurveyQuestionType.Name;

                }
                return "";
            }
        }

        public virtual long SurveyCategoryId { get; set; }
        public virtual SurveyCategoryResponse SurveyCategory { get; set; }
        public string SurveyCategoryText
        {
            get
            {
                if (SurveyCategory != null)
                    return SurveyCategory.Name;
                else
                    return "";
            }
        }
     
        public virtual ICollection<SurveyQuestionOptionsResponse> SurveySurveyQuestionOptions { get; set; }
        //public virtual ICollection<SurveyDto> Surveys { get; set; }
        // public UserResponse CreatorUser { get; set; }
        //public string CleanText
        //{
        //    get
        //    {
        //        //string result = Text.StripHTML();
        //        return result;
        //    }
        //}
        //public string ShortText
        //{
        //    get
        //    {
        //        string result = Text.StripHTML().HtmlDecode();

        //        if (Text.Length > maxLength && !UseNotMaxLength)
        //        {
        //            result = result.TruncateLongString(maxLength) + "...";
        //        }

        //        return result;
        //    }
        //}
    }
}
