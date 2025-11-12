namespace SurveyMonster.Models.Response;

public class MySurveyResponse
{
public long Id { get; set; }
    public int UserId { get; set; }
    public long SurveyAssignmentId { get; set; }
    public int CompletionStatus { get; set; }
    public DateTime? CompletionTime { get; set; }
    public DateTime? LastEnterDate { get; set; }
    public double? CompletionPercentage { get; set; }
    public decimal Point { get; set; }
    public bool? IsAutoAssign { get; set; }
    public DateTime CreationTime { get; set; }
 public int EntryCount { get; set; }
    public int SurveyEntryCompletedCount { get; set; }
    public SurveyAssignmentsResponse? SurveyAssignment { get; set; }
}
