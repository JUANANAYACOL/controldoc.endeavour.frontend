﻿using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Response
{
    public class BranchOfficesDtoResponse
    {
        public int BranchOfficeId { get; set; }
        public int? AddressId { get; set; }
        public string AddressString { get; set; }
        public string Code { get; set; }
        public string NameOffice { get; set; }
        public string Region { get; set; }
        public string Territory { get; set; }

        public AddressDtoResponse Address { get; set; }
    }
}