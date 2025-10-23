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
    public string gameType // Doubles or singles
    {
        get => _gameType;
        set
        {
            _gameType = value;
            OnPropertyChanged();
        }
    }
    public string? weather // Current weather on field, null if none
    {
        get => _weather;
        set
        {
            _weather = value;
            OnPropertyChanged();
        }
    }
    public string? terrain // Current terrain on field, null if none
    {
        get => _terrain;
        set
        {
            _terrain = value;
            OnPropertyChanged();
        }
    }
    public bool isMagicRoom // If magic room is active
    {
        get => _isMagicRoom;
        set
        {
            _isMagicRoom = value;
            OnPropertyChanged();
        }
    }
    public bool isWonderRoom // If wonder room is active
    {
        get => _isWonderRoom;
        set
        {
            _isWonderRoom = value;
            OnPropertyChanged();
        }
    }
    public bool isGravity // If gravity is active
    {
        get => _isGravity;
        set
        {
            _isGravity = value;
            OnPropertyChanged();
        }
    }
    public bool isAuraBreak // If aura break is active
    {
        get => _isAuraBreak;
        set
        {
            _isAuraBreak = value;
            OnPropertyChanged();
        }
    }
    public bool isFairyAura // If fairy aura is active
    {
        get => _isFairyAura;
        set
        {
            _isFairyAura = value;
            OnPropertyChanged();
        }
    }
    public bool isDarkAura // If dark aura is active
    {
        get => _isDarkAura;
        set
        {
            _isDarkAura = value;
            OnPropertyChanged();
        }
    }
    public bool isBeadsOfRuin // If BOR is active
    {
        get => _isBeadsOfRuin;
        set
        {
            _isBeadsOfRuin = value;
            OnPropertyChanged();
        }
    }
    public bool isSwordOfRuin // If SOR is active
    {
        get => _isSwordOfRuin;
        set
        {
            _isSwordOfRuin = value;
            OnPropertyChanged();
        }
    }
    public bool isTabletOfRuin // If TOR is active
    {
        get => _isTabletOfRuin;
        set
        {
            _isTabletOfRuin = value;
            OnPropertyChanged();
        }
    }
    public bool isVesselOfRuin // If VOR is active
    {
        get => _isVesselOfRuin;
        set
        {
            _isVesselOfRuin = value;
            OnPropertyChanged();
        }
    }
    public Side attackerSide // Info about attacker side-specific field effects
    {
        get => _attackerSide;
        set
        {
            _attackerSide = value;
            OnPropertyChanged();
        }
    }
    public Side defenderSide // Info about defender side-specific field effects
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
    public class Side() // Class holding info about side-specific effects
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
        public int spikes // Level of spikes active, 0 if none
        {
            get => _spikes;
            set
            {
                _spikes = value;
            }
        }
        public bool steelsurge // If steelsurge is active
        {
            get => _steelsurge;
            set
            {
                _steelsurge = value;
            }
        }
        public bool vinelash // If vinelash is active
        {
            get => _vinelash;
            set
            {
                _vinelash = value;
            }
        }
        public bool wildfire // If wildfire is active
        {
            get => _wildfire;
            set
            {
                _wildfire = value;
            }
        }
        public bool cannonade // If cannonade is active
        {
            get => _cannonade;
            set
            {
                _cannonade = value;
            }
        }
        public bool volcalith // If volcalith is active
        {
            get => _volcalith;
            set
            {
                _volcalith = value;
            }
        }
        public bool isSR // If stealth rocks is active
        {
            get => _isSR;
            set
            {
                _isSR = value;
            }
        }
        public bool isReflect // If reflect is active
        {
            get => _isReflect;
            set
            {
                _isReflect = value;
            }
        }
        public bool isLightScreen // If light screen is active
        {
            get => _isLightScreen;
            set
            {
                _isLightScreen = value;
            }
        }
        public bool isProtected // If target is protected
        {
            get => _isProtected;
            set
            {
                _isProtected = value;
            }
        }
        public bool isSeeded // If target is seeded
        {
            get => _isSeeded;
            set
            {
                _isSeeded = value;
            }
        }
        public bool isForesight // If foresight is hitting this turn
        {
            get => _isForesight;
            set
            {
                _isForesight = value;
            }
        }
        public bool isTailwind // If tailwind is active
        {
            get => _isTailwind;
            set
            {
                _isTailwind = value;
            }
        }
        public bool isHelpingHand // If helping hand is active
        {
            get => _isHelpingHand;
            set
            {
                _isHelpingHand = value;
            }
        }
        public bool isFlowerGift // If flower gift is active
        {
            get => _isFlowerGift;
            set
            {
                _isFlowerGift = value;
            }
        }
        public bool isFriendGuard // If friend guard is active
        {
            get => _isFriendGuard;
            set
            {
                _isFriendGuard = value;
            }
        }
        public bool isAuroraVeil // If aurora veil is active
        {
            get => _isAuroraVeil;
            set
            {
                _isAuroraVeil = value;
            }
        }
        public bool isBattery // If battery is active
        {
            get => _isBattery;
            set
            {
                _isBattery = value;
            }
        }
        public bool isPowerSpot // If power spot is active
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