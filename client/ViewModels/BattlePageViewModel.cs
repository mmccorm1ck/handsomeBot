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
        TheGame = game; // Load game data
        AllOptions = options; // Load drop-dow options
        for (int i = 0; i < 6; i++) // Attach image listeners to bot team
        {
            Sprites.Add(new());
            TheGame.BotTeam[i].Clear();
            TheGame.BotTeam[i].Attach(Sprites[i]);
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
        CurrEvent = TheGame.Turns[1].EventList[0]; // Set current event to first in list
        for (int i = 0; i < 10; i++) // Attach target check listeners to current event
        {
            TargetsChecked.Add(new(i));
            TargetsChecked[i].Attach(CurrEvent);
        }
        for (int i = 0; i < 2; i++) // Set starting mons to previous turn's ending mons
        {
            TheGame.Turns[1].BotStartMons[i] = TheGame.Turns[0].BotEndMons[i];
            TheGame.Turns[1].OppStartMons[i] = TheGame.Turns[0].OppEndMons[i];
        }
        for (int i = 0; i < 6; i++)
        {
            OpponentsPokemon[i] = "Opponent's " + TheGame.OppTeam[i].Name; // Add opponent's pokemon to opponent list
            AvailablePokemon[i + 4] = OpponentsPokemon[i]; // Add opponent's pokemon to list of mons in battle
            NameToNo.Add(OpponentsPokemon[i], i + 6); // Add opponent's pokemon to NameToNo dictionary
        }
        UserMonModel.Attach(UserSprite); // Attach UserSprite image listener to user mon model
        CurrEvent.Attach(EventType); // Attach event type listener to current event
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

    private ObservableCollection<TargetSelectorModel> _targetsChecked = [];

    public ObservableCollection<TargetSelectorModel> TargetsChecked // TargetSelectors for updating target list in current event 
    {
        get => _targetsChecked;
        set
        {
            _targetsChecked = value;
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

    public void NextEvent() // Increment current event in list
    {
        foreach (TargetSelectorModel selector in TargetsChecked) selector.Detach(); // Detach all target selectors
        EventNumber++; // Increment event number
        CurrEvent.Clear(); // Detach event type listener
        TheGame.Turns[0].EventList.Add(new()); // Add new event model to turn model
        CurrEvent = TheGame.Turns[0].EventList[EventNumber]; // Maeke current event a copy of new event model
        foreach (TargetSelectorModel selector in TargetsChecked) selector.Attach(CurrEvent); // Reattach target selectors
        CurrEvent.Attach(EventType); // Attach event type listener
        UserMonName = ""; // Reset user mon name to clear sprite
    }
}