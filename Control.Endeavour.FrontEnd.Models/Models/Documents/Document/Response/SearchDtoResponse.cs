using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response
{
    public class SearchDtoResponse
    {
        public int DocumentId { get; set; } 
        public string FilingCode { get; set; } 
        public int ControlId { get; set; }
        public DateTime DocDate { get; set; } 
        public string DocDescription { get; set; }
        public string year { get; set; } 
        public string ClassCode { get; set; } 
        public string DocumentReceivers { get; set; }
        public string DocumentSignatories { get; set; }
    }
}
