

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
public class ManagementTrayDtoRequest
{    
    public int ControlId { get; set; }
    public int ProcessedUserId { get; set; }
    public string ActionCode { get; set; }
    public List<AssignedUserIdDtoRequest> AssignedUserIds { get; set; } = new();     
}

public class AssignedUserIdDtoRequest
{
    public int AssignUserId { get; set; }
    public string Commentary { get; set; }
    public string  InstructionCode { get; set; }
    public bool  ItsCopy { get; set; }
}




