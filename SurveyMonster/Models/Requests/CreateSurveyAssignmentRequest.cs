namespace SurveyMonster.Models.Requests;

public class CreateSurveyAssignmentRequest
{
    public int SurveyId { get; set; }
    public string Discriminator { get; set; } = "SurveyAssignment";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int EventCategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool Imperative { get; set; }
    public int SurveyMaxTakeCount { get; set; }
    public int ExamSecurityType { get; set; }
    public bool IsAnonymous { get; set; }
}
