
namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
public class WorKFlowDtoResponse 
{
    public int ControlId { get; set; }
    public string FilingCode { get; set; }
    public List<DocumentManagementsDtoResponse> DocumentManagements { get; set; } = new();
}

public class DocumentManagementsDtoResponse
{
    public int ManagementOrder { get; set; }
    public int AssingUserId { get; set; }
    public DateTime AssingDate { get; set; }
    public string InstructionCode { get; set; }
    public DateTime ProcessedDate { get; set; }
    public string ActionCode { get; set; }
    public string Commentary { get; set; } 
    public string FlowStateCode { get; set; }
    public int ProcessedUserId { get; set; }
    public string processUserName { get; set; }
    public string processUserAdministrativeUnit { get; set; }
    public string processUserProductionOffice { get; set; }
    public string processUserBrachOffice { get; set; }




}