using System;
using System.Collections.ObjectModel;

namespace ICalendar.Desktop.ViewModels;

public class DayDetailsViewModel : ViewModelBase
{
    public DayDetailsViewModel(DateTime date, ObservableCollection<EventRowViewModel> events)
    {
        Date = date;
        Events = events;
    }

    public DateTime Date { get; }
    public string DateDisplay => Date.ToString("dddd, MMMM d, yyyy");
    public ObservableCollection<EventRowViewModel> Events { get; }
    public bool HasEvents => Events.Count > 0;
}
