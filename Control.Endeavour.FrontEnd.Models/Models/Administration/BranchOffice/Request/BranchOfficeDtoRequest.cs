﻿using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Request
{
    public class BranchOfficeDtoRequest
    {
        public string Code { get; set; }
        public string NameOffice { get; set; }
        public string Region { get; set; }
        public string Territory { get; set; }
        public string User { get; set; }
        public AddressDtoRequest Address { get; set; }
    }
}