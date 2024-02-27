
namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
public class ManagementTrayDtoResponse
{
    public int assignUserId { get; set; }
    public int controlId { get; set; }
    public string nameDocumentaryTypologiesBag { get; set; }
    public string filingCode { get; set; }
    public string docDescription { get; set; }
    public DateTime docDate { get; set; }
    public DateTime dueDate { get; set; }
    public string priorityCode { get; set; }
    public string priorityCodeName { get; set; }
    public string documentReceivers { get; set; }
    public string documentSignatories { get; set; }
    public bool Selected { get; set; }
}
