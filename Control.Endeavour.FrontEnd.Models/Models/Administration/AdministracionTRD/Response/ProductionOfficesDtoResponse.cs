namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{
    public class ProductionOfficesDtoResponse
    {
        public int ProductionOfficeId { get; set; }

        public int AdministrativeUnitId { get; set; }

        public string? AdministrativeUnitName { get; set; }

        public int? BossId { get; set; }

        public string? BossName { get; set; }

        public string Code { get; set; } = null!;

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool ActiveState { get; set; }
    }
}