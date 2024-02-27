using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request
{
    public class SearchDtoRequest
    {
        public int ControlId { get; set; }
        public string DocDescription { get; set; }
        public string ClassCode { get; set; }
        public string FilingCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int DocumentaryTypologyBehaviorId { get; set; }
        public int MetaDataId { get; set; }
    }
}
