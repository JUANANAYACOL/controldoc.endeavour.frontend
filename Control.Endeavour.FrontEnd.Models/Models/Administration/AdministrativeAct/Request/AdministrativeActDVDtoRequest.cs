using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeAct.Request
{
    public class AdministrativeActDVDtoRequest
    {
        public DateTime? AdminActDate { get; set; } = null;
        public string FileExt { get; set; }
        public string FileName { get; set; }
        public byte[] DataFile { get; set; }
    }
}
