using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Threading.Tasks;
using HandsomeBot.Models;
using System.Collections.ObjectModel;

namespace HandsomeBot.ViewModels;

public class BotTeamPageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public BotTeamPageViewModel(GameModel game, AllOptionsModel options)
    {
        TheGame = game;
        for (int i = 0; i < 6; i++)
        {
            Sprites.Add(new());
            TheGame.BotTeam[i].Attach(Sprites[i]);
        }
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

    private ObservableCollection<ImageListener> _sprites = [];

    public ObservableCollection<ImageListener> Sprites
    {
        get => _sprites;
        set
        {
            _sprites = value;
            OnPropertyChanged();
        }
    }

    public void LoadTeam()
    {
        TheGame.BotTeam = TheGame.BotTeamPrev;
        for (int i = 0; i < 6; i++)
        {
            TheGame.BotTeam[i].Attach(Sprites[i]);
        }
    }

    public void LoadPaste() // Triggered by load button on UI, calls async task to load team info
    {
        if (TheGame.BotTeamURL == "")
        {
            return;
        }
        Task task = Task.Run(async () => await LoadPasteHtml(TheGame.BotTeamURL));
    }

    public async Task LoadPasteHtml(string httpLink) // Parses HTML from pokepaste link and stores info in TheGame.BotTeam
    {
        HttpClient client = new();
        string response = await client.GetStringAsync(httpLink);
        string[] responses = response.Split('\n');
        int currPokemon = -1;
        int currMove = -1;
        for (int i = 0; i < responses.Length - 1; i++)
        {
            responses[i] = responses[i].Trim(' ', '\t');
            //Debug.WriteLine(i.ToString()+responses[i]);
            if (responses[i].Contains("<article>"))
            { // Detects splits between each pokemon
                currPokemon++;
                continue;
            }
            if (currPokemon < 0)
            { // If not yet reached pokemon info in pokepaste
                continue;
            }
            if (responses[i].Contains("Format"))
            { // Saves format of pokepaste
                TheGame.Format = responses[i].Split(' ')[1][0..^4];
                continue;
            }
            if (responses[i].Contains("img-pokemon"))
            { // Saves URL of pokemon image
                continue;
            }
            if (responses[i].Contains("Nature"))
            { // Saves pokemon's nature
                TheGame.BotTeam[currPokemon].Nature = responses[i].Split(' ')[0];
                currMove = 0; // Prepares to load moves
                continue;
            }
            if (currMove > -1)
            { // If loading moves
                if (responses[i] == "")
                { // If last move has been read
                    currMove = -1;
                    continue;
                }
                int idx;
                if (responses[i].Contains("IVs"))
                { // If there are IVs to be read
                    string[] temps = responses[i][31..].Split(" / ");
                    for (int j = 0; j < temps.Length; j++)
                    {
                        temps[j] = temps[j][18..^7];
                        string[] temp = temps[j].Split(" ");
                        idx = temp[0].LastIndexOf('>') + 1;
                        temp[0] = temp[0][idx..];
                        switch (temp[1])
                        {
                            case "HP":
                                TheGame.BotTeam[currPokemon].IV.HP = Int32.Parse(temp[0]);
                                break;
                            case "Atk":
                                TheGame.BotTeam[currPokemon].IV.Atk = Int32.Parse(temp[0]);
                                break;
                            case "Def":
                                TheGame.BotTeam[currPokemon].IV.Def = Int32.Parse(temp[0]);
                                break;
                            case "SpA":
                                TheGame.BotTeam[currPokemon].IV.SpA = Int32.Parse(temp[0]);
                                break;
                            case "SpD":
                                TheGame.BotTeam[currPokemon].IV.SpD = Int32.Parse(temp[0]);
                                break;
                            case "Spe":
                                TheGame.BotTeam[currPokemon].IV.Spe = Int32.Parse(temp[0]);
                                break;
                        }
                    }
                    continue;
                }
                idx = responses[i].LastIndexOf('>') + 1;
                currMove++; // Increments to next move
                switch (currMove)
                {
                    case 1:
                        TheGame.BotTeam[currPokemon].Move1 = responses[i][idx..].TrimStart([' ', '-']);
                        break;
                    case 2:
                        TheGame.BotTeam[currPokemon].Move2 = responses[i][idx..].TrimStart([' ', '-']);
                        break;
                    case 3:
                        TheGame.BotTeam[currPokemon].Move3 = responses[i][idx..].TrimStart([' ', '-']);
                        break;
                    case 4:
                        TheGame.BotTeam[currPokemon].Move4 = responses[i][idx..].TrimStart([' ', '-']);
                        currMove = -1;
                        break;
                }
                continue;
            }
            if (!responses[i].Contains("<span"))
            { // If line has no relevent info
                continue;
            }
            if (responses[i].Contains("<pre>"))
            { // Detects line that contains pokemon name, gender and item
                int idx = responses[i].LastIndexOf('@') + 2;
                if (idx != 1)
                { // If there is an item
                    string item = responses[i][idx..];
                    if (item.Contains("<span"))
                    {
                        item = item[0..^7];
                        int idxtemp = item.LastIndexOf('>');
                        item = item[idxtemp..].Replace("'", "");
                    }
                    TheGame.BotTeam[currPokemon].Item = item;
                }
                if (responses[i].Contains("gender"))
                { // If there is a specified gender
                    idx = responses[i].LastIndexOf("gender") + 10;
                    TheGame.BotTeam[currPokemon].Gender = responses[i][idx];
                }
                responses[i] = responses[i].Split("</span>")[0];
                idx = responses[i].LastIndexOf(">") + 1;
                TheGame.BotTeam[currPokemon].Name = responses[i][idx..].Replace("'", "");
            }
            if (responses[i].Contains("Ability"))
            { // Saves pokemon's ability
                int idx = responses[i].LastIndexOf('>') + 1;
                TheGame.BotTeam[currPokemon].Ability = responses[i][idx..].Replace("'", "");
                continue;
            }
            if (responses[i].Contains("Level"))
            { // Saves pokemon's level
                int idx = responses[i].LastIndexOf('>') + 1;
                TheGame.BotTeam[currPokemon].Level = Int32.Parse(responses[i][idx..]);
                continue;
            }
            if (responses[i].Contains("Tera Type"))
            { // Saves pokemon's tera type
                responses[i] = responses[i][0..^7];
                int idx = responses[i].LastIndexOf('>') + 1;
                TheGame.BotTeam[currPokemon].Tera = responses[i][idx..];
                continue;
            }
            if (responses[i].Contains("EVs"))
            { // Saves pokemon's EVs
                string[] temps = responses[i][31..].Split(" / ");
                for (int j = 0; j < temps.Length; j++)
                {
                    temps[j] = temps[j][18..^7];
                    string[] temp = temps[j].Split(" ");
                    int idx = temp[0].LastIndexOf('>') + 1;
                    temp[0] = temp[0][idx..];
                    switch (temp[1])
                    {
                        case "HP":
                            TheGame.BotTeam[currPokemon].EV.HP = Int32.Parse(temp[0]);
                            break;
                        case "Atk":
                            TheGame.BotTeam[currPokemon].EV.Atk = Int32.Parse(temp[0]);
                            break;
                        case "Def":
                            TheGame.BotTeam[currPokemon].EV.Def = Int32.Parse(temp[0]);
                            break;
                        case "SpA":
                            TheGame.BotTeam[currPokemon].EV.SpA = Int32.Parse(temp[0]);
                            break;
                        case "SpD":
                            TheGame.BotTeam[currPokemon].EV.SpD = Int32.Parse(temp[0]);
                            break;
                        case "Spe":
                            TheGame.BotTeam[currPokemon].EV.Spe = Int32.Parse(temp[0]);
                            break;
                    }
                }
                continue;
            }
        }
        string tempGen = TheGame.Format.Split("gen")[1].Substring(0, 2);
        if (!Char.IsDigit(tempGen, 1)) tempGen = tempGen.Substring(0, 1);
        TheGame.Gen = int.Parse(tempGen);
        if (TheGame.Format.Contains("VGC") || TheGame.Format.Contains("Doubles")) TheGame.GameType = "Doubles";
        else TheGame.GameType = "Singles";
    }
}
