using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HandsomeBot.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DynamicData;

namespace HandsomeBot.ViewModels;

public class BattlePageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public BattlePageViewModel(GameModel game, AllOptionsModel options)
    {
        TheGame = game; // Load game data
        AllOptions = options; // Load drop-down options
        for (int i = 0; i < 6; i++) // Attach image listeners to bot team
        {
            Sprites.Add(new());
            TheGame.BotTeam[i].Clear();
            TheGame.BotTeam[i].Attach(Sprites[i]);
            NameToNo.Add(TheGame.BotTeam[i].Name, i); // Add bot's pokemon to NameToNo dictionary
            if (!TheGame.MonsBrought.Contains(i))
            {
                TheGame.BotTeam[i].Position = "Not Brought";
            }
        }
        for (int i = 0; i < 4; i++)
        {
            AvailablePokemon[i] = TheGame.BotTeam[TheGame.MonsBrought[i]].Name;
        }
        for (int i = 0; i < 6; i++) // Attach image listeners to opponent team
        {
            Sprites.Add(new());
            TheGame.OppTeam[i].Clear();
            TheGame.OppTeam[i].Attach(Sprites[i + 6]);
        }
        TheGame.Turns.Add( // Add empty turn model for first turn
            new()
            {
                TurnNo = 1,
                EventList = [new()]
            }
        );
        CurrTurn = TheGame.Turns[1];
        CurrEvent = CurrTurn.EventList[0]; // Set current event to first in list
        for (int i = 0; i < 6; i++)
        {
            OpponentsPokemon[i] = "Opponent's " + TheGame.OppTeam[i].Name; // Add opponent's pokemon to opponent list
            AvailablePokemon[i + 4] = OpponentsPokemon[i]; // Add opponent's pokemon to list of mons in battle
            NameToNo.Add(OpponentsPokemon[i], i + 6); // Add opponent's pokemon to NameToNo dictionary
        }
        TheGame.Turns[0].BotEndMons = TheGame.Turns[0].BotStartMons;
        TheGame.Turns[0].OppEndMons = TheGame.Turns[0].OppStartMons;
        foreach (EventModel ev in TheGame.Turns[0].EventList)
        {
            if (ev.EventType == "Switch")
            {
                if (TheGame.Turns[0].BotEndMons.Contains(ev.UserMon)) TheGame.Turns[0].BotEndMons.Replace(ev.UserMon, ev.TargetMons[0].MonNo);
                else if (TheGame.Turns[0].OppEndMons.Contains(ev.UserMon)) TheGame.Turns[0].OppEndMons.Replace(ev.UserMon, ev.TargetMons[0].MonNo);
            }
        }
        for (int i = 0; i < 2; i++) // Set starting mons to previous turn's ending mons
        {
            CurrTurn.BotStartMons[i] = TheGame.Turns[0].BotEndMons[i];
            CurrTurn.OppStartMons[i] = TheGame.Turns[0].OppEndMons[i];
            TheGame.MonsSeen.Add(TheGame.Turns[1].OppStartMons[i]);
            TheGame.BotTeam[TheGame.Turns[1].BotStartMons[i]].Position = "Active";
            TheGame.OppTeam[TheGame.Turns[1].OppStartMons[i]].Position = "Active";
        }
        UserMonModel.Attach(UserSprite); // Attach UserSprite image listener to user mon model
        CurrEvent.Attach(EventType); // Attach event type listener to current event
        NextMove = new(TheGame, AllOptions, NameToNo); // Initialise next move model
        for (int i = 0; i < 2; i++)
        {
            ActiveMons[i].Name = TheGame.BotTeam[TheGame.Turns[1].BotStartMons[i]].Name;
            ActiveMons[i].Attach(ActiveSprites[i]);
            NextMove.Moves[i].TargetMon.Attach(TargetSprites[i]);
        }
        Task.Run(NextMove.LoadMonData).Wait();
        Task.Run(NextMove.UpdateNextMove);
    }

    public new event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private GameModel _theGame = new();

    public GameModel TheGame // All game data
    {
        get => _theGame;
        set
        {
            _theGame = value;
            OnPropertyChanged();
        }
    }

    private AllOptionsModel _allOptions = new();

    public AllOptionsModel AllOptions // All options for drop-down menus
    {
        get => _allOptions;
        set
        {
            _allOptions = value;
            OnPropertyChanged();
        }
    }

    public NextMoveModel NextMove { get; set; }

    /* -------------
    Handling sprites
    ------------- */

    private ObservableCollection<ImageListener> _sprites = [];

    public ObservableCollection<ImageListener> Sprites // Images listemers for pokemon sprites
    {
        get => _sprites;
        set
        {
            _sprites = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<ImageListener> _activeSprites = [new(), new()];

    public ObservableCollection<ImageListener> ActiveSprites
    {
        get => _activeSprites;
        set
        {
            _activeSprites = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<TeamModel> _activeMons = [new(), new()];

    public ObservableCollection<TeamModel> ActiveMons
    {
        get => _activeMons;
        set
        {
            _activeMons = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<ImageListener> _targetSprites = [new(), new()];

    public ObservableCollection<ImageListener> TargetSprites
    {
        get => _targetSprites;
        set
        {
            _targetSprites = value;
            OnPropertyChanged();
        }
    }

    private ImageListener _userSprite = new();

    public ImageListener UserSprite // Image listener for setting the user mon sprite in the event entry pop-up
    {
        get => _userSprite;
        set
        {
            _userSprite = value;
            OnPropertyChanged();
        }
    }

    public TeamModel UserMonModel = new(); // Team model for user mon in event entry pop-up

    /* -----------------------
    Handling event information
    ----------------------- */

    private TurnModel _currTurn = new();

    public TurnModel CurrTurn
    {
        get => _currTurn;
        set
        {
            _currTurn = value;
            OnPropertyChanged();
        }
    }

    public Dictionary<string, int> NameToNo { get; set; } = []; // To convert from pokemon name to number for storage

    private EventModel _currEvent = new();

    public EventModel CurrEvent // Copy of current event in TheGame
    {
        get => _currEvent;
        set
        {
            _currEvent = value;
            OnPropertyChanged();
        }
    }

    private EventTypeListener _eventType = new();

    public EventTypeListener EventType // Event type listener for updating which dropdowns are shown
    {
        get => _eventType;
        set
        {
            _eventType = value;
            OnPropertyChanged();
        }
    }

    public class TargetImageModel(TargetModel target, ImageListener image,
        AllOptionsModel allOptions, string[] availablePokemon)
    {
        public TargetModel Target { get; set; } = target;
        public ImageListener Image { get; } = image;
        public string[] AvailablePokemon { get; } = availablePokemon;
        public AllOptionsModel AllOptions { get; } = allOptions;
    }

    private ObservableCollection<TargetImageModel> _targetList = [];

    public ObservableCollection<TargetImageModel> TargetList
    {
        get => _targetList;
        set
        {
            _targetList = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<string> _formeList = [];

    public ObservableCollection<string> FormeList // List of pokemon's alternate formes, updated from AllOptions
    {
        get => _formeList;
        set
        {
            _formeList = value;
            OnPropertyChanged();
        }
    }

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
    
    private string[] _opponentsPokemon = new string[6]; // List of opponent's pokemon with opponent's prefix
    public string[] OpponentsPokemon
    {
    get => _opponentsPokemon;
        set
        {
            _opponentsPokemon = value;
            OnPropertyChanged();
        }
    }
    private string[] _availablePokemon = new string[10]; // List of pokemon that can trigger events
    public string[] AvailablePokemon
    {
    get => _availablePokemon;
        set
        {
            _availablePokemon = value;
            OnPropertyChanged();
        }
    }
    private string _userMonName = "";

    public string UserMonName // Name of pokemon triggering event
    {
        get => _userMonName;
        set
        {
            _userMonName = value;
            if (value != "" && value != null) // If a name is selected
            {
                CurrEvent.UserMon = NameToNo[value]; // Set user in current event
                UserMonModel.Name = value.Replace("Opponent's ", ""); // Update displayed sprite
                if (AllOptions.AllFormes.TryGetValue(UserMonModel.Name, out List<string>? temp)) FormeList = new(temp); // Get alternate formes for dropdown list
                else FormeList = [UserMonModel.Name];
            }
            else // Dummy data for if user not selected
            {
                CurrEvent.UserMon = -1;
                UserMonModel.Name = "None";
                FormeList = [];
            }
            OnPropertyChanged();
        }
    }

    public void AddTarget()
    {
        TargetList.Add(new(new(NameToNo), new(), AllOptions, AvailablePokemon));
        TargetList[^1].Target.Attach(TargetList[^1].Image);
        CurrEvent.TargetMons.Add(TargetList[^1].Target);
    }

    public void RemoveTarget()
    {
        if (TargetList.Count == 0) return;
        CurrEvent.TargetMons.RemoveAt(CurrEvent.TargetMons.Count - 1);
        TargetList.RemoveAt(TargetList.Count - 1);
    }

    public void NextEvent() // Increment current event in list
    {
        EventNumber++; // Increment event number
        CurrEvent.Clear(); // Detach event type listener
        CurrTurn.EventList.Add(new()); // Add new event model to turn model
        CurrEvent = CurrTurn.EventList[EventNumber]; // Make current event a copy of new event model
        TargetList = [];
        CurrEvent.Attach(EventType); // Attach event type listener
        UserMonName = ""; // Reset user mon name to clear sprite
    }
}