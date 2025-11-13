namespace SurveyMonster.Models.Response
{
    public class RequiredUserInformationsResponse
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Name{ get; set; }
        public string? Surname{ get; set; }

    }
    public class RequiredUserInformationsRequest
    {
        public bool? Email { get; set; }
        public bool? Phone { get; set; }
        public bool? Name { get; set; }
        public bool? Surname { get; set; }

    }
}
