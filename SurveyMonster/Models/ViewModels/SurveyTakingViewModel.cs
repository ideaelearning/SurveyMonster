namespace SurveyMonster.Models.ViewModels;

public class SurveyTakingViewModel
{
    public int SurveyId { get; set; }
    public string SurveyName { get; set; } = string.Empty;
    public int EntryId { get; set; }
    public List<QuestionViewModel> Questions { get; set; } = new();
}
