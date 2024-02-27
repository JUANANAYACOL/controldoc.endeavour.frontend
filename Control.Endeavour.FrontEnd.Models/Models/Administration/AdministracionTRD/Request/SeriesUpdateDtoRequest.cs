

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class SeriesUpdateDtoRequest
    {
        public int SerieId { get; set; }
        public int ProductionOfficeId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }

        public bool ActiveState { get; set; }
        public string? User { get; set; }
    }
}
