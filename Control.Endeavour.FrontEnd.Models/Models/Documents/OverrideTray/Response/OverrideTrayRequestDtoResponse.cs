using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response
{
    public class OverrideTrayRequestDtoResponse
    {
        public int CancelationRequestId { get; set; }
        public int CancelationReasonId { get; set; }
        public int ControlId { get; set; }
        public string TypeRequestCode { get; set; }
        public string TypeCode { get; set; }
        public int UserRequestId { get; set; }
        public int UserManagerId { get; set; }
        public DateTime DateRequest { get; set; }
        public string CancelationState { get; set; }
        public string RequestComment { get; set; }
        public string NameTypeRequestCode { get; set; }
        public string NameTypeCode { get; set; }
        public string NameCancelationState { get; set; }
    }
}
