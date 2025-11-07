namespace SurveyMonster.Models.DTOs;

public class SurveyDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InformationText { get; set; } = string.Empty;
    public DateTime ExpireDate { get; set; }
    public List<SurveyQuestionOrderDto> SurveySurveyQuestionOrders { get; set; } = new();
}
