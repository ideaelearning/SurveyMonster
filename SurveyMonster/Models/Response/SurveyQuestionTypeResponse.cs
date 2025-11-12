namespace Lms.Survey.Application.Dto.SurveyQuestionType.Response
{
    public class SurveyQuestionTypeResponse
    {
        public int? Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}
