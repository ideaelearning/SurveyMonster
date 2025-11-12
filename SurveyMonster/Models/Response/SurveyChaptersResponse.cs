namespace Lms.Survey.Application.Dto.Survey.Response
{
    public class SurveyChaptersResponse
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SurveyId { get; set; }
        public int SurveyChapterOrder { get; set; }
        public bool IsNameHidden { get; set; }
        public bool IsStatistical { get; set; }
        public int? TenantId { get; set; }
        // Add other properties as needed
    }
}
