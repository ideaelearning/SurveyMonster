namespace Lms.Survey.Application.Dto.Survey.Response
{
    public class SurveyVerificationTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RegEx { get; set; }
        public int? TenantId { get; set; }
    }
}
