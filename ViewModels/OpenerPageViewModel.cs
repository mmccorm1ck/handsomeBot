
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.IO;
using System.Collections.ObjectModel;

namespace HandsomeBot.ViewModels;

public class OpenerPageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public OpenerPageViewModel()
    {
        LoadTeams();
    }

    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

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

    public Models.GameModel GameInfo{get;set;} = new()
    {
        Format = "",
        BotTeamURL = "",
        OppTeamURL = ""
    };
    private int _eventNumber = 1; // Tracks event number in chain of events

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
    private string[] _availablePokemon = new string[12]; // List of pokemon that can trigger events;
    private string[] _availableEvents = [ // List of possible events in turn 0
        "Ability Activation",
        "Item Activation",
        "Item Reveal"
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

    public void LoadTeams() // Loads json files to get both teams
    {
        string oppTeamFileName = "Data/newOppTeam.json";
        string botTeamFileName = "Data/newBotTeam.json";
        string oppTeamJsonString = "";
        string botTeamJsonString = "";
        try{
            using (StreamReader sr = File.OpenText(botTeamFileName))
            {
                botTeamJsonString = sr.ReadToEnd();
                sr.Close();
            }
        }
        catch
        {
            return;
        }
        if (botTeamJsonString == "")
        {
            return;
        }
        try{
            using (StreamReader sr = File.OpenText(oppTeamFileName))
            {
                oppTeamJsonString = sr.ReadToEnd();
                sr.Close();
            }
        }
        catch
        {
            return;
        }
        if (oppTeamJsonString == "")
        {
            return;
        }
        ObservableCollection<Models.TeamModel> BotTeamInfoTemp = JsonSerializer.Deserialize<ObservableCollection<Models.TeamModel>>(botTeamJsonString)!;
        ObservableCollection<Models.TeamModel> OppTeamInfoTemp = JsonSerializer.Deserialize<ObservableCollection<Models.TeamModel>>(oppTeamJsonString)!;
        for (int i = 0; i < 6; i++)
        {
            AvailablePokemon[i]      = BotTeamInfoTemp[i].Name;
            AvailablePokemon[i+6]    = "Opponent's "+ OppTeamInfoTemp[i].Name;
            OpponentsPokemon[i]      = OppTeamInfoTemp[i].Name;
            BotTeamInfo[i].Name      = BotTeamInfoTemp[i].Name;
            BotTeamInfo[i].Gender    = BotTeamInfoTemp[i].Gender;
            BotTeamInfo[i].Item      = BotTeamInfoTemp[i].Item;
            BotTeamInfo[i].Level     = BotTeamInfoTemp[i].Level;
            BotTeamInfo[i].Ability   = BotTeamInfoTemp[i].Ability;
            BotTeamInfo[i].Nature    = BotTeamInfoTemp[i].Nature;
            BotTeamInfo[i].EV        = BotTeamInfoTemp[i].EV;
            BotTeamInfo[i].IV        = BotTeamInfoTemp[i].IV;
            BotTeamInfo[i].Tera      = BotTeamInfoTemp[i].Tera;
            BotTeamInfo[i].Move1     = BotTeamInfoTemp[i].Move1;
            BotTeamInfo[i].Move2     = BotTeamInfoTemp[i].Move2;
            BotTeamInfo[i].Move3     = BotTeamInfoTemp[i].Move3;
            BotTeamInfo[i].Move4     = BotTeamInfoTemp[i].Move4;
            BotTeamInfo[i].PokeImage = BotTeamInfoTemp[i].PokeImage;
            OppTeamInfo[i].Name      = OppTeamInfoTemp[i].Name;
            OppTeamInfo[i].Gender    = OppTeamInfoTemp[i].Gender;
            OppTeamInfo[i].Item      = OppTeamInfoTemp[i].Item;
            OppTeamInfo[i].Level     = OppTeamInfoTemp[i].Level;
            OppTeamInfo[i].Ability   = OppTeamInfoTemp[i].Ability;
            OppTeamInfo[i].Nature    = OppTeamInfoTemp[i].Nature;
            OppTeamInfo[i].EV        = OppTeamInfoTemp[i].EV;
            OppTeamInfo[i].IV        = OppTeamInfoTemp[i].IV;
            OppTeamInfo[i].Tera      = OppTeamInfoTemp[i].Tera;
            OppTeamInfo[i].Move1     = OppTeamInfoTemp[i].Move1;
            OppTeamInfo[i].Move2     = OppTeamInfoTemp[i].Move2;
            OppTeamInfo[i].Move3     = OppTeamInfoTemp[i].Move3;
            OppTeamInfo[i].Move4     = OppTeamInfoTemp[i].Move4;
            OppTeamInfo[i].PokeImage = OppTeamInfoTemp[i].PokeImage;
        }
    }
}
