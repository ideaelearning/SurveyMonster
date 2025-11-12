namespace SurveyMonster.Models.Response;

public class SurveyReportResponse
{
    public long AsgId { get; set; }
    public string? AsgTitle { get; set; }
    public DateTime? AsgStartDate { get; set; }
    public DateTime? AsgEndDate { get; set; }
    public long SurveyId { get; set; }
    public string? SurveyName { get; set; }
    public long? AnswerId { get; set; }
    public string? AnswerOrj { get; set; }
    public string? Answer { get; set; }
    public string? AnswerOther { get; set; }
    public long UserId { get; set; }
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? EmailAddress { get; set; }
    public string? PersonnelNo { get; set; }
    public int CompletionStatus { get; set; }
    public long? EntryId { get; set; }
    public DateTime? EntryStartDate { get; set; }
    public DateTime? EntryFinishDate { get; set; }
    public long SurveyQuestionId { get; set; }
    public int QuestionOrder { get; set; }
    public long QuestionTypeId { get; set; }
    public string? QuestionTypeName { get; set; }
    public string? QuestionText { get; set; }
}
