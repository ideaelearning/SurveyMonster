namespace SurveyMonster.Models.DTOs;

public class SurveyAssignmentDto
{
    public int Id { get; set; }
    public int SurveyId { get; set; }
    public int UserId { get; set; }
    public DateTime AssignedDate { get; set; }
    public SurveyDetailDto? Survey { get; set; }
}
