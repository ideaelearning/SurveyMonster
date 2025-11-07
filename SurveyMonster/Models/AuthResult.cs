namespace SurveyMonster.Models;

public class AuthResult
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
