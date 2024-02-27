

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response
{
    public class AppKeysDtoResponse
    {
        public int AppKeyId { get; set; }
        public int AppFunctionId { get; set; }
        public string? KeyName { get; set; }
        public string? Value1 { get; set; }
        public string? Value2 { get; set; }
        public string? Value3 { get; set; }

        public string? Value4 { get; set; }
        public AppFunctionDtoResponse? AppFunction { get; set; }
    }
}
