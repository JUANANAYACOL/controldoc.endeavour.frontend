﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request
{
    public class OverrideTrayReasonEditDtoRequest
    {
        public int CancelationReasonId { get; set; }
        public string NameReason { get; set; }
        public string ReasonCode { get; set; }
        public string TypeCode { get; set; }
        public string User { get; set; }
    }
}
