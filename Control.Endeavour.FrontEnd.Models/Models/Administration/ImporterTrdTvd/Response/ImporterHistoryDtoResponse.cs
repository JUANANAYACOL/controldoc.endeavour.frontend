using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Response
{
    public class ImporterHistoryDtoResponse
    {
        public int ImporterHistoryId { get; set; }
        public int DocumentalVersionId { get; set; }
        public int FileId { get; set; }
        public string DescriptionHistory { get; set; } = null;
        public string CreateUser { get; set; } = null;
        public DateTime CreateDate { get; set; }
    }
}
