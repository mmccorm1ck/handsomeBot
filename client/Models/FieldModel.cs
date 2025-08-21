using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace HandsomeBot.Models;

public class FieldModel() : INotifyPropertyChanged // Class to convert pokemon info into server-compatable format
{
    public string gameType
    {
        get => _gameType;
        set
        {
            _gameType = value;
            OnPropertyChanged();
        }
    }
    public string? weather
    {
        get => _weather;
        set
        {
            _weather = value;
            OnPropertyChanged();
        }
    }
    public string? terrain
    {
        get => _terrain;
        set
        {
            _terrain = value;
            OnPropertyChanged();
        }
    }
    public bool isMagicRoom
    {
        get => _isMagicRoom;
        set
        {
            _isMagicRoom = value;
            OnPropertyChanged();
        }
    }
    public bool isWonderRoom
    {
        get => _isWonderRoom;
        set
        {
            _isWonderRoom = value;
            OnPropertyChanged();
        }
    }
    public bool isGravity
    {
        get => _isGravity;
        set
        {
            _isGravity = value;
            OnPropertyChanged();
        }
    }
    public bool isAuraBreak
    {
        get => _isAuraBreak;
        set
        {
            _isAuraBreak = value;
            OnPropertyChanged();
        }
    }
    public bool isFairyAura
    {
        get => _isFairyAura;
        set
        {
            _isFairyAura = value;
            OnPropertyChanged();
        }
    }
    public bool isDarkAura
    {
        get => _isDarkAura;
        set
        {
            _isDarkAura = value;
            OnPropertyChanged();
        }
    }
    public bool isBeadsOfRuin
    {
        get => _isBeadsOfRuin;
        set
        {
            _isBeadsOfRuin = value;
            OnPropertyChanged();
        }
    }
    public bool isSwordOfRuin
    {
        get => _isSwordOfRuin;
        set
        {
            _isSwordOfRuin = value;
            OnPropertyChanged();
        }
    }
    public bool isTabletOfRuin
    {
        get => _isTabletOfRuin;
        set
        {
            _isTabletOfRuin = value;
            OnPropertyChanged();
        }
    }
    public bool isVesselOfRuin
    {
        get => _isVesselOfRuin;
        set
        {
            _isVesselOfRuin = value;
            OnPropertyChanged();
        }
    }
    public Side attackerSide
    {
        get => _attackerSide;
        set
        {
            _attackerSide = value;
            OnPropertyChanged();
        }
    }
    public Side defenderSide
    {
        get => _defenderSide;
        set
        {
            _defenderSide = value;
            OnPropertyChanged();
        }
    }
    private string _gameType = "Doubles";
    private string? _weather;
    private string? _terrain;
    private bool _isMagicRoom = false;
    private bool _isWonderRoom = false;
    private bool _isGravity = false;
    private bool _isAuraBreak = false;
    private bool _isFairyAura = false;
    private bool _isDarkAura = false;
    private bool _isBeadsOfRuin = false;
    private bool _isSwordOfRuin = false;
    private bool _isTabletOfRuin = false;
    private bool _isVesselOfRuin = false;
    private Side _attackerSide = new();
    private Side _defenderSide = new();
    public struct Side()
    {
        public int spikes = 0;
        public bool steelsurge = false;
        public bool vinelash = false;
        public bool wildfire = false;
        public bool connonade = false;
        public bool valcalith = false;
        public bool isSR = false;
        public bool isReflect = false;
        public bool isLightScreen = false;
        public bool isProtected = false;
        public bool isSeeded = false;
        public bool isForesight = false;
        public bool isTailwind = false;
        public bool isHelpingHand = false;
        public bool isFlowerGift = false;
        public bool isFriendGuard = false;
        public bool isAuroraVeil = false;
        public bool isBattery = false;
        public bool isPowerSpot = false;
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}