using Avalonia.Media;
using ICalendar.Core.Model;

namespace ICalendar.Desktop.ViewModels;

public class EventRowViewModel : ViewModelBase
{
    public EventRowViewModel(EventDay eventDay, IBrush color)
    {
        EventDay = eventDay;
        Color = color;
    }

    public EventDay EventDay { get; }
    public IBrush Color { get; }
    public string Description => EventDay.Description;
    public string Calendar => EventDay.Calendar;
    public string DateRange => EventDay.StartDate.ToShortDateString() == EventDay.EndDate.AddDays(-1).ToShortDateString()
        ? EventDay.StartDate.ToShortDateString()
        : $"{EventDay.StartDate:MMM d} – {EventDay.EndDate.AddDays(-1):MMM d}";
}
