using System;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ICalendar.Desktop.ViewModels;

public partial class CalendarItemViewModel : ViewModelBase
{
    private readonly Func<CalendarItemViewModel, bool, Task>? onSelectionChanged;
    private bool suppressCallback;

    public CalendarItemViewModel(string id, string summary, string? colorHex, bool isSelected,
        Func<CalendarItemViewModel, bool, Task>? onSelectionChanged)
    {
        Id = id;
        Summary = summary;
        Color = TryParseColor(colorHex);
        this.onSelectionChanged = onSelectionChanged;

        suppressCallback = true;
        IsSelected = isSelected;
        suppressCallback = false;
    }

    public string Id { get; }
    public string Summary { get; }
    public IBrush Color { get; }

    [ObservableProperty]
    private bool isSelected;

    partial void OnIsSelectedChanged(bool value)
    {
        if (suppressCallback)
        {
            return;
        }

        _ = onSelectionChanged?.Invoke(this, value);
    }

    private static IBrush TryParseColor(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            return Brushes.Gray;
        }

        try
        {
            return new SolidColorBrush(Avalonia.Media.Color.Parse(hex));
        }
        catch
        {
            return Brushes.Gray;
        }
    }
}
