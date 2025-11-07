using SurveyMonster.Models.DTOs;

namespace SurveyMonster.Models.ViewModels;

public class QuestionViewModel
{
    public int SurveyId { get; set; }
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public List<SurveyQuestionOptionDto> Options { get; set; } = new();
    public int CurrentQuestionIndex { get; set; }
    public int TotalQuestions { get; set; }
    public bool IsLastQuestion { get; set; }
}
