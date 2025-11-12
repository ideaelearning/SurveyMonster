using SurveyMonster.Models.DTOs;

namespace SurveyMonster.Models.ViewModels;

public class QuestionViewModel
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<OptionViewModel> Options { get; set; } = new();
}
