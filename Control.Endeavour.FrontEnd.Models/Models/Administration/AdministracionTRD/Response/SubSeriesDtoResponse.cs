

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{
    public class SubSeriesDtoResponse
    {
        public int SubSeriesId { get; set; }

        public int SeriesId { get; set; }

        public string? SeriesName { get; set; }

        public string? Code { get; set; }

        public string? Name { get; set; } 

        public string? Description { get; set; } 

        public bool ActiveState { get; set; }
        public string? CreateUser { get; set; } 
    }
}
