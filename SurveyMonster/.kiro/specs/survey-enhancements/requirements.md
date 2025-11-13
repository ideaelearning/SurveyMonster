# Requirements Document

## Introduction

Bu doküman, SurveyMonster anket uygulamasına eklenecek yeni özellikleri tanımlar. Kullanıcı deneyimini iyileştirmek için ön bilgilendirme, önizleme, anonim kullanıcı bilgi toplama, soru bazlı navigasyon ve farklı soru tiplerini destekleme özellikleri eklenecektir.

## Glossary

- **System**: SurveyMonster anket uygulaması
- **User**: Ankete katılan son kullanıcı
- **Survey**: Anket
- **Question**: Anket sorusu
- **InformationText**: Anket başlamadan önce gösterilen bilgilendirme metni
- **Preview**: Anketin son kullanıcı görünümünde önizlemesi
- **Anonymous User**: Sisteme giriş yapmadan ankete katılan kullanıcı
- **QuestionType**: Soru tipi (çoktan seçmeli tek seçim, çoktan seçmeli çoklu seçim, açık uçlu)
- **MultipleChoiceOneSelection**: Çoktan seçmeli tek seçim soru tipi (Id=1)
- **MultipleChoiceMultipleSelection**: Çoktan seçmeli çoklu seçim soru tipi (Id=2)
- **OpenEndedQuestion**: Açık uçlu soru tipi (Id=3)
- **Answer**: Kullanıcının verdiği cevap
- **Consent Checkbox**: Kullanıcının bilgilendirme metnini okuduğunu onaylayan checkbox

## Requirements

### Requirement 1: Ön Bilgilendirme ve Onay

**User Story:** Kullanıcı olarak, ankete başlamadan önce anket hakkında bilgilendirilmek ve bu bilgilendirmeyi onaylamak istiyorum, böylece anketin amacını ve koşullarını anlayabilirim.

#### Acceptance Criteria

1. WHEN kullanıcı anket sayfasına eriştiğinde, THE System SHALL InformationText içeriğini görüntüleyecektir
2. THE System SHALL InformationText altında bir Consent Checkbox gösterecektir
3. WHILE Consent Checkbox işaretli değilken, THE System SHALL "Ankete Başla" butonunu devre dışı bırakacaktır
4. WHEN kullanıcı Consent Checkbox'ı işaretlediğinde, THE System SHALL "Ankete Başla" butonunu aktif hale getirecektir
5. WHEN kullanıcı aktif "Ankete Başla" butonuna tıkladığında, THE System SHALL kullanıcıyı anket sorularına yönlendirecektir

### Requirement 2: Anket Önizleme

**User Story:** Kullanıcı olarak, ankete başlamadan önce soruları önizlemek istiyorum, böylece anketin ne kadar süreceğini ve hangi soruları içerdiğini görebilirim.

#### Acceptance Criteria

1. THE System SHALL anket bilgilendirme sayfasında bir "Önizleme" butonu gösterecektir
2. WHEN kullanıcı "Önizleme" butonuna tıkladığında, THE System SHALL bir modal pencere açacaktır
3. THE System SHALL modal pencerede tüm anket sorularını sıralı şekilde gösterecektir
4. THE System SHALL modal pencerede her sorunun QuestionType'ına göre uygun görünümü (çoktan seçmeli veya metin girişi) gösterecektir
5. THE System SHALL modal pencerede soruların cevap seçeneklerini gösterecektir ancak cevap girişine izin vermeyecektir
6. THE System SHALL modal pencerede bir "Kapat" butonu sağlayacaktır

### Requirement 3: Anonim Kullanıcı Bilgi Toplama

**User Story:** Anonim kullanıcı olarak, ankete katılmadan önce ad, soyad ve email bilgilerimi girmek istiyorum, böylece kimliğim doğrulanabilir.

#### Acceptance Criteria

1. WHEN anonim bir kullanıcı ankete başladığında, THE System SHALL ad, soyad ve email alanlarını içeren bir form gösterecektir
2. THE System SHALL ad alanının doldurulmasını zorunlu kılacaktır
3. THE System SHALL soyad alanının doldurulmasını zorunlu kılacaktır
4. THE System SHALL email alanının doldurulmasını zorunlu kılacaktır
5. THE System SHALL email alanının geçerli bir email formatında olmasını doğrulayacaktır
6. WHEN kullanıcı formu doldurduğunda, THE System SHALL bu bilgileri JSON formatına dönüştürecektir
7. THE System SHALL JSON formatındaki bilgileri tek bir string olarak Answer alanında saklayacaktır
8. THE System SHALL kullanıcı bilgilerini topladıktan sonra anket sorularına geçişe izin verecektir

### Requirement 4: Soru Bazlı Navigasyon

**User Story:** Kullanıcı olarak, anket sorularını tek tek görmek ve her seferinde bir soruya cevap vermek istiyorum, böylece odaklanabilir ve daha iyi cevaplar verebilirim.

#### Acceptance Criteria

1. THE System SHALL her seferinde yalnızca bir Question gösterecektir
2. THE System SHALL mevcut Question'ın sıra numarasını ve toplam soru sayısını gösterecektir
3. THE System SHALL kullanıcının cevap vermesi için uygun input kontrollerini gösterecektir
4. THE System SHALL bir "İleri" veya "Sonraki" butonu sağlayacaktir
5. WHEN kullanıcı "İleri" butonuna tıkladığında, THE System SHALL bir sonraki Question'a geçiş yapacaktır
6. WHEN kullanıcı son Question'a cevap verdiğinde, THE System SHALL "Anketi Tamamla" butonu gösterecektir
7. THE System SHALL kullanıcının önceki sorulara dönmesi için bir "Geri" butonu sağlayacaktır
8. THE System SHALL kullanıcının verdiği cevapları geçici olarak saklayacaktır

### Requirement 5: Farklı Soru Tiplerini Destekleme

**User Story:** Kullanıcı olarak, farklı tiplerdeki sorulara uygun şekilde cevap verebilmek istiyorum, böylece her soru tipine en uygun şekilde yanıt verebilirim.

#### Acceptance Criteria

1. THE System SHALL her Question için QuestionType değerini kontrol edecektir
2. WHEN QuestionType MultipleChoiceOneSelection (Id=1) olduğunda, THE System SHALL radio button kontrollerini gösterecektir
3. WHEN QuestionType MultipleChoiceMultipleSelection (Id=2) olduğunda, THE System SHALL checkbox kontrollerini gösterecektir
4. WHEN QuestionType OpenEndedQuestion (Id=3) olduğunda, THE System SHALL textarea kontrolünü gösterecektir
5. THE System SHALL MultipleChoiceOneSelection sorularında yalnızca bir seçeneğin seçilmesine izin verecektir
6. THE System SHALL MultipleChoiceMultipleSelection sorularında birden fazla seçeneğin seçilmesine izin verecektir
7. THE System SHALL OpenEndedQuestion sorularında serbest metin girişine izin verecektir

### Requirement 6: Çoklu Seçim Cevaplarının İşlenmesi

**User Story:** Kullanıcı olarak, çoklu seçim sorularında birden fazla seçenek seçebilmek istiyorum ve bu seçeneklerin doğru şekilde kaydedilmesini istiyorum.

#### Acceptance Criteria

1. WHEN kullanıcı MultipleChoiceMultipleSelection sorusunda birden fazla seçenek seçtiğinde, THE System SHALL seçilen tüm option ID'lerini toplayacaktır
2. THE System SHALL seçilen option ID'lerini noktalı virgül (;) ile ayırarak birleştirecektir
3. THE System SHALL birleştirilmiş string'i Answer alanına kaydedecektir
4. THE System SHALL Answer formatının "11814;11815;11816" şeklinde olmasını sağlayacaktır

### Requirement 7: Açık Uçlu Soru Cevaplarının İşlenmesi

**User Story:** Kullanıcı olarak, açık uçlu sorulara serbest metin cevabı verebilmek istiyorum ve bu cevabın doğru şekilde kaydedilmesini istiyorum.

#### Acceptance Criteria

1. WHEN kullanıcı OpenEndedQuestion sorusuna metin girdiğinde, THE System SHALL girilen metni alacaktır
2. THE System SHALL girilen metni Answer alanına kaydedecektir
3. THE System SHALL boş cevaplara izin verecektir ancak IsEmpty flag'ini true olarak işaretleyecektir
4. THE System SHALL özel karakterleri ve satır sonlarını koruyarak metni kaydedecektir

### Requirement 8: Anket Raporu Görüntüleme

**User Story:** Yönetici olarak, tamamlanmış anketlerin detaylı raporunu görmek istiyorum, böylece kullanıcı cevaplarını analiz edebilir ve soru tiplerine göre uygun şekilde görüntüleyebilirim.

#### Acceptance Criteria

1. THE System SHALL SurveyReportViewModel'de tüm servis verilerini karşılayacak property'leri içerecektir
2. THE System SHALL ReportAnswerViewModel'de QuestionTypeId, QuestionTypeName, AnswerOrj, AnswerOther, PersonnelNo, EntryId ve EntryFinishDate alanlarını içerecektir
3. WHEN rapor görüntülendiğinde, THE System SHALL her soruyu QuestionOrder'a göre sıralı şekilde gösterecektir
4. WHEN MultipleChoiceMultipleSelection sorusu görüntülendiğinde, THE System SHALL aynı kullanıcının birden fazla seçeneğini liste halinde gösterecektir
5. WHEN OpenEndedQuestion sorusu görüntülendiğinde, THE System SHALL kullanıcının girdiği metni tam olarak gösterecektir
6. THE System SHALL her soru için cevap dağılımını görsel olarak (bar chart) gösterecektir
7. THE System SHALL her kullanıcının cevaplarını detaylı şekilde gösterecektir
8. THE System SHALL raporda kullanıcı bilgilerini (ad, soyad, email, personel no) gösterecektir
9. THE System SHALL raporda anket istatistiklerini (toplam katılımcı, soru sayısı, toplam cevap) gösterecektir
10. THE System SHALL AnswerOther alanını "Diğer" seçeneği için ayrı olarak gösterecektir
