using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.IO;
using Avalonia.Controls;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.MarkupExtensions;
using DynamicData;
using System.Linq;
using System.Collections.Generic;
using HandsomeBot.Models;
using DynamicData.Binding;

namespace HandsomeBot.ViewModels;

public class OppTeamPageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public OppTeamPageViewModel(GameModel game)
    {
        TheGame = game;
        Task.Run(async () => await GetAllMons());
        /*LoadInfo();
        bool running = false;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        p.StartInfo.FileName = "cmd.exe";
        p.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                string temp = e.Data.ToString().Trim(' ','\t');
                //Console.WriteLine(temp);
                if (running)
                {
                    if (temp.Contains("£stop"))
                    {
                        running = false;
                        //Console.WriteLine("Stopped");
                        p.Close();
                    }
                    else AllMons.Add(temp);
                }
                if (temp.Contains("£start"))
                {
                    running = true;
                    //Console.WriteLine("Running");
                }
            }
        });
        p.Start();
        //Console.WriteLine("Starting Process");
        p.StandardInput.WriteLine($"cd {rootDir}Javascript");
        p.BeginOutputReadLine();
        p.StandardInput.WriteLine($"ts-node src/index.ts l {Gen}");
        p.WaitForExit();*/
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

    /*public Process p = new Process();

    public string rootDir = System.AppDomain.CurrentDomain.BaseDirectory;

    public string teamFileName = "Data/newOppTeam.json";
    
    public string prevTeamFileName = "Data/oppTeam.json";
    
    public string infoFileName = "Data/newGameInfo.json";
    
    public ObservableCollection<Models.TeamModel> OppTeamInfo{get;set;} = new() // Initialize collection of pokemon to store info about bot team
    {
        new Models.TeamModel
        {
            Name = "", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new Models.TeamModel
        {
            Name = "", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        }
    };*/
    private ObservableCollection<string> _allMons = new ObservableCollection<string>();
    public ObservableCollection<string> AllMons
    {
        get => _allMons;
        set
        {
            _allMons = value;
            OnPropertyChanged();
        }
    }
    /*private bool _teamEditable = true;
    public bool TeamEditable
    {
        get => _teamEditable;
        set
        {
            _teamEditable = value;
            OnPropertyChanged();
        }
    }
    private bool _teamLoadable = true;
    public bool TeamLoadable
    {
        get => _teamLoadable;
        set
        {
            _teamLoadable = value;
            OnPropertyChanged();
        }
    }
    private string _loadButtonLabel = "Load Previous Team";
    public string LoadButtonLabel
    {
        get => _loadButtonLabel;
        set
        {
            _loadButtonLabel = value;
            OnPropertyChanged();
        }
    }
    private string _saveButtonLabel = "Save Team";
    public string SaveButtonLabel
    {
        get => _saveButtonLabel;
        set
        {
            _saveButtonLabel = value;
            OnPropertyChanged();
        }
    }

    private char _gen;

    public char Gen
    {
        get => _gen;
        set
        {
            _gen = value;
            OnPropertyChanged();
        }
    }*/

    public void LoadPrev()
    {
        /*string infoJsonString = "";
        try
        {
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
        Gen = JsonSerializer.Deserialize<Models.GameModel>(infoJsonString)!.Format[3];*/
        TheGame.OppTeam = TheGame.OppTeamPrev;
    }

    async public Task GetAllMons()
    {
        HttpClient client = new HttpClient();
        string url = "http://" + TheGame.ServerUrl + "/mons?{%22gen%22:" + TheGame.Gen.ToString() + "}";
        string response = "";
        try
        {
            response = await client.GetStringAsync(url);
        }
        catch
        {
            return;
        }
        if (response == "") return;
        Dictionary<string, object>? mons = JsonSerializer.Deserialize<Dictionary<string, object>>(response);
        if (mons == null) return;
        ObservableCollection<string> temp = [];
        foreach (string mon in mons.Keys) temp.Add(mon);
        AllMons = temp;
    }

    /*public void SaveTeam() // Saves BotTeamInfo to json file
    {   
        for (int i = 0; i < 6; i++)
        {
            if (OppTeamInfo[i].Name == "" || OppTeamInfo[i].Name == null)
            {
                return;
            }
        }
        //Debug.WriteLine(OppTeamInfo[0].Name);
        var options = new JsonSerializerOptions {WriteIndented = true};
        using (StreamWriter sw = File.CreateText(teamFileName))
        {
            string teamJsonString = JsonSerializer.Serialize(OppTeamInfo, options);
            sw.Write(teamJsonString);
            sw.Close();
        }
        TeamEditable = false;
        TeamLoadable = false;
        SaveButtonLabel = "Edit Team";
    }
    public void UnsaveTeam()
    {
        File.Delete(teamFileName);
        TeamEditable = true;
        TeamLoadable = true;
        SaveButtonLabel = "Save Team";
    }
    public void ToggleSave()
    {
        if (TeamLoadable)
        {
            SaveTeam();
        } else
        {
            UnsaveTeam();
        }
    }
    public void LoadTeam() // Loads json file into BotTeamInfo
    {
        if (!TeamEditable)
        {
            return;
        }
        string teamJsonString = "";
        try
        {
            using (StreamReader sr = File.OpenText(prevTeamFileName))
            {
                teamJsonString = sr.ReadToEnd();
                //Debug.WriteLine(teamJsonString);
                sr.Close();
            }
        }
        catch
        {
            return;
        }
        if (teamJsonString == "")
        {
            return;
        }
        ObservableCollection<Models.TeamModel> OppTeamInfoTemp = JsonSerializer.Deserialize<ObservableCollection<Models.TeamModel>>(teamJsonString)!;
        for (int i = 0; i < 6; i++)
        {
            OppTeamInfo[i].Name = OppTeamInfoTemp[i].Name;
            OppTeamInfo[i].Gender = OppTeamInfoTemp[i].Gender;
            OppTeamInfo[i].Item = OppTeamInfoTemp[i].Item;
            OppTeamInfo[i].Level = OppTeamInfoTemp[i].Level;
            OppTeamInfo[i].Ability = OppTeamInfoTemp[i].Ability;
            OppTeamInfo[i].Nature = OppTeamInfoTemp[i].Nature;
            OppTeamInfo[i].EV = OppTeamInfoTemp[i].EV;
            OppTeamInfo[i].IV = OppTeamInfoTemp[i].IV;
            OppTeamInfo[i].Tera = OppTeamInfoTemp[i].Tera;
            OppTeamInfo[i].Move1 = OppTeamInfoTemp[i].Move1;
            OppTeamInfo[i].Move2 = OppTeamInfoTemp[i].Move2;
            OppTeamInfo[i].Move3 = OppTeamInfoTemp[i].Move3;
            OppTeamInfo[i].Move4 = OppTeamInfoTemp[i].Move4;
            OppTeamInfo[i].PokeImage = OppTeamInfoTemp[i].PokeImage;
        }
        TeamEditable = false;
        LoadButtonLabel = "Change Team";
    }
    public void UnloadTeam()
    {
        if (TeamEditable)
        {
            return;
        }
        for (int i = 0; i < 6; i++)
        {
            OppTeamInfo[i].Gender = 'R';
            OppTeamInfo[i].Item = "None";
            OppTeamInfo[i].Level = 50;
            OppTeamInfo[i].Ability = "None";
            OppTeamInfo[i].Nature = "None";
            OppTeamInfo[i].Tera = "None";
            OppTeamInfo[i].Move1 = "None";
            OppTeamInfo[i].Move2 = "None";
            OppTeamInfo[i].Move3 = "None";
            OppTeamInfo[i].Move4 = "None";
            OppTeamInfo[i].PokeImage = "";
        }
        TeamEditable = true;
        LoadButtonLabel = "Load Previous Team";
    }
    public void ToggleLoad()
    {
        if (TeamEditable)
        {
            LoadTeam();
        } else
        {
            UnloadTeam();
        }
    }*/
}
