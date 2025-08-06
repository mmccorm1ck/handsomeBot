using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Permissions;
using HandsomeBot.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Avalonia.Markup.Xaml;
using DialogHostAvalonia;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace HandsomeBot.ViewModels;

public class OpenerPageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public OpenerPageViewModel()
    {
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        p.StartInfo.FileName = "cmd.exe";
        LoadTeams();
        HandleP();
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
            AvailablePokemon[i] = BotTeamInfo[OpenerMonNos[i]].Name;
            NameToNo.Add(AvailablePokemon[i], OpenerMonNos[i]);
        }
        for (int i = 0; i < 2; i++) Turn.BotStartMons[i] = OpenerMonNos[i];
    }

    public new event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Process p = new Process();

    public Dictionary<string, int> StratWeights = new(); // Contains strategic value of moves & abilities for deciding on an opener

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

    public Models.TurnModel Turn{get;set;} = new()
    {
        TurnNo=0
    };

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
            Turn.BotStartMons = _openerMonNos;
            Turn.BotEndMons = _openerMonNos;
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
                    Turn.OppStartMons[i] = NameToNo["Opponent's "+_oppOpener[i]];
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

    public string rootDir = System.AppDomain.CurrentDomain.BaseDirectory;

    private char _gen;

    public char Gen
    {
        get => _gen;
        set
        {
            _gen = value;
            OnPropertyChanged();
        }
    }

    public void LoadTeams() // Loads json files to get both teams
    {
        string oppTeamFileName = "Data/newOppTeam.json";
        string botTeamFileName = "Data/newBotTeam.json";
        string infoFileName = "Data/newGameInfo.json";
        string oppTeamJsonString = "";
        string botTeamJsonString = "";
        string infoJsonString = "";
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
        try{
            using (StreamReader sr = File.OpenText(infoFileName))
            {
                infoJsonString = sr.ReadToEnd();
                sr.Close();
            }
        }
        catch
        {
            return;
        }
        if (infoJsonString == "")
        {
            return;
        }
        ObservableCollection<Models.TeamModel> BotTeamInfoTemp = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<Models.TeamModel>>(botTeamJsonString)!;
        ObservableCollection<Models.TeamModel> OppTeamInfoTemp = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<Models.TeamModel>>(oppTeamJsonString)!;
        Models.GameModel GameInfoTemp = System.Text.Json.JsonSerializer.Deserialize<Models.GameModel>(infoJsonString)!;
        GameInfo.Format = GameInfoTemp.Format;
        Gen = GameInfo.Format[3];
        for (int i = 0; i < 6; i++)
        {
            AvailablePokemon[i+4]    = "Opponent's "+ OppTeamInfoTemp[i].Name;
            NameToNo.Add("Opponent's "+OppTeamInfoTemp[i].Name, i+4);
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
        int gameType = 0;
        if (GameInfo.Format.Contains("vgc"))
        {
            gameType = 1;
        }
        string stratFileName = "Data/stratWeights.json";
        string stratJsonString = "";
        try{
            using (StreamReader sr = File.OpenText(stratFileName))
            {
                stratJsonString = sr.ReadToEnd();
                sr.Close();
            }
        }
        catch
        {
            return;
        }
        if (stratJsonString == "")
        {
            return;
        }
        Dictionary<string, int>[]? temp = JsonConvert.DeserializeObject<Dictionary<string, int>[]>(stratJsonString);
        if (temp is not null) StratWeights = temp[gameType];
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

    public string EncodeStats(int deflt, EVIVModel statsInfo)
    {
        string output = "{";
        if (statsInfo.HP != deflt) {
            output += "#hp#:"+statsInfo.HP+",";
        }
        if (statsInfo.Atk != deflt) {
            output += "#atk#:"+statsInfo.Atk+",";
        }
        if (statsInfo.Def != deflt) {
            output += "#def#:"+statsInfo.Def+",";
        }
        if (statsInfo.SpA != deflt) {
            output += "#spa#:"+statsInfo.SpA+",";
        }
        if (statsInfo.SpD != deflt) {
            output += "#spd#:"+statsInfo.SpD+",";
        }
        return output.Trim(',') + "}";
    }

    public string EncodeMon(char gen, TeamModel monInfo)
    {
        string encodedMon = "{#gen#:"+gen+",#name#:#"+monInfo.Name+"#,#options#:{#level#:"+monInfo.Level;
        if (monInfo.Item != "None") {
            encodedMon += ",#item#:#" + monInfo.Item + "#";
        }
        if (monInfo.Gender != 'R') {
            encodedMon += ",#gender#:#" + monInfo.Gender + "#";
        }
        if (monInfo.Ability != "None") {
            encodedMon += ",#ability#:#" + monInfo.Ability + "#";
        }
        if (monInfo.Nature != "None") {
            encodedMon += ",#nature#:#" + monInfo.Nature + "#";
        }
        encodedMon += ",#evs#:" + EncodeStats(0, monInfo.EV) + ",#ivs#:" + EncodeStats(31, monInfo.IV);

        return (encodedMon + "}}").Replace(' ','_');
    }

    public void SendData(int mon, int opp, int m)
    {
        string encodedCalc = $"{Gen} {EncodeMon(Gen, BotTeamInfo[mon])} {EncodeMon(Gen, OppTeamInfo[opp])}";
            string move;
            switch(m)
            {
                case 0:
                    move = BotTeamInfo[mon].Move1;
                    break;
                case 1:
                    move = BotTeamInfo[mon].Move2;
                    break;
                case 2:
                    move = BotTeamInfo[mon].Move3;
                    break;
                default:
                    move = BotTeamInfo[mon].Move4;
                    break;
            }
            string toSend = "ts-node src/index.ts c "+encodedCalc+" {#gen#:"+Gen+",#name#:#"+move.Replace(' ','_')+"#,#options#:{}}";
            //Console.WriteLine(toSend);
            p.StandardInput.WriteLine(toSend);
    }

    public void HandleP()
    {
        bool running = false;
        char currStep = 'i';
        int currMon = 0;
        int currOpp = 0;
        int currMove = 0;
        float curWeight = 0;
        p.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                string temp = e.Data.ToString().Trim(' ','\t');
                //Console.WriteLine("Result: "+temp);
                if (running)
                {
                    if (temp.Contains("£stop"))
                    {
                        running = false;
                        if (currStep == 'i')
                        {
                            AllItems.Sort();
                            currStep = 'a';
                            p.StandardInput.WriteLine($"ts-node src/index.ts a {Gen}");
                        }
                        else if (currStep == 'a')
                        {
                            AllAbilities.Sort();
                            currStep = 'b';
                            SendData(0,0,0);
                        }
                        //Console.WriteLine("Stopped");
                        else 
                        {
                            if (currMove < 3) currMove++;
                            else if (currOpp < 5) {
                                currOpp++;
                                currMove = 0;
                                Weights[currMon] += curWeight;
                                curWeight = 0;
                            }
                            else if (currMon < 5)
                            {
                                Weights[currMon] += curWeight;
                                curWeight = 0;
                                currMon++;
                                currOpp = 0;
                                currMove = 0;
                            }
                            else {
                                Weights[currMon] += curWeight;
                                p.Close();
                                return;
                            }
                            SendData(currMon, currOpp, currMove);
                        }
                    }
                    else 
                    {
                        if (currStep == 'i') AllItems.Add(temp);
                        else if (currStep == 'a') AllAbilities.Add(temp);
                        else
                        {
                            float tempWeight = float.Parse(temp.Split(" - ")[0]);
                            if (tempWeight > curWeight) curWeight = tempWeight;
                        }
                    }
                }
                if (temp.Contains("£start"))
                {
                    running = true;
                    //Console.WriteLine("Running");
                }
            }
        });
        p.Start();
        p.StandardInput.WriteLine($"cd {rootDir}Javascript");
        p.BeginOutputReadLine();
        p.StandardInput.WriteLine($"ts-node src/index.ts i {Gen}");
        p.WaitForExit();
    }

    public void CalcStrategy() 
    {
        for (int currMon = 0; currMon < 6; currMon++)
        {
            if (StratWeights.ContainsKey(BotTeamInfo[currMon].Ability) )
            {
                Weights[currMon] += StratWeights[BotTeamInfo[currMon].Ability] * 10;
            }
            if (StratWeights.ContainsKey(BotTeamInfo[currMon].Move1) )
            {
                Weights[currMon] += StratWeights[BotTeamInfo[currMon].Move1] * 10;
            }
            if (StratWeights.ContainsKey(BotTeamInfo[currMon].Move2) )
            {
                Weights[currMon] += StratWeights[BotTeamInfo[currMon].Move2] * 10;
            }
            if (StratWeights.ContainsKey(BotTeamInfo[currMon].Move3) )
            {
                Weights[currMon] += StratWeights[BotTeamInfo[currMon].Move3] * 10;
            }
            if (StratWeights.ContainsKey(BotTeamInfo[currMon].Move4) )
            {
                Weights[currMon] += StratWeights[BotTeamInfo[currMon].Move4] * 10;
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
        if (Turn.OppEndMons[0] == -1 || Turn.OppEndMons[1] == -1) Turn.OppEndMons = Turn.OppStartMons;
        if (Turn.BotEndMons[0] == -1 || Turn.BotEndMons[1] == -1) Turn.BotEndMons = Turn.BotStartMons;
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
                int i = Turn.BotEndMons.FindIndex(search.MonMatch);
                Console.WriteLine(i);
                if (i == -1) return;
                Turn.BotEndMons[i] = CurrEvent.TargetMons[0];
            } else 
            {
                int i = Turn.OppEndMons.FindIndex(search.MonMatch);
                if (i == -1) return;
                Turn.OppEndMons[i] = CurrEvent.TargetMons[0];
            }
        }
        Turn.EventList.Add(new());
        Turn.EventList[EventNumber] = CurrEvent; 
        string historyFileName = "Data/gameHistory.json";
        var options = new JsonSerializerOptions {WriteIndented = true};
        using (StreamWriter sw = File.CreateText(historyFileName))
        {
            string historyJsonString = System.Text.Json.JsonSerializer.Serialize(Turn, options);
            sw.Write(historyJsonString);
            sw.Close();
        }
        UserMonName = "";
        CurrEvent = new();
        EventNumber++;
    }
}