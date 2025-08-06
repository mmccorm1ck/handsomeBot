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

public class BotTeamPageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public BotTeamPageViewModel()
    {
        File.Delete("Data/newBotTeam.json");
        File.Delete("Data/newOppTeam.json");
        File.Delete("Data/newGameInfo.json");
    }
    public new event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

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

    public Models.GameModel GameInfo{get;set;} = new() {};

    public void SaveTeam() // Saves BotTeamInfo to json file
    {   
        //Debug.WriteLine(BotTeamInfo[0].Name);
        if (BotTeamInfo[0].Name == "Pokemon 1") {
            return;
        }
        string teamFileName = "Data/newBotTeam.json";
        string infoFileName = "Data/newGameInfo.json";
        var options = new JsonSerializerOptions {WriteIndented = true};
        using (StreamWriter sw = File.CreateText(teamFileName))
        {
            string teamJsonString = JsonSerializer.Serialize(BotTeamInfo, options);
            sw.Write(teamJsonString);
            sw.Close();
        }
        using (StreamWriter sw = File.CreateText(infoFileName))
        {
            string infoJsonString = JsonSerializer.Serialize(GameInfo, options);
            sw.Write(infoJsonString);
            sw.Close();
        }
    }

    public void LoadTeam() // Loads json file into BotTeamInfo
    {
        string teamFileName = "Data/botTeam.json";
        string infoFileName = "Data/gameInfo.json";
        string teamJsonString = "";
        string infoJsonString = "";
        try{
            using (StreamReader sr = File.OpenText(teamFileName))
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
        ObservableCollection<Models.TeamModel> BotTeamInfoTemp = JsonSerializer.Deserialize<ObservableCollection<Models.TeamModel>>(teamJsonString)!;
        using (StreamReader sr = File.OpenText(infoFileName))
        {
            infoJsonString = sr.ReadToEnd();
            //Debug.WriteLine(infoJsonString);
            sr.Close();
        }
        if (infoJsonString == "")
        {
            return;
        }
        Models.GameModel GameInfoTemp = JsonSerializer.Deserialize<Models.GameModel>(infoJsonString)!;
        for (int i = 0; i < 6; i++)
        {
            BotTeamInfo[i].Name = BotTeamInfoTemp[i].Name;
            BotTeamInfo[i].Gender = BotTeamInfoTemp[i].Gender;
            BotTeamInfo[i].Item = BotTeamInfoTemp[i].Item;
            BotTeamInfo[i].Level = BotTeamInfoTemp[i].Level;
            BotTeamInfo[i].Ability = BotTeamInfoTemp[i].Ability;
            BotTeamInfo[i].Nature = BotTeamInfoTemp[i].Nature;
            BotTeamInfo[i].EV = BotTeamInfoTemp[i].EV;
            BotTeamInfo[i].IV = BotTeamInfoTemp[i].IV;
            BotTeamInfo[i].Tera = BotTeamInfoTemp[i].Tera;
            BotTeamInfo[i].Move1 = BotTeamInfoTemp[i].Move1;
            BotTeamInfo[i].Move2 = BotTeamInfoTemp[i].Move2;
            BotTeamInfo[i].Move3 = BotTeamInfoTemp[i].Move3;
            BotTeamInfo[i].Move4 = BotTeamInfoTemp[i].Move4;
            BotTeamInfo[i].PokeImage = BotTeamInfoTemp[i].PokeImage;
        }
        GameInfo.Format = GameInfoTemp.Format;
        GameInfo.BotTeamURL = GameInfoTemp.BotTeamURL;
        SaveTeam();
    }

    public void LoadPaste() // Triggered by load button on UI, calls async task to load team info
    {
        if (GameInfo.BotTeamURL == "")
        {
            return;
        }
        //Debug.WriteLine(GameInfo.BotTeamURL);
        Task task = Task.Run(async () => await LoadPasteHtml(GameInfo.BotTeamURL));
    }
    
    public async Task LoadPasteHtml(string httpLink) // Parses HTML from pokepaste link and stores info in BotTeamInfo
    {
        HttpClient client = new HttpClient();
        string response = await client.GetStringAsync(httpLink);
        string[] responses = response.Split('\n');
        int currPokemon = -1;
        int currMove = -1;
        for (int i = 0; i < responses.Length-1; i++) {
            responses[i] = responses[i].Trim(' ','\t');
            //Debug.WriteLine(i.ToString()+responses[i]);
            if (responses[i].Contains("<article>")) { // Detects splits between each pokemon
                currPokemon++;
                continue;
            }
            if (currPokemon < 0) { // If not yet reached pokemon info in pokepaste
                continue;
            }
            if (responses[i].Contains("Format")) { // Saves format of pokepaste
                GameInfo.Format = responses[i].Split(' ')[1][0..^4];
                //Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("img-pokemon")) { // Saves URL of pokemon image
                BotTeamInfo[currPokemon].PokeImage = "https://pokepast.es" + responses[i].Split(' ')[2][5..^2];
                //Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("Nature")) { // Saves pokemon's nature
                BotTeamInfo[currPokemon].Nature = responses[i].Split(' ')[0];
                currMove = 0; // Prepares to load moves
                //Debug.WriteLine(i);
                continue;
            }
            if (currMove > -1) { // If loading moves
                if (responses[i] == "") { // If last move has been read
                    currMove = -1;
                    continue;
                }
                int idx;
                if (responses[i].Contains("IVs")) { // If there are IVs to be read
                    string[] temps = responses[i][31..].Split(" / ");
                    for (int j = 0; j < temps.Length; j++)
                    {
                        temps[j] = temps[j][18..^7];
                        string[] temp = temps[j].Split(" ");
                        idx = temp[0].LastIndexOf('>') + 1;
                        temp[0] = temp[0][idx..];
                        switch(temp[1])
                        {
                            case "HP":
                                BotTeamInfo[currPokemon].IV.HP = Int32.Parse(temp[0]);
                                break;
                            case "Atk":
                                BotTeamInfo[currPokemon].IV.Atk = Int32.Parse(temp[0]);
                                break;
                            case "Def":
                                BotTeamInfo[currPokemon].IV.Def = Int32.Parse(temp[0]);
                                break;
                            case "SpA":
                                BotTeamInfo[currPokemon].IV.SpA = Int32.Parse(temp[0]);
                                break;
                            case "SpD":
                                BotTeamInfo[currPokemon].IV.SpD = Int32.Parse(temp[0]);
                                break;
                            case "Spe":
                                BotTeamInfo[currPokemon].IV.Spe = Int32.Parse(temp[0]);
                                break;
                        }
                    }
                    //Debug.WriteLine(i);
                    continue;
                }
                idx = responses[i].LastIndexOf('>') + 1;
                currMove++; // Increments to next move
                switch(currMove)
                {
                    case 1:
                        BotTeamInfo[currPokemon].Move1 = responses[i][idx..].TrimStart([' ','-']);
                        break;
                    case 2:
                        BotTeamInfo[currPokemon].Move2 = responses[i][idx..].TrimStart([' ','-']);
                        break;
                    case 3:
                        BotTeamInfo[currPokemon].Move3 = responses[i][idx..].TrimStart([' ','-']);
                        break;
                    case 4:
                        BotTeamInfo[currPokemon].Move4 = responses[i][idx..].TrimStart([' ','-']);
                        currMove = -1;
                        break;
                }
                //Debug.WriteLine(i);
                continue;
            }
            if (!responses[i].Contains("<span")) { // If line has no relevent info
                continue;
            }
            if (responses[i].Contains("<pre>")) { // Detects line that contains pokemon name, gender and item
                int idx = responses[i].LastIndexOf('@') + 2;
                if (idx != 1) { // If there is an item
                    string item = responses[i][idx..];
                    if (item.Contains("<span")) {
                        item = item[0..^7];
                        int idxtemp = item.LastIndexOf('>');
                        item = item[idxtemp..];
                    }
                    BotTeamInfo[currPokemon].Item = item;
                }
                if (responses[i].Contains("gender")) { // If there is a specified gender
                    idx = responses[i].LastIndexOf("gender")+10;
                    BotTeamInfo[currPokemon].Gender = responses[i][idx];
                }
                responses[i] = responses[i].Split("</span>")[0];
                idx = responses[i].LastIndexOf(">") + 1;
                BotTeamInfo[currPokemon].Name = responses[i][idx..];
            }
            if (responses[i].Contains("Ability")) { // Saves pokemon's ability
                int idx = responses[i].LastIndexOf('>') + 1;
                BotTeamInfo[currPokemon].Ability = responses[i][idx..];
                //Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("Level")) { // Saves pokemon's level
                int idx = responses[i].LastIndexOf('>') + 1;
                BotTeamInfo[currPokemon].Level = Int32.Parse(responses[i][idx..]);
                //Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("Tera Type")) { // Saves pokemon's tera type
                responses[i] = responses[i][0..^7];
                int idx = responses[i].LastIndexOf('>') + 1;
                BotTeamInfo[currPokemon].Tera = responses[i][idx..];
                //Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("EVs")) { // Saves pokemon's EVs
                string[] temps = responses[i][31..].Split(" / ");
                for (int j = 0; j < temps.Length; j++)
                {
                    temps[j] = temps[j][18..^7];
                    string[] temp = temps[j].Split(" ");
                    int idx = temp[0].LastIndexOf('>') + 1;
                    temp[0] = temp[0][idx..];
                    switch(temp[1])
                    {
                        case "HP":
                            BotTeamInfo[currPokemon].EV.HP = Int32.Parse(temp[0]);
                            break;
                        case "Atk":
                            BotTeamInfo[currPokemon].EV.Atk = Int32.Parse(temp[0]);
                            break;
                        case "Def":
                            BotTeamInfo[currPokemon].EV.Def = Int32.Parse(temp[0]);
                            break;
                        case "SpA":
                            BotTeamInfo[currPokemon].EV.SpA = Int32.Parse(temp[0]);
                            break;
                        case "SpD":
                            BotTeamInfo[currPokemon].EV.SpD = Int32.Parse(temp[0]);
                            break;
                        case "Spe":
                            BotTeamInfo[currPokemon].EV.Spe = Int32.Parse(temp[0]);
                            break;
                    }
                }
                //Debug.WriteLine(i);
                continue;
            }

        }
        SaveTeam();
    }
}
