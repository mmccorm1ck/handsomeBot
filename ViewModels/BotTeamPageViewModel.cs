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


namespace HandsomeBot.ViewModels;

public class BotTeamPageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
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

    public ObservableCollection<TeamModel> BotTeamInfo{get;set;} = new()
    {
        new TeamModel
        {
            Name = "Pokemon 1", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "Bashful",
            EV = [0,0,0,0,0,0], IV = [31,31,31,31,31,31], Tera = "Normal", Moves = ["None","None","None","None"],
            PokeImage = ""
        },
        new TeamModel
        {
            Name = "Pokemon 2", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "Bashful",
            EV = [0,0,0,0,0,0], IV = [31,31,31,31,31,31], Tera = "Normal", Moves = ["None","None","None","None"],
            PokeImage = ""
        },
        new TeamModel
        {
            Name = "Pokemon 3", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "Bashful",
            EV = [0,0,0,0,0,0], IV = [31,31,31,31,31,31], Tera = "Normal", Moves = ["None","None","None","None"],
            PokeImage = ""
        },
        new TeamModel
        {
            Name = "Pokemon 4", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "Bashful",
            EV = [0,0,0,0,0,0], IV = [31,31,31,31,31,31], Tera = "Normal", Moves = ["None","None","None","None"],
            PokeImage = ""
        },
        new TeamModel
        {
            Name = "Pokemon 5", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "Bashful",
            EV = [0,0,0,0,0,0], IV = [31,31,31,31,31,31], Tera = "Normal", Moves = ["None","None","None","None"],
            PokeImage = ""
        },
        new TeamModel
        {
            Name = "Pokemon 6", Gender = 'R', Item = "None", Level =  50, Ability = "None", Nature = "Bashful",
            EV = [0,0,0,0,0,0], IV = [31,31,31,31,31,31], Tera = "Normal", Moves = ["None","None","None","None"],
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

    private string _format = "";
    public string format
    {
        get => _format;
        set
        {
            _format = value;
            OnPropertyChanged();
        }
    }
    private string _pasteLink = "";
    public string pasteLink
    {
        get => _pasteLink;
        set
        {
            _pasteLink = value;
            OnPropertyChanged();
        }
    }
    public void LoadPaste()
    {
        if (pasteLink == "")
        {
            return;
        }
        Debug.WriteLine(pasteLink);
        Task task = Task.Run(async () => await LoadPasteHtml(pasteLink));
    }
    
    public async Task LoadPasteHtml(string httpLink)
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
            if (responses[i].Contains("<article>")) {
                currPokemon++;
                continue;
            }
            if (currPokemon < 0) {
                continue;
            }
            if (responses[i].Contains("Format")) {
                format = responses[i].Split(' ')[1][0..^4];
                Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("img-pokemon")) {
                BotTeamInfo[currPokemon].PokeImage = "https://pokepast.es" + responses[i].Split(' ')[2][5..^2];
                Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("Nature")) {
                BotTeamInfo[currPokemon].Nature = responses[i].Split(' ')[0];
                currMove = 0;
                Debug.WriteLine(i);
                continue;
            }
            if (currMove != -1) {
                if (responses[i] == "") {
                    currMove = -1;
                    continue;
                }
                int idx;
                if (responses[i].Contains("IVs")) {
                    responses[i] = responses[i][0..^7];
                    idx = responses[i].LastIndexOf('>') + 1;
                    //BotTeamInfo[currPokemon].IV = responses[i][idx..];
                    Debug.WriteLine(i);
                    continue;
                }
                idx = responses[i].LastIndexOf('>') + 1;
                //BotTeamInfo[currPokemon].Moves[currMove] = responses[i][idx..].TrimStart([' ','-']);
                currMove++;
                Debug.WriteLine(i);
                if (currMove > 3) {
                    currMove = -1;
                }
                continue;
            }
            if (!responses[i].Contains("<span")) {
                continue;
            }
            if (responses[i].Contains("<pre>")) {
                int idx = responses[i].LastIndexOf('@') + 2;
                if (idx != 1) {
                    string item = responses[i][idx..];
                    if (item.Contains("<span")) {
                        item = item[0..^7];
                        int idxtemp = item.LastIndexOf('>');
                        item = item[idxtemp..];
                    }
                    BotTeamInfo[currPokemon].Item = item;
                }
                if (responses[i].Contains("gender")) {
                    idx = responses[i].LastIndexOf("gender")+10;
                    BotTeamInfo[currPokemon].Gender = responses[i][idx];
                }
                responses[i] = responses[i].Split("</span>")[0];
                idx = responses[i].LastIndexOf(">") + 1;
                BotTeamInfo[currPokemon].Name = responses[i][idx..];
            }
            if (responses[i].Contains("Ability")) {
                int idx = responses[i].LastIndexOf('>') + 1;
                BotTeamInfo[currPokemon].Ability = responses[i][idx..];
                Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("Level")) {
                int idx = responses[i].LastIndexOf('>') + 1;
                BotTeamInfo[currPokemon].Level = Int32.Parse(responses[i][idx..]);
                Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("Tera Type")) {
                responses[i] = responses[i][0..^7];
                int idx = responses[i].LastIndexOf('>') + 1;
                BotTeamInfo[currPokemon].Tera = responses[i][idx..];
                Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("EVs")) {
                string[] temp = responses[i].Split(" / ");
                for (int j = 0; j < temp.Length; j++) {
                    temp[j] = temp[j][0..^7];
                    int idx = temp[j].LastIndexOf('>') + 1;
                    //BotTeamInfo[currPokemon].EV = temp[j][idx..]; 
                }
                Debug.WriteLine(i);
                continue;
            }

        }
        Debug.WriteLine(format);
        for (int i = 0; i < 6; i++) {
            Debug.WriteLine(BotTeamInfo[i].Name);
            for (int j = 0; j<4; j++) {
                Debug.WriteLine(BotTeamInfo[i].Moves[j]);
            }
        }
        
    }
}
