using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class SendDocumentDtoResponse
    {
        public string Description { get; set; } = string.Empty;
        public string Instruction { get; set; } = string.Empty;
        public VUserDtoResponse? Recivers { get; set; }
    }
}
