using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SurveyMonster.Models.Response;

public class SurveyAssignmentsResponse
{
    public long? Id { get; set; }
    public string? Title { get; set; }
    public string? EventCode { get; set; }
    public string? Details { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? SurveyMaxTakeCount { get; set; }
    public bool? Imperative { get; set; }
    public int? DefaultGamificationPoint { get; set; }
    public string? RequiredUserInformations { get; set; }
    public SimpleSurveyResponse? Survey { get; set; }


    public RequiredUserInformationsRequest RequiredUserInformationList
    {
        get
        {
            if (!string.IsNullOrEmpty(RequiredUserInformations))
            {
                return JsonConvert.DeserializeObject<RequiredUserInformationsRequest>(RequiredUserInformations);
            }
            return null;
        }
    }
    public bool CanTakeSurvey(int entryCount)
    {
        if (StartDate.HasValue &&
 EndDate.HasValue &&
       StartDate.Value < DateTime.UtcNow &&
          EndDate.Value > DateTime.UtcNow &&
  entryCount < (SurveyMaxTakeCount ?? 1))
      {
            return true;
        }
   return false;
    }

 public bool IsExpired => EndDate.HasValue && EndDate.Value < DateTime.UtcNow;

    public bool IsActive => StartDate.HasValue &&
        EndDate.HasValue &&
            StartDate.Value <= DateTime.UtcNow &&
           EndDate.Value >= DateTime.UtcNow;
}
