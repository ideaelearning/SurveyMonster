namespace SurveyMonster.Helpers;

public static class ErrorMessages
{
    // Anonymous User Form Errors
    public const string RequiredField = "Bu alan zorunludur";
    public const string InvalidEmail = "Geçerli bir e-posta adresi girin";
    public const string AnonymousInfoRequired = "Lütfen tüm alanları doldurun.";
    public const string AnonymousInfoInvalid = "Lütfen geçerli bilgiler girin.";
    
    // Question Answering Errors
    public const string AnswerRequired = "Lütfen soruyu cevaplayın";
    public const string NoAnswersFound = "Hiç cevap bulunamadı. Lütfen soruları cevaplayın.";
    public const string AnswerSaveFailed = "Cevap kaydedilirken bir hata oluştu.";
    
    // Session Errors
    public const string SessionExpired = "Oturumunuz sona erdi. Lütfen tekrar başlayın.";
    public const string SessionTimeout = "Oturumunuz zaman aşımına uğradı. Lütfen anketi yeniden başlatın.";
    public const string UserInfoNotFound = "Kullanıcı bilgisi bulunamadı. Lütfen tekrar giriş yapın.";
    
    // Survey Errors
    public const string SurveyNotFound = "Anket bulunamadı.";
    public const string SurveyLoadError = "Anket yüklenirken bir hata oluştu.";
    public const string SurveyStartError = "Anket başlatılırken bir hata oluştu.";
    public const string SurveySubmitError = "Anket gönderilirken bir hata oluştu.";
    public const string SurveyAssignmentCreateError = "Anket ataması oluşturulamadı.";
    public const string SurveyTakerCreateError = "Anket katılımcısı oluşturulamadı.";
    public const string SurveyEntryCreateError = "Anket girişi oluşturulamadı.";
    
    // Preview Errors
    public const string PreviewLoadError = "Önizleme yüklenirken bir hata oluştu.";
    
    // API Errors
    public const string ApiConnectionError = "Sunucu ile bağlantı kurulamadı. Lütfen internet bağlantınızı kontrol edin.";
    public const string ApiRequestError = "İstek işlenirken bir hata oluştu. Lütfen tekrar deneyin.";
    
    // General Errors
    public const string UnexpectedError = "Beklenmeyen bir hata oluştu. Lütfen tekrar deneyin.";
    public const string DataProcessingError = "Veri işleme hatası oluştu.";
    
    // Permission Errors
    public const string AnonymousUserRestriction = "Anonim kullanıcılar bu işlemi gerçekleştiremez.";
}
