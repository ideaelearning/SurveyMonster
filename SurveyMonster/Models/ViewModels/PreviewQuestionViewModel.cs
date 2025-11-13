namespace SurveyMonster.Models.ViewModels;

public class PreviewQuestionViewModel
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int Order { get; set; }
    public int QuestionTypeId { get; set; }
    public string QuestionTypeName { get; set; } = string.Empty;
    public List<OptionViewModel> Options { get; set; } = new();
}
