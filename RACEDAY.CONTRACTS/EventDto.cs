
using System;

namespace RaceDay.Contracts
{
    public class EventDto
    {
        public int EventID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string Location { get; set; } = string.Empty;

        public decimal Distance { get; set; }

        public string Type { get; set; } = string.Empty;

        public int OrganiserID { get; set; }
    }
}
