using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class DocumentalVersionFilterDtoRequest
    {
        public int CompanyId { get; set; }
        public string VersionType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; } = null;
    }
}
