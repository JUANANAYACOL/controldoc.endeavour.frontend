using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request
{
    public class CompanyCreateDtoRequest
    {
        public string IdentificationType { get; set; } = null!;
        public string Identification { get; set; } = null!;
        public string? BusinessName { get; set; }
        public string? User { get; set; }
        public virtual CompanyDataDtoRequest? CompanyData { get; set; }
    }
}
