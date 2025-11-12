namespace Lms.Survey.Application.Dto.Survey.Response
{
    public class SurveyAssignmentTakersResponse
    {
        public int? Id { get; set; }
        public int SurveyAssignmentId { get; set; }
        public int? SurveyAssignmentTrainingParticipantId { get; set; }
        //public SurveyAssignmentsResponse SurveyAssignment { get; set; }
        public List<SurveyAssignmentTakerEntriesResponse> Entries { get; set; }

        /// <summary>
        /// RESPONSE EKLENECEK!!!!
        /// </summary>
        //public virtual CoreEventAssignment SurveyAssignmentTrainingParticipant { get; set; }
    }
}
