namespace SurveyMonster.Models.Requests;

public class SaveAnswerRequest
{
    public int SurveyAssignmentTakerEntryId { get; set; }
    public int SurveyQuestionId { get; set; }
    public string Answer { get; set; } = string.Empty;
    public bool IsEmpty { get; set; }
    public bool IsAnonymous { get; set; } = true;

    public int TenantId { get; set; }
}
