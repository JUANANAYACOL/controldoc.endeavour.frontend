﻿

using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdParty.Request
{
    public class ThirdPartyCreateDtoRequest
    {
        public string PersonType { get; set; } = null!;
        public int CompanyId { get; set; }
        public string IdentificationType { get; set; } = null!;
        public string IdentificationNumber { get; set; } = null!;
        public bool ActiveState { get; set; }
        public string Names { get; set; } = null!;

        public string? Login { get; set; }

        public string? Password { get; set; }

        public string? Email1 { get; set; }

        public string? Email2 { get; set; }
        public string? WebPage { get; set; }
        public string? FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        public string? ChargeCode { get; set; }

        public string? Initials { get; set; }

        public string? NatureCode { get; set; }

        public string? Phone1 { get; set; }

        public string? Phone2 { get; set; }

        public  AddressDtoRequest? Address { get; set; }

        public string? User { get; set; }
    }

}