using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia.Collections;
using ReactiveUI;
using System.Net.Mail;
using System.Collections.ObjectModel;
using System.Globalization;


namespace HandsomeBot.ViewModels;

public class BotTeamPageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    /*private AvaloniaDictionary<string, string>[] _botTeamDict = {
        new AvaloniaDictionary<string, string>(),
        new AvaloniaDictionary<string, string>(),
        new AvaloniaDictionary<string, string>(),
        new AvaloniaDictionary<string, string>(),
        new AvaloniaDictionary<string, string>(),
        new AvaloniaDictionary<string, string>()
    };
    public AvaloniaDictionary<string, string>[] botTeamDict
    {
        get => _botTeamDict;
        set
        {
            _botTeamDict = value;
            OnPropertyChanged();
        }
    }*/

    public ObservableCollection<TeamModel> BotTeamInfo{get;set;} = new() // Initialize collection of pokemon to store info about bot team
    {
        new TeamModel
        {
            Name = "Pokemon 1", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "Bashful",
            EV = evInit, IV = ivInit, Tera = "Normal", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None",
            PokeImage = ""
        },
        new TeamModel
        {
            Name = "Pokemon 2", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "Bashful",
            EV = evInit, IV = ivInit, Tera = "Normal", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None",
            PokeImage = ""
        },
        new TeamModel
        {
            Name = "Pokemon 3", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "Bashful",
            EV = evInit, IV = ivInit, Tera = "Normal", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None",
            PokeImage = ""
        },
        new TeamModel
        {
            Name = "Pokemon 4", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "Bashful",
            EV = evInit, IV = ivInit, Tera = "Normal", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None",
            PokeImage = ""
        },
        new TeamModel
        {
            Name = "Pokemon 5", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "Bashful",
            EV = evInit, IV = ivInit, Tera = "Normal", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None",
            PokeImage = ""
        },
        new TeamModel
        {
            Name = "Pokemon 6", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "Bashful",
            EV = evInit, IV = ivInit, Tera = "Normal", Move1 = "None", Move2 = "None", Move3 = "None", Move4 = "None",
            PokeImage = ""
        }

    };

    /*public class pokemonInfoTemplate
    {
        public pokemonInfoTemplate(string name, char gender, string item, int level, string ability, 
                                    string nature, int[] ev, int[] iv, string tera, string[] moves, string image)
        {
            Name = name;
            Gender = gender;
            Item = item;
            Level = level;
            Ability = ability;
            Nature = nature;
            EV = ev;
            IV = iv;
            Tera = tera;
            Moves = moves;
            PokeImage = image;
        }
        public string Name{get;set;}
        public char Gender {get;set;}
        public string Item {get;set;}
        public int Level {get;set;}
        public string Ability {get;set;}
        public string Nature {get;set;}
        public int[] EV {get;set;}
        public int[] IV {get;set;}
        public string Tera {get;set;}
        public string[] Moves {get;set;}
        public string PokeImage {get;set;}
    }*/

    private string _format = ""; // Format of the battle
    public string format
    {
        get => _format;
        set
        {
            _format = value;
            OnPropertyChanged();
        }
    }
    private string _pasteLink = ""; // Pokepaste URL to fetch team info from
    public string pasteLink
    {
        get => _pasteLink;
        set
        {
            _pasteLink = value;
            OnPropertyChanged();
        }
    }

    public static EVIVModel evInit = new EVIVModel()
    {
        HP = 0,
        Atk = 0,
        Def = 0,
        SpA = 0,
        SpD = 0,
        Spe = 0
    };
    public static EVIVModel ivInit = new EVIVModel()
    {
        HP = 31,
        Atk = 31,
        Def = 31,
        SpA = 31,
        SpD = 31,
        Spe = 31
    };

    public void LoadPaste() // Triggered by load button on UI, calls async task to load team info
    {
        if (pasteLink == "")
        {
            return;
        }
        Debug.WriteLine(pasteLink);
        Task task = Task.Run(async () => await LoadPasteHtml(pasteLink));
    }
    
    public async Task LoadPasteHtml(string httpLink) // Parses HTML from pokepaste link and stores info in BotTeamInfo
    {
        HttpClient client = new HttpClient();
        string response = await client.GetStringAsync(httpLink);
        string[] responses = response.Split('\n');
        for(int i = 0; i < responses.Length; i++) {
            Debug.WriteLine(i.ToString()+responses[i]);
        }
        int currPokemon = -1;
        int currMove = -1;
        for (int i = 0; i < responses.Length-1; i++) {
            responses[i] = responses[i].Trim(' ','\t');
            Debug.WriteLine(i.ToString()+responses[i]);
            if (responses[i].Contains("<article>")) { // Detects splits between each pokemon
                currPokemon++;
                continue;
            }
            if (currPokemon < 0) { // If not yet reached pokemon info in pokepaste
                continue;
            }
            if (responses[i].Contains("Format")) { // Saves format of pokepaste
                format = responses[i].Split(' ')[1][0..^4];
                Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("img-pokemon")) { // Saves URL of pokemon image
                BotTeamInfo[currPokemon].PokeImage = "https://pokepast.es" + responses[i].Split(' ')[2][5..^2];
                Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("Nature")) { // Saves pokemon's nature
                BotTeamInfo[currPokemon].Nature = responses[i].Split(' ')[0];
                currMove = 0; // Prepares to load moves
                Debug.WriteLine(i);
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
                    Debug.WriteLine(i);
                    continue;
                }
                idx = responses[i].LastIndexOf('>') + 1;
                //BotTeamInfo[currPokemon].Moves[currMove] = responses[i][idx..].TrimStart([' ','-']);
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
                Debug.WriteLine(i);
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
                Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("Level")) { // Saves pokemon's level
                int idx = responses[i].LastIndexOf('>') + 1;
                BotTeamInfo[currPokemon].Level = Int32.Parse(responses[i][idx..]);
                Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("Tera Type")) { // Saves pokemon's tera type
                responses[i] = responses[i][0..^7];
                int idx = responses[i].LastIndexOf('>') + 1;
                BotTeamInfo[currPokemon].Tera = responses[i][idx..];
                Debug.WriteLine(i);
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
                    Debug.WriteLine(temp[0]);
                    Debug.WriteLine(temp[1]);
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
                Debug.WriteLine(i);
                continue;
            }

        }
        OnPropertyChanged();
        Debug.WriteLine(format);
        
    }
}
