using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.IO;
using System.Linq;
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
        TheGame.Turns = new() {
            new()
            {
                TurnNo = 0
            }
        };
        CalcDamages();
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
        OpenerMonNos.RemoveRange(4, OpenerMonNos.Count-4);
        for (int i = 0; i < 4; i++)
        {
            AvailablePokemon[i] = TheGame.BotTeam[OpenerMonNos[i]].Name;
            NameToNo.Add(AvailablePokemon[i], OpenerMonNos[i]);
        }
        for (int i = 0; i < 2; i++) TheGame.Turns[0].BotStartMons[i] = OpenerMonNos[i];
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

    public Dictionary<string, int> StratWeights = new(); // Contains strategic value of moves & abilities for deciding on an opener

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

    private string[] _oppOpener = ["", ""];

    public string[] OppOpener
    {
        get => _oppOpener;
        set
        {
            _oppOpener = value;
            for (int i = 0; i < 2; i++)
            {
                if (!_oppOpener[i].Equals(""))
                {
                    TheGame.Turns[0].OppStartMons[i] = NameToNo["Opponent's "+_oppOpener[i]];
                }
            }
            OnPropertyChanged();
        }
    }

    public Dictionary<string, int> NameToNo{get;set;} = new Dictionary<string, int>();

    private List<string> _allItems = new();

    private List<string> _allAbilities = new();

    public List<string> AllItems
    {
        get => _allItems;
        set
        {
            _allItems = value;
            OnPropertyChanged();
        }
    }

    public List<string> AllAbilities
    {
        get => _allAbilities;
        set
        {
            _allAbilities = value;
            OnPropertyChanged();
        }
    }

    private Models.EventModel _currEvent = new();

    public Models.EventModel CurrEvent
    {
        get => _currEvent;
        set
        {
            _currEvent = value;
            OnPropertyChanged();
        }
    }

    private List<bool> _targetsChecked = Enumerable.Repeat(false, 10).ToList();

    public List<bool> TargetsChecked
    {
        get => _targetsChecked;
        set
        {
            _targetsChecked = value;
            OnPropertyChanged();
        }
    }

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

    private string _userMonName = "";

    public string UserMonName
    {
        get => _userMonName;
        set
        {
            _userMonName = value;
            if (value != "" && value != null) CurrEvent.UserMon = NameToNo[value];
            OnPropertyChanged();
        }
    }

    public async void CalcDamages()
    {
        ObservableCollection<PokemonModel> BotPokemon = [];
        ObservableCollection<PokemonModel> OppPokemon = [];
        for (int i = 0; i < 6; i++)
        {
            BotPokemon.Add(new PokemonModel(TheGame.Gen, TheGame.BotTeam[i]));
            OppPokemon.Add(new PokemonModel(TheGame.Gen, TheGame.OppTeam[i]));
        }
        CalcCallModel callData = new()
        {
            Gen = TheGame.Gen,
            BotMons = BotPokemon,
            OppMons = OppPokemon,
            Field = new(
                TheGame.GameType,
                TheGame.CurrentArena
                )
        };
        string callString = JsonSerializer.Serialize(callData);
        HttpClient client = new();
        List<CalcRespModel>? response = await client.GetFromJsonAsync<List<CalcRespModel>>($"http://{TheGame.ServerUrl}/calc?{callString}");
        if (response == null) return;
        foreach (CalcRespModel item in response)
        {
            Console.Write(JsonSerializer.Serialize(item));
        }
        foreach (CalcRespModel result in response)
        {
            if (result.BotUser)
            {
                Weights[result.UserMon] += result.DamageRange[0];
            }
            else
            {
                Weights[result.TargetMon] -= result.DamageRange[result.DamageRange.Count] / 2;
            }
        }
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
    }

    public void SaveEvent()
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
        string historyFileName = "Data/gameHistory.json";
        var options = new JsonSerializerOptions {WriteIndented = true};
        using (StreamWriter sw = File.CreateText(historyFileName))
        {
            string historyJsonString = System.Text.Json.JsonSerializer.Serialize(TheGame.Turns[0], options);
            sw.Write(historyJsonString);
            sw.Close();
        }
        UserMonName = "";
        CurrEvent = new();
        EventNumber++;
    }
}