using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;
public class GameModel() : INotifyPropertyChanged // Class to hold info about a game
{
    public string ServerUrl
    {
        get => _serverUrl;
        set
        {
            _serverUrl = value;
            OnPropertyChanged();
        }
    }
    public string Format
    {
        get => _format;
        set
        {
            _format = value;
            OnPropertyChanged();
        }
    }
    public int Gen
    {
        get => _gen;
        set
        {
            _gen = value;
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
    public ObservableCollection<TeamModel> BotTeam
    {
        get => _botTeam;
        set
        {
            _botTeam = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<TeamModel> OppTeam
    {
        get => _oppTeam;
        set
        {
            _oppTeam = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<TeamModel> BotTeamPrev
    {
        get => _botTeamPrev;
        set
        {
            _botTeamPrev = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<TeamModel> OppTeamPrev
    {
        get => _oppTeamPrev;
        set
        {
            _oppTeamPrev = value;
            OnPropertyChanged();
        }
    }
    public ArenaModel CurrentArena
    {
        get => _currentArena;
        set
        {
            _currentArena = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<TurnModel> Turns
    {
        get => _turns;
        set
        {
            _turns = value;
            OnPropertyChanged();
        }
    }
    private string _serverUrl = "";
    private string _format = "";
    private int _gen = 0;
    private string _botTeamURL = "";
    private ObservableCollection<TeamModel> _botTeam = new()
    {
        new(), new(), new(), new(), new(), new()
    };
    private ObservableCollection<TeamModel> _botTeamPrev = new()
    {
        new(), new(), new(), new(), new(), new()
    };
    private ObservableCollection<TeamModel> _oppTeam = new()
    {
        new(), new(), new(), new(), new(), new()
    };
    private ObservableCollection<TeamModel> _oppTeamPrev = new()
    {
        new(), new(), new(), new(), new(), new()
    };
    private ArenaModel _currentArena = new();
    private ObservableCollection<TurnModel> _turns = [];
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}