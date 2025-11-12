namespace SurveyMonster.Models.ViewModels;

public class SurveyReportViewModel
{
    public long AssignmentId { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public int SurveyId { get; set; }
     public string SurveyName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ReportAnswerViewModel> Answers { get; set; } = new();
    public Dictionary<string, List<ReportAnswerViewModel>> GroupedByQuestion { get; set; } = new();
}

public class ReportAnswerViewModel
{
    public long AnswerId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public int QuestionOrder { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public int CompletionStatus { get; set; }
}
