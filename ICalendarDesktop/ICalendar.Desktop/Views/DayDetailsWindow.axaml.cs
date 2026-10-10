using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ICalendar.Core.Model;
using ICalendar.Desktop.ViewModels;

namespace ICalendar.Desktop.Views;

public partial class DayDetailsWindow : Window
{
    private readonly MainViewModel mainViewModel;
    private DayDetailsViewModel Details => (DayDetailsViewModel)DataContext!;

    public DayDetailsWindow(DayDetailsViewModel details, MainViewModel mainViewModel)
    {
        this.mainViewModel = mainViewModel;
        DataContext = details;
        InitializeComponent();
    }

    private void RefreshEventsList()
    {
        Details.Events.Clear();
        foreach (var row in mainViewModel.GetEventsForDay(Details.Date))
        {
            Details.Events.Add(row);
        }
    }

    private async void OnAddClicked(object? sender, RoutedEventArgs e)
    {
        var editor = new EventEditViewModel(ToCalendarOptions(), isEdit: false)
        {
            StartDate = Details.Date,
            EndDate = Details.Date,
        };

        var window = new EventEditWindow(editor);
        var saved = await window.ShowDialog<bool>(this);

        if (saved && editor.SelectedCalendar != null)
        {
            var eventDay = new EventDay
            {
                CalendarId = editor.SelectedCalendar.Id,
                Calendar = editor.SelectedCalendar.Summary,
                Description = editor.Description,
                StartDate = editor.StartDate!.Value.Date,
                EndDate = editor.EndDate!.Value.Date.AddDays(1),
            };

            await mainViewModel.AddEventAsync(eventDay);
            RefreshEventsList();
        }
    }

    private async void OnEditClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: EventRowViewModel row })
        {
            return;
        }

        var options = ToCalendarOptions();
        var editor = new EventEditViewModel(options, isEdit: true)
        {
            Description = row.EventDay.Description,
            StartDate = row.EventDay.StartDate,
            EndDate = row.EventDay.EndDate.AddDays(-1),
            SelectedCalendar = options.FirstOrDefaultById(row.EventDay.CalendarId),
        };

        var window = new EventEditWindow(editor);
        var saved = await window.ShowDialog<bool>(this);

        if (saved && editor.SelectedCalendar != null)
        {
            var eventDay = new EventDay
            {
                CalendarId = editor.SelectedCalendar.Id,
                Calendar = editor.SelectedCalendar.Summary,
                Description = editor.Description,
                StartDate = editor.StartDate!.Value.Date,
                EndDate = editor.EndDate!.Value.Date.AddDays(1),
            };

            await mainViewModel.EditEventAsync(eventDay, row.EventDay.Id, row.EventDay.CalendarId);
            RefreshEventsList();
        }
    }

    private async void OnDeleteClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: EventRowViewModel row })
        {
            return;
        }

        await mainViewModel.DeleteEventAsync(row.EventDay.Id, row.EventDay.CalendarId);
        RefreshEventsList();
    }

    private void OnCloseClicked(object? sender, RoutedEventArgs e) => Close();

    private ObservableCollection<CalendarItemViewModel> ToCalendarOptions() => mainViewModel.Calendars;
}

internal static class CalendarOptionExtensions
{
    public static CalendarItemViewModel? FirstOrDefaultById(this ObservableCollection<CalendarItemViewModel> options, string id)
    {
        foreach (var option in options)
        {
            if (option.Id == id)
            {
                return option;
            }
        }

        return null;
    }
}
