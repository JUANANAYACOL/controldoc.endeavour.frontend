using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request
{
    public class OverrideTrayRequestDtoRequest
    {
        public int ControlId { get; set; }
        public int CancelationReasonId { get; set; }
        public string TypeRequestCode { get; set; }
        public string TypeCode { get; set; }
        public string cancelationState { get; set; }
        public int UserRequestId { get; set; }
        public string RequestComment { get; set; }
        public string Email { get; set; }
        public string User { get; set; }
        
      
        public string FileExt { get; set; }
        public string FileName { get; set; }
        public string Archivo { get; set; }
    }
}
