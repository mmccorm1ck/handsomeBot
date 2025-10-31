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
    public string[] AvailableEvents { get; set; } = [ // List of possible events
        "Move",
        "Switch",
        "KO",
        "Ability Activation",
        "Ability Change",
        "Ability Reveal",
        "Item Activation",
        "Item Reveal",
        "Item Change",
        "Status Change",
        "Status Activation",
        "Status Ended",
        "Stat Level Change",
        "Forme Reveal",
        "Forme Change",
        "Field Effect Change",
        "Field Effect Ended",
        "Terastallize",
        "Type Change",
        "Mega Evolution",
        "Dynamax",
        "Gigantamax",
        "Z-Move",
        "Position Change",
        "Transformation",
        "Illusion Reveal"
    ];
    public string[] AvailableOpeningEvents { get; set; } = [ // List of possible events in turn 0
        "Ability Activation",
        "Ability Change",
        "Ability Reveal",
        "Item Activation",
        "Item Reveal",
        "Item Change",
        "Stat Level Change",
        "Forme Reveal",
        "Forme Change",
        "Field Effect Change",
        "Field Effect Ended",
        "Type Change",
        "Switch"
    ];
    public string[] TypeList { get; set; } = [ // List of types that a mon could change to
        "Normal",
        "Fighting",
        "Flying",
        "Poison",
        "Ground",
        "Rock",
        "Bug",
        "Ghost",
        "Steel",
        "Fire",
        "Water",
        "Grass",
        "Electric",
        "Psychic",
        "Ice",
        "Dragon",
        "Dark",
        "Fairy",
        "???",
        "Stellar"
    ];
    public string[] StatusList { get; set; } = [
        "Burn",
        "Freeze",
        "Paralysis",
        "Poison",
        "Badly Poisoned",
        "Sleep",
        "Frostbite",
        "Protected",
        "Substitute",
        "Transformed",
        "Bound",
        "Curse",
        "Nightmare",
        "Perish",
        "Seeded",
        "Salt Cure",
        "Splinters",
        "Autotomize",
        "Identified",
        "Minimize",
        "Tar Shot",
        "Grounded",
        "Magnetic Levitation",
        "Telekinesis",
        "Aqua Ring",
        "Rooted",
        "Laser Focus",
        "Taking Aim",
        "Drowsy",
        "Charged",
        "Stockpiled",
        "Defence Curl",
        "Primed",
        "Can't Escape",
        "No Retreat",
        "Octolock",
        "Disable",
        "Embargo",
        "Heal Block",
        "Imprison",
        "Taunt",
        "Throat Chop",
        "Torment",
        "Confusion",
        "Infatuation",
        "Getting Pumped",
        "Guard Split",
        "Power Split",
        "Speed Swap",
        "Power Trick",
        "Power Boost",
        "Power Drop",
        "Guard Boost",
        "Guard Drop",
        "Critical Hit Boost",
        "Obscured",
        "Encore",
        "Rampage",
        "Rolling",
        "Making an Uproar",
        "Fixated",
        "Bide",
        "Recharging",
        "Charging Turn",
        "Semi-invulnerable Turn",
        "Flinch",
        "Bracing",
        "Center of Attention",
        "Magic Coat",
        "Might"
    ];
    public string[] StatList { get; set; } = [ // List of stats that could change
        "Atk",
        "Def",
        "SpA",
        "SpD",
        "Spe",
        "Acc",
        "Eva",
        "Crt"
    ];
    public string[] FieldList { get; set; } = [ // List of field effects that could happen on turn 0
        "Rain",
        "Sun",
        "Snow",
        "Sandstorm",
        "Electric Terrain",
        "Psychic Terrain",
        "Grassy Terrain",
        "Misty Terrain"
    ];
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