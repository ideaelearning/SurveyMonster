namespace SurveyMonster.Models.ViewModels;

public class OptionViewModel
{
    public int OptionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public int Value { get; set; }
}
