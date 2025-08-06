using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Linq;

namespace HandsomeBot.ViewModels;

public class BattlePageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public BattlePageViewModel()
    {
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        p.StartInfo.FileName = "cmd.exe";
    }
    public new event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public Process p = new Process();
    
    public ObservableCollection<Models.TeamModel> BotTeamInfo{get;set;} = new() // Initialize collection of pokemon to store info about bot team
    {
        new Models.TeamModel
        {
            Name = "Pokemon 1", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "Pokemon 2", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "Pokemon 3", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "Pokemon 4", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "Pokemon 5", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "Pokemon 6", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        }
    };

    public ObservableCollection<Models.TeamModel> OppTeamInfo{get;set;} = new() // Initialize collection of pokemon to store info about bot team
    {
        new Models.TeamModel
        {
            Name = "Pokemon 1", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "Pokemon 2", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "Pokemon 3", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "Pokemon 4", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "Pokemon 5", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "Pokemon 6", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        }
    };

    public Models.GameModel GameInfo{get;set;} = new() {};

    public List<Models.EventModel> EventList{get;set;} = new();
    private int _eventNumber = 0; // Tracks event number in the chain of events

    public int EventNumber
    {
    get => _eventNumber;
        set
        {
            _eventNumber = value;
            OnPropertyChanged();
        }
    }

    private string[] _availablePokemon = new string[10];
    private string[] _availableEvents = [ // List of possible events
        "Move",
        "Switch",
        "KO",
        "Ability Activation",
        "Ability Change",
        "Ability Reveal",
        "Item Activation",
        "Item Reveal",
        "Item Change",
        "Status Change",
        "Status Activation",
        "Stat Level Change",
        "Forme Reveal",
        "Terastallize",
        "Mega Evolution",
        "Dynamax",
        "Gigantamax",
        "Z-Move"
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
}