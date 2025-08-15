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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;

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
        /*p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        p.StartInfo.FileName = "cmd.exe";
        //LoadTeams();
        HandleP();*/
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

    //public Process p = new Process();

    public Dictionary<string, int> StratWeights = new(); // Contains strategic value of moves & abilities for deciding on an opener

    /*public ObservableCollection<Models.TeamModel> TheGame.BotTeam{get;set;} = new() // Initialize collection of pokemon to store info about bot team
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

    public ObservableCollection<Models.TeamModel> TheGame.OppTeam{get;set;} = new() // Initialize collection of pokemon to store info about bot team
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
    };*/

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

    //public string rootDir = System.AppDomain.CurrentDomain.BaseDirectory;

    /*private char _gen;

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
        ObservableCollection<Models.TeamModel> TheGame.BotTeamTemp = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<Models.TeamModel>>(botTeamJsonString)!;
        ObservableCollection<Models.TeamModel> TheGame.OppTeamTemp = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<Models.TeamModel>>(oppTeamJsonString)!;
        Models.GameModel GameInfoTemp = System.Text.Json.JsonSerializer.Deserialize<Models.GameModel>(infoJsonString)!;
        GameInfo.Format = GameInfoTemp.Format;
        Gen = GameInfo.Format[3];
        for (int i = 0; i < 6; i++)
        {
            AvailablePokemon[i+4]    = "Opponent's "+ TheGame.OppTeamTemp[i].Name;
            NameToNo.Add("Opponent's "+TheGame.OppTeamTemp[i].Name, i+4);
            OpponentsPokemon[i]      = TheGame.OppTeamTemp[i].Name;
            TheGame.BotTeam[i].Name      = BotTeamInfoTemp[i].Name;
            TheGame.BotTeam[i].Gender    = BotTeamInfoTemp[i].Gender;
            TheGame.BotTeam[i].Item      = BotTeamInfoTemp[i].Item;
            TheGame.BotTeam[i].Level     = BotTeamInfoTemp[i].Level;
            TheGame.BotTeam[i].Ability   = BotTeamInfoTemp[i].Ability;
            TheGame.BotTeam[i].Nature    = BotTeamInfoTemp[i].Nature;
            TheGame.BotTeam[i].EV        = BotTeamInfoTemp[i].EV;
            TheGame.BotTeam[i].IV        = BotTeamInfoTemp[i].IV;
            TheGame.BotTeam[i].Tera      = BotTeamInfoTemp[i].Tera;
            TheGame.BotTeam[i].Move1     = BotTeamInfoTemp[i].Move1;
            TheGame.BotTeam[i].Move2     = BotTeamInfoTemp[i].Move2;
            TheGame.BotTeam[i].Move3     = BotTeamInfoTemp[i].Move3;
            TheGame.BotTeam[i].Move4     = BotTeamInfoTemp[i].Move4;
            TheGame.BotTeam[i].PokeImage = BotTeamInfoTemp[i].PokeImage;
            TheGame.OppTeam[i].Name      = OppTeamInfoTemp[i].Name;
            TheGame.OppTeam[i].Gender    = OppTeamInfoTemp[i].Gender;
            TheGame.OppTeam[i].Item      = OppTeamInfoTemp[i].Item;
            TheGame.OppTeam[i].Level     = OppTeamInfoTemp[i].Level;
            TheGame.OppTeam[i].Ability   = OppTeamInfoTemp[i].Ability;
            TheGame.OppTeam[i].Nature    = OppTeamInfoTemp[i].Nature;
            TheGame.OppTeam[i].EV        = OppTeamInfoTemp[i].EV;
            TheGame.OppTeam[i].IV        = OppTeamInfoTemp[i].IV;
            TheGame.OppTeam[i].Tera      = OppTeamInfoTemp[i].Tera;
            TheGame.OppTeam[i].Move1     = OppTeamInfoTemp[i].Move1;
            TheGame.OppTeam[i].Move2     = OppTeamInfoTemp[i].Move2;
            TheGame.OppTeam[i].Move3     = OppTeamInfoTemp[i].Move3;
            TheGame.OppTeam[i].Move4     = OppTeamInfoTemp[i].Move4;
            TheGame.OppTeam[i].PokeImage = OppTeamInfoTemp[i].PokeImage;
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
    }*/

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

    /*public string EncodeStats(int deflt, EVIVModel statsInfo)
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

    public string EncodeMon(int gen, TeamModel monInfo)
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
        string encodedCalc = $"{TheGame.Gen} {EncodeMon(TheGame.Gen, TheGame.BotTeam[mon])} {EncodeMon(TheGame.Gen, TheGame.OppTeam[opp])}";
            string move;
            switch(m)
            {
                case 0:
                    move = TheGame.BotTeam[mon].Move1;
                    break;
                case 1:
                    move = TheGame.BotTeam[mon].Move2;
                    break;
                case 2:
                    move = TheGame.BotTeam[mon].Move3;
                    break;
                default:
                    move = TheGame.BotTeam[mon].Move4;
                    break;
            }
            string toSend = "ts-node src/index.ts c "+encodedCalc+" {#gen#:"+TheGame.Gen +",#name#:#"+move.Replace(' ','_')+"#,#options#:{}}";
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
                            p.StandardInput.WriteLine($"ts-node src/index.ts a {TheGame.Gen}");
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
        p.StandardInput.WriteLine($"ts-node src/index.ts i {TheGame.Gen}");
        p.WaitForExit();
    }*/

    public async void CalcDamages()
    {
        CalcCallModel callData = new()
        {
            Gen = TheGame.Gen,
            BotMons = TheGame.BotTeam,
            OppMons = TheGame.OppTeam
        };
        string callString = JsonSerializer.Serialize(callData);
        HttpClient client = new HttpClient();
        List<CalcRespModel>? response = await client.GetFromJsonAsync<List<CalcRespModel>>($"http://{TheGame.ServerUrl}/calc?{callString}");
        if (response == null) return;
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