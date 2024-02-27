namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request
{
    public class SecondPasswordDtoRequest
    {
        public int UserId { get; set; }

        public string SecondPassword { get; set; } = null!;
    }
}
