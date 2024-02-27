﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Request
{
    public class ImporterDtoRequest
    {
        public int DocumentalVersionId { get; set; }
        public string FileExt { get; set; }
        public string FileName { get; set; }
        public byte[] DataFile { get; set; }
        public string DescriptionHistory { get; set; } = null;
        public string User { get; set; }
    }
}
