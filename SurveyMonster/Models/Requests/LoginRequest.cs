using System.ComponentModel.DataAnnotations;

namespace SurveyMonster.Models.Requests;

public class LoginRequest
{
    [Required(ErrorMessage = "Kullan?c? ad? gereklidir")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "?ifre gereklidir")]
    public string Password { get; set; } = string.Empty;

    public string Language { get; set; } = "tr";
}
