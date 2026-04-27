using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ClavierDOrGUI.Models;

namespace ClavierDOrGUI.ViewModels;

public class NewGameViewModel : BaseViewModel
{
    private string _playerName = string.Empty;
    private PlayerRole _selectedRole = PlayerRole.FrontDeveloper;

    public string PlayerName
    {
        get => _playerName;
        set => SetProperty(ref _playerName, value);
    }

    public PlayerRole SelectedRole
    {
        get => _selectedRole;
        set => SetProperty(ref _selectedRole, value);
    }

    public List<PlayerRole> Roles { get; } = new List<PlayerRole>
    {
        PlayerRole.FrontDeveloper,
        PlayerRole.BackDeveloper,
        PlayerRole.MobileDeveloper
    };

    public ICommand StartGameCommand { get; }
    public ICommand CancelCommand { get; }

    public NewGameViewModel()
    {
        StartGameCommand = new Command(async () => await StartGameAsync());
        CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    private async Task StartGameAsync()
    {
        if (string.IsNullOrWhiteSpace(PlayerName))
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez entrer un nom de joueur.", "OK");
            return;
        }

        // For now navigate to GamePage route; the GamePage will be implemented in later steps.
        // Pass player name and role via query parameters (simple approach)
        var route = $"GamePage?playerName={System.Net.WebUtility.UrlEncode(PlayerName)}&role={(int)SelectedRole}";
        await Shell.Current.GoToAsync(route);
    }
}

