namespace SurveyMonster.Models.ViewModels;

public class QuestionNavigationViewModel
{
    public int SurveyId { get; set; }
    public string SurveyName { get; set; } = string.Empty;
    public int EntryId { get; set; }
    public int CurrentQuestionIndex { get; set; }
    public int TotalQuestions { get; set; }
    public QuestionViewModel CurrentQuestion { get; set; } = new();
    public Dictionary<int, string> SavedAnswers { get; set; } = new();
}
