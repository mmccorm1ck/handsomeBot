using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Collections.Generic;
using HandsomeBot.Models;

namespace HandsomeBot.ViewModels;

public class OppTeamPageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public OppTeamPageViewModel(GameModel game)
    {
        TheGame = game;
        Task.Run(async () => await GetAllMons());
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

    public void LoadPrev()
    {
        TheGame.OppTeam = TheGame.OppTeamPrev;
    }

    async public Task GetAllMons()
    {
        HttpClient client = new HttpClient();
        string url = "http://" + TheGame.ServerUrl + "/mons?{%22Gen%22:" + TheGame.Gen.ToString() + "}";
        string response = await client.GetStringAsync(url);
        if (response == null) return;
        Dictionary<string, object>? mons = JsonSerializer.Deserialize<Dictionary<string, object>>(response);
        if (mons == null) return;
        ObservableCollection<string> temp = [];
        foreach (string mon in mons.Keys) temp.Add(mon);
        AllMons = temp;
    }
}
