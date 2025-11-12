using Lms.Survey.Application.Dto.Survey.Response;
using Lms.Survey.Application.Dto.SurveyQuestion.Response;
using Lms.Survey.Application.Dto.SurveyQuestionBank.Response;
using Lms.Survey.Application.Dto.SurveyType.Response;
using SurveyMonster.Models.Response;

namespace SurveyMonster.Models.DTOs;

public class SurveyDetailDto
{
    public int? Id { get; set; }
    public int? SurveyTypeId { get; set; }
    public int? SurveyCategoryId { get; set; }
    public virtual int? SurveyQuestionBankId { get; set; }
    public virtual string Name { get; set; } = string.Empty;
    public virtual string InformationText { get; set; } = string.Empty;
    public virtual bool ShowChapters { get; set; } = false;
    public virtual bool IsTeamplate { get; set; } = false;
    public virtual bool IsStatic { get; set; }

    public virtual SurveyTypeResponse? SurveyType { get; set; }
    public virtual SurveyQuestionBankResponse? SurveyQuestionBank { get; set; }

    public virtual ICollection<SurveyQuestionResponse>? SurveyQuestions { get; set; }
    public virtual ICollection<SurveyChaptersResponse>? SurveyChapters { get; set; }
    public virtual ICollection<SurveyQuestionOrdersResponse>? SurveySurveyQuestionOrders { get; set; }
    public virtual ICollection<SurveyAssignmentsResponse>? SurveyAssignments { get; set; }
    public virtual DateTime ExpireDate { get; set; }
}
