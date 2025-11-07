namespace SurveyMonster.Models.DTOs;

public class SurveyQuestionOptionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Value { get; set; }
}
