namespace SurveyMonster.Models.Requests;

public class CreateSurveyAssignmentTakerRequest
{
    public int SurveyAssignmentId { get; set; }
    public int? UserId { get; set; }
    public string? AnonymousId { get; set; }
    public string? AnonymousAgent{ get; set; }
    public bool? IsAnonymous { get; set; }

    public string Discriminator { get; set; } = "SurveyAssignmentTaker";
}
