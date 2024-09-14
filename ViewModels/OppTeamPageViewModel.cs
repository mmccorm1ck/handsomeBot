using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.IO;

namespace HandsomeBot.ViewModels;

public class OppTeamPageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public ObservableCollection<TeamModel> OppTeamInfo{get;set;} = new() // Initialize collection of pokemon to store info about bot team
    {
        new TeamModel
        {
            Name = "", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new TeamModel
        {
            Name = "", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new TeamModel
        {
            Name = "", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new TeamModel
        {
            Name = "", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new TeamModel
        {
            Name = "", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        },
        new TeamModel
        {
            Name = "", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "None",
            Tera = "None", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None", PokeImage = ""
        }
    };
    public void SaveTeam() // Saves BotTeamInfo to json file
    {   
        Debug.WriteLine(OppTeamInfo[0].Name);
        if (OppTeamInfo[0].Name == "Pokemon 1") {
            return;
        }
        string teamFileName = "Data/oppTeam.json";
        var options = new JsonSerializerOptions {WriteIndented = true};
        using (StreamWriter sw = File.CreateText(teamFileName))
        {
            string teamJsonString = JsonSerializer.Serialize(OppTeamInfo, options);
            sw.Write(teamJsonString);
            sw.Close();
        }
    }
    public void LoadTeam() // Loads json file into BotTeamInfo
    {
        string teamFileName = "Data/oppTeam.json";
        string teamJsonString = "";
        using (StreamReader sr = File.OpenText(teamFileName))
        {
            teamJsonString = sr.ReadToEnd();
            Debug.WriteLine(teamJsonString);
            sr.Close();
        }
        if (teamJsonString == "")
        {
            return;
        }
        ObservableCollection<TeamModel> OppTeamInfoTemp = JsonSerializer.Deserialize<ObservableCollection<TeamModel>>(teamJsonString)!;
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
        SaveTeam();
    }
}
