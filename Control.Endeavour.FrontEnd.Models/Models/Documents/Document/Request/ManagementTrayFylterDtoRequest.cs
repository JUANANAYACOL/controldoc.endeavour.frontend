using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;

public class ManagementTrayFylterDtoRequest
{
    public int AssingUserId { get; set; }
    public string? FlowStateCode { get; set; }
    public string? ClassCode { get; set; }
    public int ControlId { get; set; }
    public string? FilingCode { get; set; }
    public string? PriorityFields { get; set; }    
    public int Year { get; set; }
    public int Month { get; set; }
    public int Days { get; set; }
    public bool DueDate { get; set; }
    
}
