using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICalendar.Core;
using ICalendar.Core.Model;
using ICalendar.Desktop.Services;

namespace ICalendar.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly GoogleCalendarImporter importer;
    private readonly Dictionary<DateTime, List<DayCellViewModel>> cellIndex = new();

    public MainViewModel()
    {
        importer = new GoogleCalendarImporter(new GoogleOptions
        {
            ApplicationName = "ICalendar Desktop",
            CredentialsPath = AppPaths.CredentialsPath,
            TokenPath = AppPaths.TokenPath,
        });

        Calendars = new ObservableCollection<CalendarItemViewModel>();
        Months = new ObservableCollection<MonthViewModel>();
        DisplayYear = DateTime.Today.Year;

        BuildMonthGrid();
    }

    public ObservableCollection<MonthViewModel> Months { get; }
    public ObservableCollection<CalendarItemViewModel> Calendars { get; }

    [ObservableProperty]
    private int displayYear;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = "Starting up…";

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public event Func<DayDetailsViewModel, Task>? DayDetailsRequested;

    public async Task InitializeAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Signing in to Google Calendar…";
            await importer.AuthenticateAsync();

            StatusMessage = "Loading events…";
            await LoadYearAsync();

            StatusMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not connect to Google Calendar: {ex.Message}";
            StatusMessage = string.Empty;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task PreviousYearAsync()
    {
        DisplayYear--;
        await ReloadForYearChangeAsync();
    }

    [RelayCommand]
    private async Task NextYearAsync()
    {
        DisplayYear++;
        await ReloadForYearChangeAsync();
    }

    [RelayCommand]
    private async Task TodayAsync()
    {
        DisplayYear = DateTime.Today.Year;
        await ReloadForYearChangeAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync() => await LoadYearAsync();

    [RelayCommand]
    private async Task DayClickedAsync(DayCellViewModel? day)
    {
        if (day == null || DayDetailsRequested == null)
        {
            return;
        }

        var rows = importer.GetEventsForDay(day.Date)
            .Select(ev => new EventRowViewModel(ev, ColorFromHex(ev.CalendarBkColor)))
            .ToList();

        var details = new DayDetailsViewModel(day.Date, new ObservableCollection<EventRowViewModel>(rows));
        await DayDetailsRequested.Invoke(details);
    }

    public IEnumerable<EventRowViewModel> GetEventsForDay(DateTime date) =>
        importer.GetEventsForDay(date).Select(ev => new EventRowViewModel(ev, ColorFromHex(ev.CalendarBkColor)));

    public async Task<bool> AddEventAsync(EventDay eventDay)
    {
        return await RunGuardedAsync(() => importer.AddEventAsync(eventDay));
    }

    public async Task<bool> EditEventAsync(EventDay eventDay, string eventId, string calendarId)
    {
        return await RunGuardedAsync(() => importer.EditEventAsync(eventDay, eventId, calendarId));
    }

    public async Task<bool> DeleteEventAsync(string eventId, string calendarId)
    {
        return await RunGuardedAsync(() => importer.DeleteEventAsync(eventId, calendarId));
    }

    private async Task<bool> RunGuardedAsync(Func<Task> action)
    {
        try
        {
            IsBusy = true;
            await action();
            await LoadYearAsync();
            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Operation failed: {ex.Message}";
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ReloadForYearChangeAsync()
    {
        BuildMonthGrid();
        await LoadYearAsync();
    }

    private async Task LoadYearAsync()
    {
        try
        {
            IsBusy = true;
            var start = new DateTime(DisplayYear, 1, 1);
            var end = new DateTime(DisplayYear, 12, 31, 23, 59, 59);

            await importer.RefreshEventsAsync(start, end);

            SyncCalendarList();
            ApplyEventsToGrid();
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not load events: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void SyncCalendarList()
    {
        var selectedIds = Calendars.Where(c => c.IsSelected).Select(c => c.Id).ToHashSet();
        Calendars.Clear();

        foreach (var cal in importer.Calendars.OrderBy(c => c.Summary))
        {
            var isSelected = cal.Selected ?? selectedIds.Contains(cal.Id);
            Calendars.Add(new CalendarItemViewModel(cal.Id, cal.Summary, cal.BackgroundColor, isSelected, OnCalendarSelectionChangedAsync));
        }
    }

    private async Task OnCalendarSelectionChangedAsync(CalendarItemViewModel calendar, bool isSelected)
    {
        await RunGuardedAsync(() => importer.SetCalendarSelectedAsync(calendar.Summary, isSelected));
    }

    private void BuildMonthGrid()
    {
        Months.Clear();
        cellIndex.Clear();

        for (var month = 1; month <= 12; month++)
        {
            var monthVm = new MonthViewModel(DisplayYear, month, DayClickedCommand);
            Months.Add(monthVm);

            foreach (var day in monthVm.Days)
            {
                if (!cellIndex.TryGetValue(day.Date, out var list))
                {
                    list = new List<DayCellViewModel>();
                    cellIndex[day.Date] = list;
                }

                list.Add(day);
            }
        }
    }

    private void ApplyEventsToGrid()
    {
        foreach (var cells in cellIndex.Values)
        {
            foreach (var cell in cells)
            {
                cell.EventDots.Clear();
                cell.HasEvents = false;
                cell.ToolTipText = string.Empty;
            }
        }

        foreach (var ev in importer.Events)
        {
            var color = ColorFromHex(ev.CalendarBkColor);
            var date = ev.StartDate.Date;

            while (date < ev.EndDate.Date || date == ev.StartDate.Date)
            {
                if (cellIndex.TryGetValue(date, out var cells))
                {
                    foreach (var cell in cells)
                    {
                        if (cell.EventDots.Count < 4)
                        {
                            cell.EventDots.Add(color);
                        }

                        cell.HasEvents = true;
                        cell.ToolTipText = string.IsNullOrEmpty(cell.ToolTipText)
                            ? ev.Description
                            : cell.ToolTipText + Environment.NewLine + ev.Description;
                    }
                }

                if (date >= ev.EndDate.Date)
                {
                    break;
                }

                date = date.AddDays(1);
            }
        }
    }

    private static IBrush ColorFromHex(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            return Brushes.SlateGray;
        }

        try
        {
            return new SolidColorBrush(Avalonia.Media.Color.Parse(hex));
        }
        catch
        {
            return Brushes.SlateGray;
        }
    }
}
