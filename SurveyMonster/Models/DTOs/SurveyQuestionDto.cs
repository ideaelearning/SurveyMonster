namespace SurveyMonster.Models.DTOs;

public class SurveyQuestionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<SurveyQuestionOptionDto> SurveySurveyQuestionOptions { get; set; } = new();
}
