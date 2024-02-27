

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Request
{
    public class AdministrativeUnitFilterDtoRequest
    {
        public int? DocumentalVersionId { get; set; }
        public string? Code { get; set; }
        public int? BossId { get; set; }
        public string? Name { get; set; }
    }
}