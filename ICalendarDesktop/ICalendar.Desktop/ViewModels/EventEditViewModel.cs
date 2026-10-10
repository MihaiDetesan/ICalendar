using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ICalendar.Desktop.ViewModels;

public partial class EventEditViewModel : ViewModelBase
{
    public EventEditViewModel(ObservableCollection<CalendarItemViewModel> availableCalendars, bool isEdit)
    {
        AvailableCalendars = availableCalendars;
        IsEdit = isEdit;
        Title = isEdit ? "Edit event" : "Add event";
    }

    public ObservableCollection<CalendarItemViewModel> AvailableCalendars { get; }
    public bool IsEdit { get; }
    public string Title { get; }

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private DateTime? startDate = DateTime.Today;

    [ObservableProperty]
    private DateTime? endDate = DateTime.Today;

    [ObservableProperty]
    private CalendarItemViewModel? selectedCalendar;

    [ObservableProperty]
    private string errorMessage = string.Empty;
}
