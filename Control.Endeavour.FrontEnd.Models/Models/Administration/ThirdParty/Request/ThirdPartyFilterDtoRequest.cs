using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdParty.Request
{
    public class ThirdPartyFilterDtoRequest
    {
        public int CompanyId { get; set; }
        public string? PersonType { get; set; }
        public string? Names { get; set; }
        public string? Email { get; set; }
        public string? IdentificationNumber { get; set; }
    }
}