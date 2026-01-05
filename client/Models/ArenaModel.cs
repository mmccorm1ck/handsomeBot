using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace HandsomeBot.Models;

public class ArenaModel() : INotifyPropertyChanged // Class to store arena info
{
    public string Weather
    {
        get => _weather;
        set
        {
            _weather = value;
            OnPropertyChanged();
        }
    }
    public string Terrain
    {
        get => _terrain;
        set
        {
            _terrain = value;
            OnPropertyChanged();
        }
    }
    public bool MagicRoom
    {
        get => _magicRoom;
        set
        {
            _magicRoom = value;
            OnPropertyChanged();
        }
    }
    public bool TrickRoom
    {
        get => _trickRoom;
        set
        {
            _trickRoom = value;
            OnPropertyChanged();
        }
    }
    public bool WonderRoom
    {
        get => _wonderRoom;
        set
        {
            _wonderRoom = value;
            OnPropertyChanged();
        }
    }
    public bool Gravity
    {
        get => _gravity;
        set
        {
            _gravity = value;
            OnPropertyChanged();
        }
    }
    public bool AuraBreak
    {
        get => _auraBreak;
        set
        {
            _auraBreak = value;
            OnPropertyChanged();
        }
    }
    public bool FairyAura
    {
        get => _fairyAura;
        set
        {
            _fairyAura = value;
            OnPropertyChanged();
        }
    }
    public bool DarkAura
    {
        get => _darkAura;
        set
        {
            _darkAura = value;
            OnPropertyChanged();
        }
    }
    public bool BeadsOfRuin
    {
        get => _beadsOfRuin;
        set
        {
            _beadsOfRuin = value;
            OnPropertyChanged();
        }
    }
    public bool SwordOfRuin
    {
        get => _swordOfRuin;
        set
        {
            _swordOfRuin = value;
            OnPropertyChanged();
        }
    }
    public bool TabletOfRuin
    {
        get => _tabletOfRuin;
        set
        {
            _tabletOfRuin = value;
            OnPropertyChanged();
        }
    }
    public bool VesselOfRuin
    {
        get => _vesselOfRuin;
        set
        {
            _vesselOfRuin = value;
            OnPropertyChanged();
        }
    }
    public bool MudSport
    {
        get => _mudSport;
        set
        {
            _mudSport = value;
            OnPropertyChanged();
        }
    }
    public bool WaterSport
    {
        get => _waterSport;
        set
        {
            _waterSport = value;
            OnPropertyChanged();
        }
    }
    public ArenaSideModel BotSide
    {
        get => _botSide;
        set
        {
            _botSide = value;
            OnPropertyChanged();
        }
    }
    public ArenaSideModel OppSide
    {
        get => _oppSide;
        set
        {
            _oppSide = value;
            OnPropertyChanged();
        }
    }
    private string _weather = "None";
    private string _terrain = "None";
    private bool _magicRoom = false;
    private bool _trickRoom = false;
    private bool _wonderRoom = false;
    private bool _gravity = false;
    private bool _auraBreak = false;
    private bool _fairyAura = false;
    private bool _darkAura = false;
    private bool _beadsOfRuin = false;
    private bool _swordOfRuin = false;
    private bool _tabletOfRuin = false;
    private bool _vesselOfRuin = false;
    private bool _mudSport = false;
    private bool _waterSport = false;
    private ArenaSideModel _botSide = new();
    private ArenaSideModel _oppSide = new();
    public void AddEffect(string effect)
    {
        if (effect.Contains("Terrain"))
        {
            Terrain = effect;
            return;
        }
        AllOptionsModel allOptions = new();
        if (allOptions.FieldList.Contains(effect))
        {
            Weather = effect;
            return;
        }
        switch (effect)
        {
            case "Magic Room":
                MagicRoom = true;
                break;
            case "Trick Room":
                TrickRoom = true;
                break;
            case "Wonder Room":
                WonderRoom = true;
                break;
            case "Gravity":
                Gravity = true;
                break;
            case "Aura Break":
                AuraBreak = true;
                break;
            case "Fairy Aura":
                FairyAura = true;
                break;
            case "Dark Aura":
                DarkAura = true;
                break;
            case "Beads of Ruin":
                BeadsOfRuin = true;
                break;
            case "Sword of Ruin":
                SwordOfRuin = true;
                break;
            case "Tablet of Ruin":
                TabletOfRuin = true;
                break;
            case "Vessel of Ruin":
                VesselOfRuin = true;
                break;
            case "Mud Sport":
                MudSport = true;
                break;
            case "Water Sport":
                WaterSport = true;
                break;
        }
    }
    public void RemoveEffect(string effect)
    {
        if (effect.Contains("Terrain"))
        {
            Terrain = "None";
            return;
        }
        AllOptionsModel allOptions = new();
        if (allOptions.FieldList.Contains(effect))
        {
            Weather = "None";
            return;
        }
        switch (effect)
        {
            case "Magic Room":
                MagicRoom = false;
                break;
            case "Trick Room":
                TrickRoom = false;
                break;
            case "Wonder Room":
                WonderRoom = false;
                break;
            case "Gravity":
                Gravity = false;
                break;
            case "Aura Break":
                AuraBreak = false;
                break;
            case "Fairy Aura":
                FairyAura = false;
                break;
            case "Dark Aura":
                DarkAura = false;
                break;
            case "Beads of Ruin":
                BeadsOfRuin = false;
                break;
            case "Sword of Ruin":
                SwordOfRuin = false;
                break;
            case "Tablet of Ruin":
                TabletOfRuin = false;
                break;
            case "Vessel of Ruin":
                VesselOfRuin = false;
                break;
            case "Mud Sport":
                MudSport = false;
                break;
            case "Water Sport":
                WaterSport = false;
                break;
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}