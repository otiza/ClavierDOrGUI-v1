using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace ClavierDOrGUI.ViewModels;

public class MainMenuViewModel : BaseViewModel
{
    public ICommand NewGameCommand { get; }
    public ICommand ResumeGameCommand { get; }
    public ICommand HistoryCommand { get; }
    public ICommand QuitCommand { get; }

    public MainMenuViewModel()
    {
        NewGameCommand = new Command(async () => await Shell.Current.GoToAsync("NewGamePage"));
        ResumeGameCommand = new Command(async () => await Shell.Current.GoToAsync("ResumeGamePage"));
        HistoryCommand = new Command(async () => await Shell.Current.GoToAsync("HistoryPage"));
        QuitCommand = new Command(() =>
        {
            // Terminate the process - simple cross-platform quit for now
            System.Environment.Exit(0);
        });
    }
}

