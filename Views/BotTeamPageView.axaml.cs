using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace HandsomeBot.Views;

public partial class BotTeamPageView : UserControl
{
    
    public BotTeamPageView()
    {
        InitializeComponent();
    }

    public void LoadPaste(object source, RoutedEventArgs args)
    {
        if (pasteLink.Text is null)
        {
            return;
        }
        string httpLink = pasteLink.Text;
        Task task = Task.Run(async () => await LoadPasteHtml(httpLink));
    }


    public async Task LoadPasteHtml(string httpLink)
    {
        HttpClient client = new HttpClient();
        string response = await client.GetStringAsync(httpLink);
        string[] responses = response.Split('\n');
        for(int i = 0; i < responses.Length; i++) {
            Debug.WriteLine(i.ToString()+responses[i]);
        }
        Dictionary<string, string>[] botTeamDict = new Dictionary<string, string>[]
        {
            new Dictionary<string,string>(),
            new Dictionary<string,string>(),
            new Dictionary<string,string>(),
            new Dictionary<string,string>(),
            new Dictionary<string,string>(),
            new Dictionary<string,string>()
        };
        int currPokemon = -1;
        int currMove = -1;
        string format = "";
        for (int i = 0; i < responses.Length-1; i++) {
            responses[i] = responses[i].TrimStart(' ','\t');
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
                botTeamDict[currPokemon].Add("image", "https://pokepast.es" + responses[i].Split(' ')[2][5..^2]);
                Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("Nature")) {
                botTeamDict[currPokemon].Add("nature", responses[i].Split(' ')[0]);
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
                    botTeamDict[currPokemon].Add("IVs", responses[i][idx..]);
                    Debug.WriteLine(i);
                    continue;
                }
                idx = responses[i].LastIndexOf('>') + 1;
                botTeamDict[currPokemon].Add("move"+currMove.ToString(), responses[i][idx..].TrimStart([' ','-']));
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
            if (responses[i].Contains("Ability")) {
                int idx = responses[i].LastIndexOf('>') + 1;
                botTeamDict[currPokemon].Add("ability", responses[i][idx..]);
                Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("Level")) {
                int idx = responses[i].LastIndexOf('>') + 1;
                botTeamDict[currPokemon].Add("level", responses[i][idx..]);
                Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("Tera Type")) {
                responses[i] = responses[i][0..^7];
                int idx = responses[i].LastIndexOf('>') + 1;
                botTeamDict[currPokemon].Add("tera", responses[i][idx..]);
                Debug.WriteLine(i);
                continue;
            }
            if (responses[i].Contains("EVs")) {
                string[] temp = responses[i].Split(" / ");
                for (int j = 0; j < temp.Length; j++) {
                    temp[j] = temp[j][0..^7];
                    int idx = temp[j].LastIndexOf('>') + 1;
                    botTeamDict[currPokemon].Add("EVs"+j.ToString(), temp[j][idx..]); 
                }
                Debug.WriteLine(i);
                continue;
            }

        }
        Debug.WriteLine(format);
        for (int i = 0; i < 6; i++) {
            Debug.WriteLine(botTeamDict[i]["ability"]);
        }
    }

    public void LoadPrevious(object source, RoutedEventArgs args)
    {
        Debug.WriteLine("Clack!");
    }
}

