namespace SurveyMonster.Models.DTOs;

public class SurveyQuestionOrderDto
{
    public int Id { get; set; }
    public int Order { get; set; }
    public int SurveyQuestionId { get; set; }
    public SurveyQuestionDto SurveyQuestion { get; set; } = null!;
}
