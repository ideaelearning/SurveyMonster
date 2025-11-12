namespace SurveyMonster.Models.ViewModels;

public class SurveyInfoViewModel
{
    public int SurveyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InformationText { get; set; } = string.Empty;
    public DateTime ExpireDate { get; set; }
    public int QuestionCount { get; set; }
}
