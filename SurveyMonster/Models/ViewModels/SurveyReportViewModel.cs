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
    public Dictionary<string, Dictionary<string, List<ReportAnswerViewModel>>> GroupedByQuestionAndUser { get; set; } = new();
}

public class ReportAnswerViewModel
{
    public long AnswerId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string PersonnelNo { get; set; } = string.Empty;
    public int QuestionOrder { get; set; }
    public long SurveyQuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public long QuestionTypeId { get; set; }
    public string QuestionTypeName { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string AnswerOrj { get; set; } = string.Empty;
    public string AnswerOther { get; set; } = string.Empty;
    public long? EntryId { get; set; }
    public DateTime EntryDate { get; set; }
    public DateTime? EntryFinishDate { get; set; }
    public int CompletionStatus { get; set; }
}
