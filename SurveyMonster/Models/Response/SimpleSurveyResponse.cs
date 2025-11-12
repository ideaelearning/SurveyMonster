namespace SurveyMonster.Models.Response;

public class SimpleSurveyResponse
{
  public int? Id { get; set; }
 public string? Name { get; set; }
public string? InformationText { get; set; }
  public bool ShowChapters { get; set; }
    public int? State { get; set; }
    public DateTime ExpireDate { get; set; }
}
