namespace Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffices
{
    public class BranchOfficeFilterDtoRequest
    {
        public int AddressId { get; set; }
        public string? Code { get; set; }
        public string? NameOffice { get; set; }
        public string? Region { get; set; }
        public string? Territory { get; set; }
    }
}