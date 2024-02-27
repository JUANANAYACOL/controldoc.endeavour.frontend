using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeAct.Response
{
    public class AdministrativeActDVDtoResponse
    {
        public int AdministrativeActId { get; set; }
        public int FileId { get; set; }
        public string AdministrativeActName { get; set; }
        public DateTime? AdminActDate { get; set; }
    }
}
