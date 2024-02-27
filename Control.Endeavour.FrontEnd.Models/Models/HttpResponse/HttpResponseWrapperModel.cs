using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.HttpResponse
{
    public class HttpResponseWrapperModel<T>
    {
        public bool Succeeded { get; set; } = false;
        public T? Data { get; set; } = default;
        public List<string>? Errors { get; set; }
        public string? CodeError { get; set; }
        public string? Message { get; set; }
        public MetaModel? Meta { get; set; }
    }

    public class EventCallbackArgs
    {
        public string? Vista { get; set; }
        public string? Uuid { get; set; }
        public string? Ip { get; set; }
        public string? User { get; set; }
    }
}
