using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using HandsomeBot.Models;
using System.Collections.ObjectModel;

namespace HandsomeBot.ViewModels;

public class OpenerPageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public OpenerPageViewModel(GameModel game, AllOptionsModel options)
    {
        TheGame = game; // Load game data
        AllOptions = options; // Load dropdown options
        for (int i = 0; i < 6; i++) // Initialise image listeners for sprites
        {
            MonsForSprites.Add(new());
            Sprites.Add(new());
            MonsForSprites[i].Attach(Sprites[i]);
            TheGame.BotTeam[i].Position = "Reserve";
            TheGame.OppTeam[i].Position = "Reserve";
        }
        TheGame.Turns = [ // Create empty entry for opening turn
            new()
            {
                TurnNo = 0,
                EventList = [new()]
            }
        ];
        CurrEvent = TheGame.Turns[0].EventList[0]; // Set current event to first in list
        TheGame.MonsBrought = [];
        TheGame.MonsSeen = [];
        /*for (int i = 0; i < 10; i++) // Attach target check listeners to current event
        {
            TargetsChecked.Add(new(i));
            TargetsChecked[i].Attach(CurrEvent);
        }*/
        Weights = Task.Run(CalcDamages).Result; // Run damage calculations for first stage of choosing openers
        CalcStrategy(); // Calculate strategic weights for second stage of choosing openers
        for (int i = 0; i < 6; i++) // Loop over bot's team
        {
            for (int j = 0; j < 4; j++) // Order mons by highest weighting (only top 4 needed for VGC, will account for other formats later)
            {
                if (OpenerMonNos[j] == -1 || Weights[i] > Weights[OpenerMonNos[j]])
                {
                    OpenerMonNos.Insert(j, i);
                    break;
                }
            }
        }
        OpenerMonNos.RemoveRange(4, OpenerMonNos.Count - 4); // Trim openers to only top 4 (will account for other formats later)
        for (int i = 0; i < 4; i++)
        {
            AvailablePokemon[i] = TheGame.BotTeam[OpenerMonNos[i]].Name; // Add selected mons to list of mons in battle
            NameToNo.Add(AvailablePokemon[i], OpenerMonNos[i]); // Add selected mons to NameToNo dictionary
            MonsForSprites[i].Name = AvailablePokemon[i]; // Set sprites to selected mons
            TheGame.MonsBrought.Add(OpenerMonNos[i]);
        }
        for (int i = 0; i < 2; i++) // Set opening mons in event model to top 2 selected mons
        {
            TheGame.Turns[0].BotStartMons[i] = OpenerMonNos[i];
        }
        for (int i = 0; i < 6; i++)
        {
            OpponentsPokemon[i] = "Opponent's " + TheGame.OppTeam[i].Name; // Add opponent's pokemon to opponent list
            AvailablePokemon[i + 4] = OpponentsPokemon[i]; // Add opponent's pokemon to list of mons in battle
            NameToNo.Add(OpponentsPokemon[i], i + 6); // Add opponent's pokemon to NameToNo dictionary
        }
        for (int i = 0; i < 2; i++)
        {
            OppSelect[i].Attach(MonsForSprites[i + 4], TheGame.Turns[0], NameToNo); // Attach opponent selector class to last 2 mons for sprites
        }
        UserMonModel.Attach(UserSprite); // Attach UserSprite image listener to user mon model
        CurrEvent.Attach(EventType); // Attach event type listener to current event
        if (TheGame.Gen < 9) AllOptions.FieldList[2] = "Hail"; // Replace snow with hail if gen is less than 9
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

    public AllOptionsModel AllOptions // All drop-down options
    {
        get => _allOptions;
        set
        {
            _allOptions = value;
            OnPropertyChanged();
        }
    }

    /* ----------------------------------------
    Weighting calculations for choosing openers
    ---------------------------------------- */

    private float[] _weights = new float[6];

    public float[] Weights // Opener weightings for each pokemon in bot's team
    {
        get => _weights;
        set
        {
            _weights = value;
            OnPropertyChanged();
        }
    }

    private List<int> _openerMonNos = [-1];

    public List<int> OpenerMonNos // The opponent's opening pokemon
    {
        get => _openerMonNos;
        set
        {
            _openerMonNos = value;
            TheGame.Turns[0].BotStartMons = _openerMonNos;
            TheGame.Turns[0].BotEndMons = _openerMonNos;
            OnPropertyChanged();
        }
    }

    public Dictionary<string, int> StratWeights = new(); // Contains strategic value of moves & abilities for deciding on an opener

    public async Task<float[]> CalcDamages() // Calculates damage portion of weightings
    {
        float[] weights = new float[6]; // Empty weights array
        Dictionary<string, int> botMonToNo = []; // Dictionary for placing weights in correct place in array
        ObservableCollection<PokemonModel> botPokemon = []; // Collection of bot's pokemon in server compatable format
        ObservableCollection<PokemonModel> oppPokemon = []; // Collection of opponent's pokemon in server compatable format
        for (int i = 0; i < 6; i++) // Add all mons to collections
        {
            botMonToNo.Add(TheGame.BotTeam[i].Name, i);
            botPokemon.Add(new PokemonModel(TheGame.Gen, TheGame.BotTeam[i]));
            oppPokemon.Add(new PokemonModel(TheGame.Gen, TheGame.OppTeam[i]));
        }
        CalcCallModel callData = new() // Collect all data together for calc
        {
            Gen = TheGame.Gen,
            BotMons = botPokemon,
            OppMons = oppPokemon,
            Field = new(
                TheGame.GameType,
                TheGame.CurrentArena
                )
        };
        string callString = JsonSerializer.Serialize(callData); // Serialise call data into string
        HttpClient client = new();
        List<CalcRespModel>? response = await client.GetFromJsonAsync<List<CalcRespModel>>($"http://{TheGame.ServerUrl}/calc?{callString}"); // Send data to server and await response
        if (response == null) return weights; // Return empty results on null response
        foreach (CalcRespModel result in response) // Loop over each calc
        {
            if (result.BotUser)
            {
                weights[botMonToNo[result.UserMon]] += ParseDamage(result.Damage); // For damage dealt, add percentage to weighting
            }
            else
            {
                weights[botMonToNo[result.TargetMon]] -= ParseDamage(result.Damage) / 2; // For damage recieved, subtract half damage from weighting
            }
        }
        return weights;
    }

    public static float ParseDamage(string input) // Parses damage string into percentage
    {
        string splitInput = input.Split(':')[1].Split('(')[1].Split(" - ")[0];
        if (Single.TryParse(splitInput, out float damage)) return damage; // If successful return damage value
        return 0; // Else return 0
    }

    public void CalcStrategy() // Looks at each pokemon's ability and moves and adds their strategic value to weightings
    {
        for (int currMon = 0; currMon < 6; currMon++)
        {
            if (StratWeights.ContainsKey(TheGame.BotTeam[currMon].Ability))
            {
                Weights[currMon] += StratWeights[TheGame.BotTeam[currMon].Ability] * 10;
            }
            if (StratWeights.ContainsKey(TheGame.BotTeam[currMon].Move1))
            {
                Weights[currMon] += StratWeights[TheGame.BotTeam[currMon].Move1] * 10;
            }
            if (StratWeights.ContainsKey(TheGame.BotTeam[currMon].Move2))
            {
                Weights[currMon] += StratWeights[TheGame.BotTeam[currMon].Move2] * 10;
            }
            if (StratWeights.ContainsKey(TheGame.BotTeam[currMon].Move3))
            {
                Weights[currMon] += StratWeights[TheGame.BotTeam[currMon].Move3] * 10;
            }
            if (StratWeights.ContainsKey(TheGame.BotTeam[currMon].Move4))
            {
                Weights[currMon] += StratWeights[TheGame.BotTeam[currMon].Move4] * 10;
            }
        }
    }

    /* -------------
    Handling sprites
    ------------- */

    private ObservableCollection<ImageListener> _sprites = [];

    public ObservableCollection<ImageListener> Sprites // Image listeners for pokemon sprites
    {
        get => _sprites;
        set
        {
            _sprites = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<TeamModel> MonsForSprites { get; set; } = []; // Team models for Sprites image listeners to watch

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
    
    private ObservableCollection<OppSelector> _oppSelect = [new(0), new(1)];

    public ObservableCollection<OppSelector> OppSelect // Opp selectors for selecting the opponents opening pokemon
    {
        get => _oppSelect;
        set
        {
            _oppSelect = value;
            OnPropertyChanged();
        }
    }

    public class OppSelector(int position) // Class to update opponent's opening mons in turn model
    {
        private string _monName = "";
        public string MonName // Name of selected mon
        {
            get => _monName;
            set
            {
                _monName = value;
                Update();
                OnPropertyChanged();
            }
        }
        private readonly int _position = position; // Whether it's in position 0 or 1
        private Dictionary<string, int> _nameToNo = []; // Copy of main NameToNo dictionary
        private List<TeamModel> _mons = []; // MonsForSprites used for updating displayed sprites
        private List<TurnModel> _turns = []; // Turn models to update
        public void Attach(TeamModel mon, TurnModel turn, Dictionary<string, int> nameToNo) // Attach relevant models for updating
        {
            _mons.Add(mon);
            _turns.Add(turn);
            _nameToNo = nameToNo;
        }
        private void Update() // Update the models
        {
            foreach (TeamModel mon in _mons)
            {
                mon.Name = MonName.Replace("Opponent's ", ""); // Remove Opponent's prefix in order to find correct sprite
            }
            foreach (TurnModel turn in _turns)
            {
                turn.OppStartMons[_position] = _nameToNo[MonName] - 6; // Set opponent's opening mons to new mon number
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

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

    /*private ObservableCollection<TargetSelectorModel> _targetsChecked = [];

    public ObservableCollection<TargetSelectorModel> TargetsChecked // TargetSelectors for updating target list in current event 
    {
        get => _targetsChecked;
        set
        {
            _targetsChecked = value;
            OnPropertyChanged();
        }
    }*/

    public class TargetImageModel(TargetModel target, ImageListener image)
    {
        public TargetModel Target = target;
        public ImageListener Image = image;
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
        TargetList.Add(new(new(NameToNo), new()));
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
        //foreach (TargetSelectorModel selector in TargetsChecked) selector.Detach(); // Detach all target selectors
        EventNumber++; // Increment event number
        CurrEvent.Clear(); // Detach event type listener
        TheGame.Turns[0].EventList.Add(new()); // Add new event model to turn model
        CurrEvent = TheGame.Turns[0].EventList[EventNumber]; // Maeke current event a copy of new event model
        //foreach (TargetSelectorModel selector in TargetsChecked) selector.Attach(CurrEvent); // Reattach target selectors
        TargetList = [];
        CurrEvent.Attach(EventType); // Attach event type listener
        UserMonName = ""; // Reset user mon name to clear sprite
    }
}