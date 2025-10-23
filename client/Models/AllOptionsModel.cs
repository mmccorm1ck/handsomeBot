using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace HandsomeBot.Models;

public class AllOptionsModel() : INotifyPropertyChanged // Class holding lists of options for dropdown menus. These need to be dynamic as they change between game generations
{
    private ObservableCollection<string> _allItems = [];
    private ObservableCollection<string> _allAbilities = [];
    private ObservableCollection<string> _allMons = [];
    private Dictionary<string, List<string>> _allFormes = [];
    public ObservableCollection<string> AllItems // All in-game items
    {
        get => _allItems;
        set
        {
            _allItems = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<string> AllAbilities // All pokemon abilities
    {
        get => _allAbilities;
        set
        {
            _allAbilities = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<string> AllMons // All pokemon names
    {
        get => _allMons;
        set
        {
            _allMons = value;
            OnPropertyChanged();
        }
    }
    public Dictionary<string, List<string>> AllFormes // Dictionary of all alternate formes for each mon
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
        string response = await client.GetStringAsync(url); // Get list of all in-game abilities from server
        if (response == null) return;
        ObservableCollection<string>? temp = JsonSerializer.Deserialize<ObservableCollection<string>>(response);
        if (temp == null) return;
        AllAbilities = temp; // Assign list of abilities to AllAbilities
        url = "http://" + game.ServerUrl + "/items?{%22Gen%22:" + game.Gen.ToString() + "}";
        response = await client.GetStringAsync(url); // Get list of all in-game items from server
        if (response == null) return;
        temp = JsonSerializer.Deserialize<ObservableCollection<string>>(response);
        if (temp == null) return;
        AllItems = temp; // Assign list of items to AllAbilities
        url = "http://" + game.ServerUrl + "/mons?{%22Gen%22:" + game.Gen.ToString() + "}";
        response = await client.GetStringAsync(url); // Get list of all pokemon info from server
        if (response == null) return;
        Dictionary<string, MonFormes>? allMonInfo = JsonSerializer.Deserialize<Dictionary<string, MonFormes>>(response);
        if (allMonInfo == null) return;
        AllMons = [.. allMonInfo.Keys]; // Assign all pokemon names to AllMons
        foreach (string name in AllMons) // Loop over all pokemon
        {
            if (AllFormes.ContainsKey(name)) continue; // Skip pokemon that already have an entry in AllFormes
            if (allMonInfo[name] == null) // If no additional info
            {
                AllFormes.Add(name, [name]); // Only form is itself
                continue;
            }
            List<string>? formes = allMonInfo[name].otherFormes; // Try to get list of any other formes for that mon
            if (formes == null) // If none found
            {
                AllFormes.Add(name, [name]); // Only forme is itself
                continue;
            }
            List<string> formesWithName = [.. formes.Prepend(name)]; // List of alternate formes plus base forme
            foreach (string formeName in formesWithName) // For each forme
            {
                if (!AllFormes.TryAdd(formeName, formesWithName)) AllFormes[formeName] = formesWithName; // Try to add a new entry for each forme, otherwise update the existing one
            }               
        }
    }
    public class MonFormes() // Class to decode forme info from server into 
    {
        public List<string>? otherFormes { get; set; }
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}