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
    public OpenerPageViewModel(GameModel game)
    {
        TheGame = game;
        for (int i = 0; i < 6; i++)
        {
            MonsForSprites.Add(new());
            Sprites.Add(new());
            MonsForSprites[i].Attach(Sprites[i]);
        }
        TheGame.Turns = [
            new()
            {
                TurnNo = 0,
                EventList = [new()]
            }
        ];
        CurrEvent = TheGame.Turns[0].EventList[0];
        for (int i = 0; i < 10; i++)
        {
            TargetsChecked.Add(new(i));
            TargetsChecked[i].Attach(CurrEvent);
        }
        Task.Run(GetAllAbilitiesItems);
        Weights = Task.Run(CalcDamages).Result;
        CalcStrategy();
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (OpenerMonNos[j] == -1 || Weights[i] > Weights[OpenerMonNos[j]])
                {
                    OpenerMonNos.Insert(j, i);
                    break;
                }
            }
        }
        OpenerMonNos.RemoveRange(4, OpenerMonNos.Count - 4);
        for (int i = 0; i < 4; i++)
        {
            AvailablePokemon[i] = TheGame.BotTeam[OpenerMonNos[i]].Name;
            NameToNo.Add(AvailablePokemon[i], OpenerMonNos[i]);
            MonsForSprites[i].Name = AvailablePokemon[i];
        }
        for (int i = 0; i < 2; i++)
        {
            TheGame.Turns[0].BotStartMons[i] = OpenerMonNos[i];
        }
        for (int i = 0; i < 6; i++)
        {
            OpponentsPokemon[i] = "Opponent's " + TheGame.OppTeam[i].Name;
            AvailablePokemon[i + 4] = OpponentsPokemon[i];
            NameToNo.Add(OpponentsPokemon[i], i + 6);
        }
        for (int i = 0; i < 2; i++)
        {
            OppSelect[i].Attach(MonsForSprites[i + 4]);
        }
        UserMonModel.Attach(UserSprite);
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

    public ObservableCollection<TeamModel> MonsForSprites { get; set; } = [];

    /* ----------------------------------------
    Weighting calculations for choosing openers
    ---------------------------------------- */

    private float[] _weights = new float[6];

    public float[] Weights
    {
        get => _weights;
        set
        {
            _weights = value;
            OnPropertyChanged();
        }
    }

    private List<int> _openerMonNos = [-1];

    public List<int> OpenerMonNos
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

    public async Task<float[]> CalcDamages()
    {
        float[] weights = new float[6];
        Dictionary<string, int> botMonToNo = [];
        ObservableCollection<PokemonModel> botPokemon = [];
        ObservableCollection<PokemonModel> oppPokemon = [];
        for (int i = 0; i < 6; i++)
        {
            botMonToNo.Add(TheGame.BotTeam[i].Name, i);
            botPokemon.Add(new PokemonModel(TheGame.Gen, TheGame.BotTeam[i]));
            oppPokemon.Add(new PokemonModel(TheGame.Gen, TheGame.OppTeam[i]));
        }
        CalcCallModel callData = new()
        {
            Gen = TheGame.Gen,
            BotMons = botPokemon,
            OppMons = oppPokemon,
            Field = new(
                TheGame.GameType,
                TheGame.CurrentArena
                )
        };
        string callString = JsonSerializer.Serialize(callData);
        HttpClient client = new();
        List<CalcRespModel>? response = await client.GetFromJsonAsync<List<CalcRespModel>>($"http://{TheGame.ServerUrl}/calc?{callString}");
        if (response == null) return weights;
        foreach (CalcRespModel result in response)
        {
            if (result.BotUser)
            {
                weights[botMonToNo[result.UserMon]] += ParseDamage(result.Damage);
            }
            else
            {
                weights[botMonToNo[result.TargetMon]] -= ParseDamage(result.Damage) / 2;
            }
        }
        return weights;
    }

    public static float ParseDamage(string input)
    {
        string splitInput = input.Split(':')[1].Split('(')[1].Split(" - ")[0];
        if (Single.TryParse(splitInput, out float damage)) return damage;
        return 0;
    }

    public void CalcStrategy()
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
    private string[] _availableEvents = [ // List of possible events in turn 0
        "Ability Activation",
        "Item Activation",
        "Ability Reveal",
        "Item Reveal",
        "Stat Change",
        "Forme Reveal",
        "Switch"
    ];
    public string[] AvailablePokemon
    {
    get => _availablePokemon;
        set
        {
            _availablePokemon = value;
            OnPropertyChanged();
        }
    }
    public string[] AvailableEvents
    {
        get => _availableEvents;
        set
        {
            _availableEvents = value;
            OnPropertyChanged();
        }
    }

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
            }
            else
            {
                CurrEvent.UserMon = -1;
                UserMonModel.Name = "None";
            }
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
    
    public Dictionary<string, int> NameToNo{get;set;} = [];

    private ObservableCollection<string> _allItems = [];

    public ObservableCollection<string> AllItems
    {
        get => _allItems;
        set
        {
            _allItems = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<string> _allAbilities = [];

    public ObservableCollection<string> AllAbilities
    {
        get => _allAbilities;
        set
        {
            _allAbilities = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<OppSelector> _oppSelect = [new(), new()];

    public ObservableCollection<OppSelector> OppSelect
    {
        get => _oppSelect;
        set
        {
            _oppSelect = value;
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

    public class OppSelector
    {
        private string _monName = "";
        public string MonName
        {
            get => _monName;
            set
            {
                _monName = value;
                Update();
                OnPropertyChanged();
            }
        }
        private List<TeamModel> mons = [];
        public void Attach(TeamModel model)
        {
            mons.Add(model);
        }
        private void Update()
        {
            foreach (TeamModel mon in mons)
            {
                mon.Name = MonName.Replace("Opponent's ", "");
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

    public void NextEvent()
    {
        foreach (TargetSelectorModel selector in TargetsChecked) selector.Detach();
        EventNumber++;
        TheGame.Turns[0].EventList.Add(new());
        CurrEvent = TheGame.Turns[0].EventList[EventNumber];
        foreach (TargetSelectorModel selector in TargetsChecked) selector.Attach(CurrEvent);
        UserMonName = "";
    }
    
    async public Task GetAllAbilitiesItems()
    {
        HttpClient client = new();
        string url = "http://" + TheGame.ServerUrl + "/abilities?{%22Gen%22:" + TheGame.Gen.ToString() + "}";
        string response = await client.GetStringAsync(url);
        if (response == null) return;
        ObservableCollection<string>? temp = JsonSerializer.Deserialize<ObservableCollection<string>>(response);
        if (temp == null) return;
        AllAbilities = temp;
        url = "http://" + TheGame.ServerUrl + "/items?{%22Gen%22:" + TheGame.Gen.ToString() + "}";
        response = await client.GetStringAsync(url);
        if (response == null) return;
        temp = JsonSerializer.Deserialize<ObservableCollection<string>>(response);
        if (temp == null) return;
        AllItems = temp;
    }

    /*public void SaveEvent()
    {
        if (UserMonName == "" || CurrEvent.EventType == "") return;
        if (TheGame.Turns[0].OppEndMons[0] == -1 || TheGame.Turns[0].OppEndMons[1] == -1) TheGame.Turns[0].OppEndMons = TheGame.Turns[0].OppStartMons;
        if (TheGame.Turns[0].BotEndMons[0] == -1 || TheGame.Turns[0].BotEndMons[1] == -1) TheGame.Turns[0].BotEndMons = TheGame.Turns[0].BotStartMons;
        CurrEvent.TargetMons = [];
        for (int i = 0; i < 10; i++)
        {
            if (TargetsChecked[i])
            {
                CurrEvent.TargetMons.Add(i);
                TargetsChecked[i] = false;
            }
        }
        if (!CurrEvent.EventType.Contains("Item"))
        {
            CurrEvent.ItemName = "";
        }
        if (!CurrEvent.EventType.Contains("Ability"))
        {
            CurrEvent.AbilityName = "";
        }
        if (CurrEvent.EventType.Contains("Switch"))
        {
            if (CurrEvent.TargetMons.Count != 1) return;
            var search = new MonSwitchSearch(CurrEvent.UserMon);
            if (CurrEvent.UserMon < 4)
            {
                int i = TheGame.Turns[0].BotEndMons.FindIndex(search.MonMatch);
                Console.WriteLine(i);
                if (i == -1) return;
                TheGame.Turns[0].BotEndMons[i] = CurrEvent.TargetMons[0];
            } else 
            {
                int i = TheGame.Turns[0].OppEndMons.FindIndex(search.MonMatch);
                if (i == -1) return;
                TheGame.Turns[0].OppEndMons[i] = CurrEvent.TargetMons[0];
            }
        }
        TheGame.Turns[0].EventList.Add(new());
        TheGame.Turns[0].EventList[EventNumber] = CurrEvent; 
        var options = new JsonSerializerOptions {WriteIndented = true};
        UserMonName = "";
        CurrEvent = new();
        EventNumber++;
    }

    public class MonSwitchSearch
    {
        int _mon;
        public MonSwitchSearch(int Mon)
        {
            _mon = Mon;
        }
        public bool MonMatch(int i)
        {
            return i == _mon;
        }
    }*/
}