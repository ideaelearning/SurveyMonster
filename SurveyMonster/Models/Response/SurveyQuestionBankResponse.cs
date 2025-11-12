using Lms.Survey.Application.Dto.SurveyQuestionCategory.Response;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lms.Survey.Application.Dto.SurveyQuestionBank.Response
{
    public class SurveyQuestionBankResponse
    {
        public const int maxLength = 140;
        public bool CopyOtherQuestionBank { get; set; }
        public long? OtherQuestionBankId { get; set; }
        public bool CopyDefaultCategories { get; set; }
        public bool CopyFromOtherQuestionBanksCategories { get; set; }
        public bool CopyFromOtherQuestionBanksQuestions { get; set; }
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }
        //public string ShortDescription
        //{
        //    get
        //    {
        //        if (Description == null)
        //            return "";

        //        string result = Description.StripHTML();

        //        if (Description.Length > maxLength)
        //        {
        //            result = result.TruncateLongString(maxLength) + "...";
        //        }

        //        return result;
        //    }
        //}

        public virtual DateTime ExpireDate { get; set; }
        public virtual DateTime CreationTime { get; set; }
        public DateTime CreationTimeUserTime
        {
            get
            {
                //if (UITimeZone != null)
                //{
                //    return CreationTime.ToUserTime(UITimeZone);
                //}
                return DateTime.UtcNow;
            }
        }

        public virtual int? CopiedSurveyQuestionBankId { get; set; }

        //public virtual SurveyQuestionBankDto CopiedSurveyQuestionBank { get; set; }

        [NotMapped]
        public virtual List<SurveyQuestionCategoryResponse> QuestionBankCategories { get; set; }
        //[NotMapped]
        //public virtual List<SurveyQuestionCategoryDto> QuestionBankQuestions { get; set; }
        //public virtual ICollection<SurveyDto> Surveys { get; set; }
        public virtual int? TenantId { get; set; }
        //public virtual TimeZoneInfo UITimeZone { get; set; }
    }
}