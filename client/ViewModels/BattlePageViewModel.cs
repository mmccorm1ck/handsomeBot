using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HandsomeBot.Models;
using System.Collections.ObjectModel;

namespace HandsomeBot.ViewModels;

public class BattlePageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public BattlePageViewModel(GameModel game, AllOptionsModel options)
    {
        TheGame = game;
        AllOptions = options;
        for (int i = 0; i < 6; i++)
        {
            Sprites.Add(new());
            TheGame.BotTeam[i].Attach(Sprites[i]);
        }
        for (int i = 0; i < 6; i++)
        {
            Sprites.Add(new());
            TheGame.OppTeam[i].Attach(Sprites[i + 6]);
        }
        TheGame.Turns.Add(
            new()
            {
                TurnNo = 1,
                EventList = [new()]
            }
        );
        CurrEvent = TheGame.Turns[1].EventList[0];
        for (int i = 0; i < 10; i++)
        {
            TargetsChecked.Add(new(i));
            TargetsChecked[i].Attach(CurrEvent);
        }
        for (int i = 0; i < 2; i++)
        {
            TheGame.Turns[1].BotStartMons[i] = TheGame.Turns[0].BotEndMons[i];
            TheGame.Turns[1].OppStartMons[i] = TheGame.Turns[0].OppEndMons[i];
        }
        for (int i = 0; i < 6; i++)
        {
            OpponentsPokemon[i] = "Opponent's " + TheGame.OppTeam[i].Name;
            AvailablePokemon[i + 4] = OpponentsPokemon[i];
            NameToNo.Add(OpponentsPokemon[i], i + 6);
        }
        UserMonModel.Attach(UserSprite);
        CurrEvent.Attach(EventType);
        if (TheGame.Gen < 9) FieldList[2] = "Hail";
    }

    public new event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private GameModel _theGame = new();

    public GameModel TheGame
    {
        get => _theGame;
        set
        {
            _theGame = value;
            OnPropertyChanged();
        }
    }

    private AllOptionsModel _allOptions = new();

    public AllOptionsModel AllOptions
    {
        get => _allOptions;
        set
        {
            _allOptions = value;
            OnPropertyChanged();
        }
    }

    /* -------------
    Handling sprites
    ------------- */

    private ObservableCollection<ImageListener> _sprites = [];

    public ObservableCollection<ImageListener> Sprites
    {
        get => _sprites;
        set
        {
            _sprites = value;
            OnPropertyChanged();
        }
    }

    private ImageListener _userSprite = new();

    public ImageListener UserSprite
    {
        get => _userSprite;
        set
        {
            _userSprite = value;
            OnPropertyChanged();
        }
    }

    public TeamModel UserMonModel = new();

    /* -----------------------
    Handling event information
    ----------------------- */

    private int _eventNumber = 0; // Tracks event number in chain of events

    public int EventNumber
    {
    get => _eventNumber;
        set
        {
            _eventNumber = value;
            OnPropertyChanged();
        }
    }
    
    private string[] _opponentsPokemon = new string[6]; // List of opponent's pokemon;
    public string[] OpponentsPokemon
    {
    get => _opponentsPokemon;
        set
        {
            _opponentsPokemon = value;
            OnPropertyChanged();
        }
    }
    private string[] _availablePokemon = new string[10]; // List of pokemon that can trigger events;
    public string[] AvailablePokemon
    {
    get => _availablePokemon;
        set
        {
            _availablePokemon = value;
            OnPropertyChanged();
        }
    }
    public string[] TypeList { get; set; } = [
        "Normal",
        "Fighting",
        "Flying",
        "Poison",
        "Ground",
        "Rock",
        "Bug",
        "Ghost",
        "Steel",
        "Fire",
        "Water",
        "Grass",
        "Electric",
        "Psychic",
        "Ice",
        "Dragon",
        "Dark",
        "Fairy",
        "???",
        "Stellar"
    ];

    public string[] StatList { get; set; } = [
        "Atk",
        "Def",
        "SpA",
        "SpD",
        "Spe"
    ];

    public string[] FieldList { get; set; } = [
        "Rain",
        "Sun",
        "Snow",
        "Sandstorm",
        "Electric Terrain",
        "Psychic Terrain",
        "Grassy Terrain",
        "Misty Terrain"
    ];

    private string _userMonName = "";

    public string UserMonName
    {
        get => _userMonName;
        set
        {
            _userMonName = value;
            if (value != "" && value != null)
            {
                CurrEvent.UserMon = NameToNo[value];
                UserMonModel.Name = value.Replace("Opponent's ", "");
                if (AllOptions.AllFormes.TryGetValue(UserMonModel.Name, out List<string>? temp)) FormeList = new(temp);
                else FormeList = [UserMonModel.Name];
            }
            else
            {
                CurrEvent.UserMon = -1;
                UserMonModel.Name = "None";
                FormeList = [];
            }
            OnPropertyChanged();
        }
    }

    public Dictionary<string, int> NameToNo { get; set; } = [];

    private ObservableCollection<string> _formeList = [];

    public ObservableCollection<string> FormeList
    {
        get => _formeList;
        set
        {
            _formeList = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<TargetSelectorModel> _targetsChecked = [];

    public ObservableCollection<TargetSelectorModel> TargetsChecked
    {
        get => _targetsChecked;
        set
        {
            _targetsChecked = value;
            OnPropertyChanged();
        }
    }

    private EventModel _currEvent = new();

    public EventModel CurrEvent
    {
        get => _currEvent;
        set
        {
            _currEvent = value;
            OnPropertyChanged();
        }
    }

    private EventTypeListener _eventType = new();

    public EventTypeListener EventType
    {
        get => _eventType;
        set
        {
            _eventType = value;
            OnPropertyChanged();
        }
    }

    public void NextEvent()
    {
        foreach (TargetSelectorModel selector in TargetsChecked) selector.Detach();
        EventNumber++;
        CurrEvent.Clear();
        TheGame.Turns[0].EventList.Add(new());
        CurrEvent = TheGame.Turns[0].EventList[EventNumber];
        foreach (TargetSelectorModel selector in TargetsChecked) selector.Attach(CurrEvent);
        CurrEvent.Attach(EventType);
        UserMonName = "";
    }
}