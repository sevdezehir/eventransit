using System.Collections.Generic;

namespace EvenTransit.Service.Dto.Event
{
    public class SaveServiceDto
    {
        public string EventId { get; set; }
        public string ServiceName { get; set; }
        public string Url { get; set; }
        public int Timeout { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}