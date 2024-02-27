namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request
{
    public class FilterManagementDtoRequest
    {

#nullable enable

        public int TaskId { get; set; }
        public int ControlId { get; set; }
        public string? ClassCode { get; set; }
        public int UserTaskId { get; set; }
        public int UserForwardId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string>? ProcessCode { get; set; } = new List<string>();
        public List<string>? InstructionCode { get; set; } = new List<string>();
        public string? TaskDescription { get; set; }
        public bool? Indicted { get; set; }
    }

    public class Visor
    {
        public int identification { get; set; }
        public int id { get; set; }
    }
}
