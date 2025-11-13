namespace SurveyMonster.Models.Requests;

public class CreateSurveyAssignmentTakerRequest
{
    public long SurveyAssignmentId { get; set; }
    public long? UserId { get; set; }
    public string? AnonymousId { get; set; }
    public string? AnonymousAgent{ get; set; }
    public string? UserInformations { get; set; }
    public bool? IsAnonymous { get; set; }

    public string Discriminator { get; set; } = "SurveyAssignmentTaker";
}
