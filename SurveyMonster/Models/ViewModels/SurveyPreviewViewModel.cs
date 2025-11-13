namespace SurveyMonster.Models.ViewModels;

public class SurveyPreviewViewModel
{
    public int SurveyId { get; set; }
    public string SurveyName { get; set; } = string.Empty;
    public List<PreviewQuestionViewModel> Questions { get; set; } = new();
}
