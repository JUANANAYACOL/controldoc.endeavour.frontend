using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;

public class ObjectTransaction
{
    public VUserDtoResponse UserInfo { get; set; }
    public string? Action { get; set; }
    public string? Days { get; set; }
    public string? Hours { get; set; }
    public string? Subject { get; set; }
    public int Position { get; set; }
    public int CountCharacters { get; set; } = 0;
}
