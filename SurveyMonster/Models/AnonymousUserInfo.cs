using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace SurveyMonster.Models;

public class AnonymousUserInfo
{
    [Required(ErrorMessage = "Ad alanı zorunludur")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad alanı zorunludur")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta alanı zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin")]
    public string Email { get; set; } = string.Empty;

    public string ToJson()
    {
        try
        {
            return JsonSerializer.Serialize(this);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Kullanıcı bilgileri JSON formatına dönüştürülemedi.", ex);
        }
    }

    public static AnonymousUserInfo? FromJson(string json)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            return JsonSerializer.Deserialize<AnonymousUserInfo>(json);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("JSON verisi kullanıcı bilgilerine dönüştürülemedi.", ex);
        }
    }
}
