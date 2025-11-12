namespace Lms.Survey.Application.Dto.SurveyType.Response
{
    public class SurveyTypeResponse
    {
        public long? Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        //public virtual ideaLEnviromentConsts.State State { get; set; }
        public bool isDefault { get; set; }
        public bool isStatic { get; set; }

        public int? TenantId { get; set; }

         public int State { get; set; }

    }
}
