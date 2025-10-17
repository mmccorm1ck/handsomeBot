using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace HandsomeBot.Models;

public class AllOptionsModel() : INotifyPropertyChanged
{
    private ObservableCollection<string> _allItems = [];
    private ObservableCollection<string> _allAbilities = [];
    private ObservableCollection<string> _allMons = [];
    private Dictionary<string, List<string>> _allFormes = [];
    public ObservableCollection<string> AllItems
    {
        get => _allItems;
        set
        {
            _allItems = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<string> AllAbilities
    {
        get => _allAbilities;
        set
        {
            _allAbilities = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<string> AllMons
    {
        get => _allMons;
        set
        {
            _allMons = value;
            OnPropertyChanged();
        }
    }
    public Dictionary<string, List<string>> AllFormes
    {
        get => _allFormes;
        set
        {
            _allFormes = value;
            OnPropertyChanged();
        }
    }
    async public Task UpdateInfo(GameModel game)
    {
        HttpClient client = new();
        string url = "http://" + game.ServerUrl + "/abilities?{%22Gen%22:" + game.Gen.ToString() + "}";
        string response = await client.GetStringAsync(url);
        if (response == null) return;
        ObservableCollection<string>? temp = JsonSerializer.Deserialize<ObservableCollection<string>>(response);
        if (temp == null) return;
        AllAbilities = temp;
        url = "http://" + game.ServerUrl + "/items?{%22Gen%22:" + game.Gen.ToString() + "}";
        response = await client.GetStringAsync(url);
        if (response == null) return;
        temp = JsonSerializer.Deserialize<ObservableCollection<string>>(response);
        if (temp == null) return;
        AllItems = temp;
        url = "http://" + game.ServerUrl + "/mons?{%22Gen%22:" + game.Gen.ToString() + "}";
        response = await client.GetStringAsync(url);
        if (response == null) return;
        Dictionary<string, MonFormes>? allMonInfo = JsonSerializer.Deserialize<Dictionary<string, MonFormes>>(response);
        if (allMonInfo == null) return;
        AllMons = [.. allMonInfo.Keys];
        foreach (string name in AllMons)
        {
            if (AllFormes.ContainsKey(name)) continue;
            if (allMonInfo[name] == null)
            {
                AllFormes.Add(name, [name]);
                continue;
            }
            List<string>? formes = allMonInfo[name].otherFormes;
            if (formes == null)
            {
                AllFormes.Add(name, [name]);
                continue;
            }
            List<string> formesWithName = [.. formes.Prepend(name)];
            foreach (string formeName in formesWithName)
            {
                if (!AllFormes.TryAdd(formeName, formesWithName)) AllFormes[formeName] = formesWithName;
            }               
        }
    }
    public class MonFormes()
    {
        public List<string>? otherFormes { get; set; }
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}