using System;
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.Input;

namespace ICalendar.Desktop.ViewModels;

public class MonthViewModel : ViewModelBase
{
    public MonthViewModel(int year, int month, IAsyncRelayCommand<DayCellViewModel> dayClicked)
    {
        Year = year;
        Month = month;
        MonthName = new DateTime(year, month, 1).ToString("MMMM", CultureInfo.CurrentCulture);
        Days = new ObservableCollection<DayCellViewModel>();

        BuildGrid(dayClicked);
    }

    public int Year { get; }
    public int Month { get; }
    public string MonthName { get; }
    public ObservableCollection<DayCellViewModel> Days { get; }

    private void BuildGrid(IAsyncRelayCommand<DayCellViewModel> dayClicked)
    {
        var firstOfMonth = new DateTime(Year, Month, 1);

        // Monday-first grid.
        var firstDayOfWeek = (int)firstOfMonth.DayOfWeek;
        var leadingDays = firstDayOfWeek == 0 ? 6 : firstDayOfWeek - 1;
        var gridStart = firstOfMonth.AddDays(-leadingDays);

        const int totalCells = 42; // 6 rows x 7 columns, enough for any month layout
        for (var i = 0; i < totalCells; i++)
        {
            var date = gridStart.AddDays(i);
            Days.Add(new DayCellViewModel(date, date.Month == Month, dayClicked));
        }
    }
}
