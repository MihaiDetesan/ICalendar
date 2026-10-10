using Avalonia.Controls;
using Avalonia.Interactivity;
using ICalendar.Desktop.ViewModels;

namespace ICalendar.Desktop.Views;

public partial class EventEditWindow : Window
{
    private EventEditViewModel ViewModel => (EventEditViewModel)DataContext!;

    public EventEditWindow(EventEditViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }

    private void OnSaveClicked(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ViewModel.Description))
        {
            ViewModel.ErrorMessage = "Please enter a description.";
            return;
        }

        if (ViewModel.SelectedCalendar == null)
        {
            ViewModel.ErrorMessage = "Please choose a calendar.";
            return;
        }

        if (ViewModel.StartDate == null || ViewModel.EndDate == null)
        {
            ViewModel.ErrorMessage = "Please choose a start and end date.";
            return;
        }

        if (ViewModel.EndDate.Value.Date < ViewModel.StartDate.Value.Date)
        {
            ViewModel.ErrorMessage = "End date must be on or after the start date.";
            return;
        }

        Close(true);
    }

    private void OnCancelClicked(object? sender, RoutedEventArgs e) => Close(false);
}
