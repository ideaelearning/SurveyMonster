
namespace Lms.Survey.Application.Dto.SurveyCategory.Response
{
    public class SurveyCategoryResponse
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool isDefault { get; set; }
        public bool isStatic { get; set; }
    }
}
