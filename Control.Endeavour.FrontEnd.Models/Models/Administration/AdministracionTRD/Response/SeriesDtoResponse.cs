namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{
    public class SeriesDtoResponse
    {
        public int SeriesId { get; set; }
        public int ProductionOfficeId { get; set; }
        public string? ProductionOfficeName { get; set; }
        public string? Name { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public bool ActiveState { get; set; }
        public string? CreateUser { get; set; }
    }
}