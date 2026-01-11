using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Markup.Xaml.MarkupExtensions;

namespace HandsomeBot.Models;
public class GameModel() : INotifyPropertyChanged // Class to hold info about a game
{
    public string ServerUrl // URL to connect to the calc server
    {
        get => _serverUrl;
        set
        {
            _serverUrl = value;
            OnPropertyChanged();
        }
    }
    public string Format // String representation of game format
    {
        get => _format;
        set
        {
            _format = value;
            OnPropertyChanged();
        }
    }
    public string GameType // Doubles or Singles
    {
        get => _gameType;
        set
        {
            _gameType = value;
            OnPropertyChanged();
        }
    }
    public int Gen // Game generation number
    {
        get => _gen;
        set
        {
            _gen = value;
            OnPropertyChanged();
        }
    }
    public string BotTeamURL // Pokepaste URL for bot's team
    {
        get => _botTeamURL;
        set
        {
            _botTeamURL = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<TeamModel> BotTeam // Bot't team info
    {
        get => _botTeam;
        set
        {
            _botTeam = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<TeamModel> OppTeam // Opponent's team info
    {
        get => _oppTeam;
        set
        {
            _oppTeam = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<TeamModel> BotTeamPrev // Bot's team from previous game, for use in best of 3 etc
    {
        get => _botTeamPrev;
        set
        {
            _botTeamPrev = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<TeamModel> OppTeamPrev // Opponent's team from previous game, for use in best of 3 etc
    {
        get => _oppTeamPrev;
        set
        {
            _oppTeamPrev = value;
            OnPropertyChanged();
        }
    }
    public ArenaModel CurrentArena // Current field effects
    {
        get => _currentArena;
        set
        {
            _currentArena = value;
            OnPropertyChanged();
        }
    }
    public List<int> MonsBrought // List of mons brought to battle
    {
        get => _monsBrought;
        set
        {
            _monsBrought = value;
        }
    }
    public List<int> MonsSeen // List of opponent mons seen so far in battle
    {
        get => _monsSeen;
        set
        {
            _monsSeen = value;
        }
    }
    public ObservableCollection<TurnModel> Turns // List of turn events
    {
        get => _turns;
        set
        {
            _turns = value;
            OnPropertyChanged();
        }
    }
    public bool ZoroPresent
    {
        get => _zoroPresent;
        set
        {
            _zoroPresent = value;
            OnPropertyChanged();
        }
    }
    public GimmickList Gimmicks
    {
        get => _gimmicks;
        set
        {
            _gimmicks = value;
            OnPropertyChanged();
        }
    }
    private string _serverUrl = "";
    private string _format = "";
    private string _gameType = "Singles";
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
    private List<int> _monsBrought = [];
    private List<int> _monsSeen = [];
    private ObservableCollection<TurnModel> _turns = [];
    private bool _zoroPresent = false;
    private GimmickList _gimmicks = new();
    public class GimmickList
    {
        public bool Megas
        {
            get => _megas;
            set
            {
                Reset();
                _megas = value;
                OnPropertyChanged();
            }
        }
        public bool ZMoves
        {
            get => _zMoves;
            set
            {
                Reset();
                _zMoves = value;
                OnPropertyChanged();
            }
        }
        public bool Dynamax
        {
            get => _dynamax;
            set
            {
                Reset();
                _dynamax = value;
                OnPropertyChanged();
            }
        }
        public bool Tera
        {
            get => _tera;
            set
            {
                Reset();
                _tera = value;
                OnPropertyChanged();
            }
        }
        private bool _megas = false;
        private bool _zMoves = false;
        private bool _dynamax = false;
        private bool _tera = false;
        public void Reset()
        {
            _megas = false;
            _zMoves = false;
            _dynamax = false;
            _tera = false;
        }
        public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}