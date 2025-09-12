using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace HandsomeBot.Models;

public class FieldModel() : INotifyPropertyChanged // Class to convert field info into server-compatable format
{
    public FieldModel(string inputGameType, ArenaModel inputModel) : this()
    {
        gameType = inputGameType;
        if (inputModel.Weather != "None") weather = inputModel.Weather;
        if (inputModel.Terrain != "None") terrain = inputModel.Terrain;
        isMagicRoom = inputModel.MagicRoom;
        isWonderRoom = inputModel.WonderRoom;
        isGravity = inputModel.Gravity;
        isAuraBreak = inputModel.AuraBreak;
        isFairyAura = inputModel.FairyAura;
        isDarkAura = inputModel.DarkAura;
        isBeadsOfRuin = inputModel.BeadsOfRuin;
        isSwordOfRuin = inputModel.SwordOfRuin;
        isTabletOfRuin = inputModel.TabletOfRuin;
        isVesselOfRuin = inputModel.VesselOfRuin;
        attackerSide = new Side(inputModel.BotSide);
        defenderSide = new Side(inputModel.OppSide);
    }
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
    private bool _isMagicRoom;
    private bool _isWonderRoom;
    private bool _isGravity;
    private bool _isAuraBreak;
    private bool _isFairyAura;
    private bool _isDarkAura;
    private bool _isBeadsOfRuin;
    private bool _isSwordOfRuin;
    private bool _isTabletOfRuin;
    private bool _isVesselOfRuin;
    private Side _attackerSide = new();
    private Side _defenderSide = new();
    public class Side()
    {
        public Side(ArenaSideModel inputSide) : this()
        {
            spikes = inputSide.Spikes;
            steelsurge = inputSide.Steelsurge;
            vinelash = inputSide.Vinelash;
            wildfire = inputSide.Wildfire;
            cannonade = inputSide.Cannonade;
            isSR = inputSide.SR;
            isReflect = inputSide.Reflect;
            isLightScreen = inputSide.LightScreen;
            isProtected = inputSide.Protected;
            isSeeded = inputSide.Seeded;
            isForesight = inputSide.Foresight;
            isTailwind = inputSide.Tailwind;
            isHelpingHand = inputSide.HelpingHand;
            isFlowerGift = inputSide.FlowerGift;
            isFriendGuard = inputSide.FriendGuard;
            isAuroraVeil = inputSide.AuroraVeil;
            isBattery = inputSide.Battery;
            isPowerSpot = inputSide.PowerSpot;
        }
        public int spikes
        {
            get => _spikes;
            set
            {
                _spikes = value;
            }
        }
        public bool steelsurge
        {
            get => _steelsurge;
            set
            {
                _steelsurge = value;
            }
        }
        public bool vinelash
        {
            get => _vinelash;
            set
            {
                _vinelash = value;
            }
        }
        public bool wildfire
        {
            get => _wildfire;
            set
            {
                _wildfire = value;
            }
        }
        public bool cannonade
        {
            get => _cannonade;
            set
            {
                _cannonade = value;
            }
        }
        public bool volcalith
        {
            get => _volcalith;
            set
            {
                _volcalith = value;
            }
        }
        public bool isSR
        {
            get => _isSR;
            set
            {
                _isSR = value;
            }
        }
        public bool isReflect
        {
            get => _isReflect;
            set
            {
                _isReflect = value;
            }
        }
        public bool isLightScreen
        {
            get => _isLightScreen;
            set
            {
                _isLightScreen = value;
            }
        }
        public bool isProtected
        {
            get => _isProtected;
            set
            {
                _isProtected = value;
            }
        }
        public bool isSeeded
        {
            get => _isSeeded;
            set
            {
                _isSeeded = value;
            }
        }
        public bool isForesight
        {
            get => _isForesight;
            set
            {
                _isForesight = value;
            }
        }
        public bool isTailwind
        {
            get => _isTailwind;
            set
            {
                _isTailwind = value;
            }
        }
        public bool isHelpingHand
        {
            get => _isHelpingHand;
            set
            {
                _isHelpingHand = value;
            }
        }
        public bool isFlowerGift
        {
            get => _isFlowerGift;
            set
            {
                _isFlowerGift = value;
            }
        }
        public bool isFriendGuard
        {
            get => _isFriendGuard;
            set
            {
                _isFriendGuard = value;
            }
        }
        public bool isAuroraVeil
        {
            get => _isAuroraVeil;
            set
            {
                _isAuroraVeil = value;
            }
        }
        public bool isBattery
        {
            get => _isBattery;
            set
            {
                _isBattery = value;
            }
        }
        public bool isPowerSpot
        {
            get => _isPowerSpot;
            set
            {
                _isPowerSpot = value;
            }
        }
        private int _spikes;
        private bool _steelsurge;
        private bool _vinelash;
        private bool _wildfire;
        private bool _cannonade;
        private bool _volcalith;
        private bool _isSR;
        private bool _isReflect;
        private bool _isLightScreen;
        private bool _isProtected;
        private bool _isSeeded;
        private bool _isForesight;
        private bool _isTailwind;
        private bool _isHelpingHand;
        private bool _isFlowerGift;
        private bool _isFriendGuard;
        private bool _isAuroraVeil;
        private bool _isBattery;
        private bool _isPowerSpot;
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}