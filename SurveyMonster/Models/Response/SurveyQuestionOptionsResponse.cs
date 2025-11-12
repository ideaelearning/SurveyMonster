namespace Lms.Survey.Application.Dto.Survey.Response
{
    public class SurveyQuestionOptionsResponse
    {
        public int? Id { get; set; }
        public int? TenantId { get; set; }
        public int SurveyQuestionId { get; set; }
        public string Text { get; set; }
        public int Value { get; set; }
        public bool IsOther { get; set; }
        public int? VerificationTypeId { get; set; }
        //public SurveyQuestionResponse SurveyQuestion { get; set; }
        // Add other properties as needed
        public virtual SurveyVerificationTypeResponse VerificationType { get; set; }
        public int NumberOfSelection { get; set; }
        public string OptionColor { get; set; }
        public int EmptyOfSelection
        {
            get
            {
                return NumberOfSelection - NumberOfSelection;
            }
        }
    }
}
