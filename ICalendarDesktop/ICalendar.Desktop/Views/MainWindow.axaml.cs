using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using ICalendar.Desktop.ViewModels;

namespace ICalendar.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        viewModel.DayDetailsRequested += OnDayDetailsRequestedAsync;
        await viewModel.InitializeAsync();
    }

    private async Task OnDayDetailsRequestedAsync(DayDetailsViewModel details)
    {
        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        var window = new DayDetailsWindow(details, viewModel);
        await window.ShowDialog(this);
    }
}
