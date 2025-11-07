namespace SurveyMonster.Models.Requests;

public class CreateSurveyEntryRequest
{
    public int SurveyAssignmentTakerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public int SurveyState { get; set; }
}
