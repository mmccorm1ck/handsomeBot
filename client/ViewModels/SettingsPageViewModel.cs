using System.ComponentModel;
using System.Runtime.CompilerServices;
using HandsomeBot.Models;

namespace HandsomeBot.ViewModels;

public class SettingsPageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public SettingsPageViewModel(GameModel game, AllOptionsModel options)
    {
        TheGame = game;
    }
    public new event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private GameModel _theGame = new();

    public GameModel TheGame // Holds all game data
    {
        get => _theGame;
        set
        {
            _theGame = value;
            OnPropertyChanged();
        }
    }
}