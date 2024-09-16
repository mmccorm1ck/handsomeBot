using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;
public class GameModel() : INotifyPropertyChanged // Class to hold info about a pokemon in a team
{
    public string Format
    {
        get => _format;
        set
        {
            _format = value;
            OnPropertyChanged();
        }
    }
    public string BotTeamURL
    {
        get => _botTeamURL;
        set
        {
            _botTeamURL = value;
            OnPropertyChanged();
        }
    }
    public string OppTeamURL
    {
        get => _oppTeamURL;
        set
        {
            _oppTeamURL = value;
            OnPropertyChanged();
        }
    }
    private string _format = "";
    private string _botTeamURL = "";
    private string _oppTeamURL = "";
    public event PropertyChangedEventHandler PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}