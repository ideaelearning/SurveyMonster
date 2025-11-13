# Implementation Plan

- [x] 1. Yeni model ve view model sınıflarını oluştur





  - AnonymousUserInfo, SurveyPreviewViewModel, QuestionNavigationViewModel ve ilgili sınıfları ekle
  - QuestionViewModel'e QuestionTypeId ve QuestionTypeName property'lerini ekle
  - SurveyInfoViewModel'e RequiresAnonymousInfo property'sini ekle
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8, 4.1, 4.2, 4.3, 4.4, 4.5, 4.6, 4.7, 4.8, 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7_

- [x] 2. Ön bilgilendirme ve onay sistemi




  - [x] 2.1 Index.cshtml view'ını güncelle


    - InformationText bölümünü yeniden düzenle
    - Consent checkbox ekle
    - "Ankete Başla" butonunu başlangıçta disabled yap
    - _Requirements: 1.1, 1.2, 1.3_
  
  - [x] 2.2 JavaScript ile checkbox-button etkileşimi ekle


    - Checkbox change event listener ekle
    - Checkbox işaretlendiğinde butonu aktif et
    - _Requirements: 1.4, 1.5_

- [x] 3. Anket önizleme özelliği




  - [x] 3.1 SurveyController'a PreviewSurvey action'ı ekle


    - Survey verilerini çek
    - SurveyPreviewViewModel oluştur
    - QuestionTypeId ve QuestionTypeName bilgilerini dahil et
    - _Requirements: 2.2, 2.4, 2.5_
  
  - [x] 3.2 _PreviewModal.cshtml partial view oluştur


    - Modal yapısını oluştur
    - Soruları sıralı şekilde göster
    - QuestionType'a göre farklı görünümler (radio, checkbox, textarea) göster
    - Kapat butonu ekle
    - _Requirements: 2.3, 2.4, 2.5, 2.6_
  
  - [x] 3.3 Index.cshtml'e "Önizleme" butonu ekle


    - Butonu bilgilendirme bölümüne ekle
    - Modal açma JavaScript kodu ekle
    - _Requirements: 2.1, 2.2_

- [x] 4. Anonim kullanıcı bilgi toplama




  - [x] 4.1 Index.cshtml'e anonim kullanıcı formu ekle


    - Ad, Soyad, Email input alanları ekle
    - Required ve email validation ekle
    - Sadece anonim kullanıcılara göster
    - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_
  
  - [x] 4.2 SurveyController.StartSurvey action'ını güncelle


    - AnonymousUserInfo parametresi ekle
    - Model validation kontrolü ekle
    - JSON'a çevirip session'a kaydet
    - _Requirements: 3.6, 3.7, 3.8_
  
  - [x] 4.3 SurveyController.SubmitSurvey action'ını güncelle


    - Session'dan anonim kullanıcı bilgisini al
    - Özel bir answer olarak kaydet (questionId = 0)
    - Session'ı temizle
    - _Requirements: 3.7, 3.8_


- [x] 5. Soru bazlı navigasyon sistemi



  - [x] 5.1 SurveyController.TakeSurvey action'ını güncelle


    - questionIndex parametresi ekle
    - Tek soru gösterecek şekilde view model oluştur
    - Session'dan kaydedilmiş cevapları al
    - QuestionTypeId bilgisini view model'e ekle
    - _Requirements: 4.1, 4.2, 4.8_
  
  - [x] 5.2 SurveyController'a SaveAnswer action'ı ekle


    - Cevabı session'a kaydet
    - Sonraki soru index'ine yönlendir
    - _Requirements: 4.4, 4.5, 4.8_
  
  - [x] 5.3 TakeSurvey.cshtml view'ını yeniden tasarla


    - Tek soru gösterecek şekilde düzenle
    - Progress indicator ekle (Soru X / Y)
    - İleri/Geri butonları ekle
    - Son soruda "Anketi Tamamla" butonu göster
    - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5, 4.6, 4.7_

- [x] 6. Farklı soru tiplerini destekleme





  - [x] 6.1 TakeSurvey.cshtml'de soru tipi kontrolü ekle


    - QuestionTypeId == 1 için radio button render et
    - QuestionTypeId == 2 için checkbox render et
    - QuestionTypeId == 3 için textarea render et
    - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7_
  
  - [x] 6.2 Çoklu seçim cevaplarını işle


    - JavaScript ile checkbox'ları topla
    - Noktalı virgülle birleştir (11814;11815;11816)
    - Form submit'te gönder
    - _Requirements: 6.1, 6.2, 6.3, 6.4_
  

  - [x] 6.3 Açık uçlu soru cevaplarını işle

    - Textarea değerini al
    - Boş cevap kontrolü yap
    - IsEmpty flag'ini ayarla
    - _Requirements: 7.1, 7.2, 7.3, 7.4_

- [x] 7. Cevap kaydetme ve anket tamamlama





  - [x] 7.1 SurveyController.SubmitSurvey action'ını güncelle


    - Session'dan tüm cevapları al
    - Her cevap için SaveAnswerAsync çağır
    - Anonim kullanıcı bilgisini kaydet
    - FinishSurveyEntryAsync çağır
    - Session'ı temizle
    - _Requirements: 6.1, 6.2, 6.3, 6.4, 7.1, 7.2, 7.3, 7.4_
  
  - [x] 7.2 JavaScript ile form submit validasyonu ekle


    - Tüm soruların cevaplanıp cevaplanmadığını kontrol et
    - Eksik cevap varsa uyarı göster
    - Onay mesajı göster
    - _Requirements: 4.8, 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7_

- [x] 8. Hata yönetimi ve kullanıcı geri bildirimi





  - [x] 8.1 Validation error mesajları ekle


    - Anonim kullanıcı formu için error mesajları
    - Soru cevaplama için error mesajları
    - Session timeout için error mesajları
    - _Requirements: 3.2, 3.3, 3.4, 3.5_
  
  - [x] 8.2 Try-catch blokları ekle


    - JSON parsing errors
    - API request errors
    - Session errors
    - _Requirements: Tüm requirements için genel hata yönetimi_

- [x] 9. UI/UX iyileştirmeleri ve styling





  - [x] 9.1 CSS stilleri ekle


    - Consent checkbox styling
    - Preview modal styling
    - Anonim kullanıcı formu styling
    - Soru navigasyon styling
    - Soru tipi bazlı styling (radio, checkbox, textarea)
    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 3.1, 3.2, 3.3, 3.4, 3.5, 4.1, 4.2, 4.3, 4.4, 4.5, 4.6, 4.7, 4.8_
  
  - [x] 9.2 Animasyon ve transition efektleri ekle


    - Soru geçişleri için animasyon
    - Button hover efektleri
    - Progress bar animasyonu
    - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5, 4.6, 4.7_

- [x] 10. Test ve doğrulama





  - [x] 10.1 Manuel test senaryolarını çalıştır


    - Ön bilgilendirme ve onay akışını test et
    - Önizleme özelliğini test et
    - Anonim kullanıcı bilgi toplama test et
    - Soru navigasyonunu test et
    - Farklı soru tiplerini test et
    - Cevap kaydetme ve tamamlama test et
    - _Requirements: Tüm requirements_
  
  - [x] 10.2 Hata senaryolarını test et


    - Boş form gönderimi
    - Geçersiz email
    - Session timeout
    - API hataları
    - _Requirements: Hata yönetimi requirements_
  
  - [x] 10.3 Tarayıcı uyumluluğu test et


    - Chrome, Firefox, Safari, Edge
    - Responsive design kontrolü
    - JavaScript compatibility
    - _Requirements: Tüm UI requirements_

- [ ] 11. Anket raporu iyileştirmeleri

  - [ ] 11.1 SurveyReportViewModel ve ReportAnswerViewModel'i güncelle
    - QuestionTypeId, QuestionTypeName property'lerini ekle
    - AnswerOrj, AnswerOther property'lerini ekle
    - PersonnelNo property'sini ekle
    - EntryId, EntryFinishDate property'lerini ekle
    - _Requirements: 8.1, 8.2_

  - [ ] 11.2 SurveyController.SurveyReport action'ını güncelle
    - Servisten gelen tüm verileri view model'e map et
    - QuestionTypeId ve QuestionTypeName bilgilerini dahil et
    - AnswerOrj ve AnswerOther alanlarını işle
    - _Requirements: 8.1, 8.2, 8.3_

  - [ ] 11.3 SurveyReport.cshtml view'ını güncelle
    - QuestionTypeId'ye göre farklı görüntüleme mantığı ekle
    - MultipleChoiceMultipleSelection için liste görünümü ekle
    - OpenEndedQuestion için metin görünümü iyileştir
    - AnswerOther alanını "Diğer" seçeneği için göster
    - _Requirements: 8.3, 8.4, 8.5, 8.6, 8.7, 8.8, 8.9, 8.10_
