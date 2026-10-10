using System;
using System.Collections.ObjectModel;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FontWeight = Avalonia.Media.FontWeight;

namespace ICalendar.Desktop.ViewModels;

public partial class DayCellViewModel : ViewModelBase
{
    public DayCellViewModel(DateTime date, bool isCurrentMonth, IAsyncRelayCommand<DayCellViewModel> dayClicked)
    {
        Date = date;
        IsCurrentMonth = isCurrentMonth;
        IsToday = date.Date == DateTime.Today;
        EventDots = new ObservableCollection<IBrush>();
        DayClickedCommand = dayClicked;
    }

    public DateTime Date { get; }
    public int DayNumber => Date.Day;
    public bool IsCurrentMonth { get; }
    public bool IsToday { get; }
    public ObservableCollection<IBrush> EventDots { get; }

    public IBrush NumberBrush => IsToday
        ? Brushes.White
        : IsCurrentMonth
            ? new SolidColorBrush(Color.Parse("#E5E7EB"))
            : new SolidColorBrush(Color.Parse("#4B5563"));

    public IBrush CellBackground => IsToday
        ? new SolidColorBrush(Color.Parse("#6366F1"))
        : Brushes.Transparent;

    public FontWeight NumberWeight => IsToday ? FontWeight.Bold : FontWeight.Normal;

    public IAsyncRelayCommand<DayCellViewModel> DayClickedCommand { get; }

    [ObservableProperty]
    private bool hasEvents;

    [ObservableProperty]
    private string toolTipText = string.Empty;
}
