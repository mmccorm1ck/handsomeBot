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
    private bool _wonderRoom = false;
    private bool _gravity = false;
    private bool _auraBreak = false;
    private bool _fairyAura = false;
    private bool _darkAura = false;
    private bool _beadsOfRuin = false;
    private bool _swordOfRuin = false;
    private bool _tabletOfRuin = false;
    private bool _vesselOfRuin = false;
    private ArenaSideModel _botSide = new();
    private ArenaSideModel _oppSide = new();
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}