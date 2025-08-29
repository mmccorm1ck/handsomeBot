using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace HandsomeBot.Models;

public class FieldModel() : INotifyPropertyChanged // Class to convert field info into server-compatable format
{
    public FieldModel(string inputGameType, ArenaModel inputModel, bool attackerIsBot) : this()
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
        if (attackerIsBot)
        {
            attackerSide = ParseSide(inputModel.BotSide);
            defenderSide = ParseSide(inputModel.OppSide);
        }
        else
        {
            attackerSide = ParseSide(inputModel.OppSide);
            defenderSide = ParseSide(inputModel.BotSide);
        }
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
    private Side _attackerSide;
    private Side _defenderSide;
    public struct Side()
    {
        public int spikes;
        public bool steelsurge;
        public bool vinelash;
        public bool wildfire;
        public bool cannonade;
        public bool volcalith;
        public bool isSR;
        public bool isReflect;
        public bool isLightScreen;
        public bool isProtected;
        public bool isSeeded;
        public bool isForesight;
        public bool isTailwind;
        public bool isHelpingHand;
        public bool isFlowerGift;
        public bool isFriendGuard;
        public bool isAuroraVeil;
        public bool isBattery;
        public bool isPowerSpot;
    }
    public Side ParseSide(ArenaSideModel inputSide)
    {
        Side side = new()
        {
            spikes = inputSide.Spikes,
            steelsurge = inputSide.Steelsurge,
            vinelash = inputSide.Vinelash,
            wildfire = inputSide.Wildfire,
            cannonade = inputSide.Cannonade,
            isSR = inputSide.SR,
            isReflect = inputSide.Reflect,
            isLightScreen = inputSide.LightScreen,
            isProtected = inputSide.Protected,
            isSeeded = inputSide.Seeded,
            isForesight = inputSide.Foresight,
            isTailwind = inputSide.Tailwind,
            isHelpingHand = inputSide.HelpingHand,
            isFlowerGift = inputSide.FlowerGift,
            isFriendGuard = inputSide.FriendGuard,
            isAuroraVeil = inputSide.AuroraVeil,
            isBattery = inputSide.Battery,
            isPowerSpot = inputSide.PowerSpot
        };
        return side;
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}