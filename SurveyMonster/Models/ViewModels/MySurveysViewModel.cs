namespace SurveyMonster.Models.ViewModels;

public class MySurveysViewModel
{
    public List<MySurveyItemViewModel> Surveys { get; set; } = new();
}

public class MySurveyItemViewModel
{
    public long Id { get; set; }
    public long SurveyAssignmentId { get; set; }
    public int SurveyId { get; set; }
    public string SurveyName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string InformationText { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CompletionStatus { get; set; }
    public double? CompletionPercentage { get; set; }
    public int EntryCount { get; set; }
    public int SurveyEntryCompletedCount { get; set; }
    public int MaxTakeCount { get; set; }
    public bool IsExpired { get; set; }
    public bool IsActive { get; set; }
    public bool IsCompleted => CompletionStatus == 2;
    public bool CanTake => !IsExpired && IsActive && (MaxTakeCount == 0 || EntryCount < MaxTakeCount);
}
