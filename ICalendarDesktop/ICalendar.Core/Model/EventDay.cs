using System;

namespace ICalendar.Core.Model
{
    public class EventDay
    {
        public string Id { get; set; } = string.Empty;
        public string Calendar { get; set; } = string.Empty;
        public string CalendarBkColor { get; set; } = string.Empty;
        public string CalendarId { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
