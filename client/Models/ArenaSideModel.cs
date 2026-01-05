using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace HandsomeBot.Models;

public class ArenaSideModel() : INotifyPropertyChanged // Class to store arena side info
{
    public int Spikes
    {
        get => _spikes;
        set
        {
            _spikes = value;
            OnPropertyChanged();
        }
    }
    public int ToxSpikes
    {
        get => _toxSpikes;
        set
        {
            _toxSpikes = value;
            OnPropertyChanged();
        }
    }
    public bool SeaOfFlames
    {
        get => _seaOfFlames;
        set
        {
            _seaOfFlames = value;
            OnPropertyChanged();
        }
    }
    public bool Moor
    {
        get => _moor;
        set
        {
            _moor = value;
            OnPropertyChanged();
        }
    }
    public bool Rainbow
    {
        get => _rainbow;
        set
        {
            _rainbow = value;
            OnPropertyChanged();
        }
    }
    public bool Steelsurge
    {
        get => _steelsurge;
        set
        {
            _steelsurge = value;
            OnPropertyChanged();
        }
    }
    public bool Vinelash
    {
        get => _vinelash;
        set
        {
            _vinelash = value;
            OnPropertyChanged();
        }
    }
    public bool Wildfire
    {
        get => _wildfire;
        set
        {
            _wildfire = value;
            OnPropertyChanged();
        }
    }
    public bool Cannonade
    {
        get => _cannonade;
        set
        {
            _cannonade = value;
            OnPropertyChanged();
        }
    }
    public bool Volcalith
    {
        get => _volcalith;
        set
        {
            _volcalith = value;
            OnPropertyChanged();
        }
    }
    public bool SR
    {
        get => _sr;
        set
        {
            _sr = value;
            OnPropertyChanged();
        }
    }
    public bool Reflect
    {
        get => _reflect;
        set
        {
            _reflect = value;
            OnPropertyChanged();
        }
    }
    public bool LightScreen
    {
        get => _lightScreen;
        set
        {
            _lightScreen = value;
            OnPropertyChanged();
        }
    }
    public bool Protected
    {
        get => _protected;
        set
        {
            _protected = value;
            OnPropertyChanged();
        }
    }
    public bool Seeded
    {
        get => _seeded;
        set
        {
            _seeded = value;
            OnPropertyChanged();
        }
    }
    public bool Foresight
    {
        get => _foresight;
        set
        {
            _foresight = value;
            OnPropertyChanged();
        }
    }
    public bool Tailwind
    {
        get => _tailwind;
        set
        {
            _tailwind = value;
            OnPropertyChanged();
        }
    }
    public bool HelpingHand
    {
        get => _helpingHand;
        set
        {
            _helpingHand = value;
            OnPropertyChanged();
        }
    }
    public bool FlowerGift
    {
        get => _flowerGift;
        set
        {
            _flowerGift = value;
            OnPropertyChanged();
        }
    }
    public bool FriendGuard
    {
        get => _friendGuard;
        set
        {
            _friendGuard = value;
            OnPropertyChanged();
        }
    }
    public bool AuroraVeil
    {
        get => _auroraVeil;
        set
        {
            _auroraVeil = value;
            OnPropertyChanged();
        }
    }
    public bool Battery
    {
        get => _battery;
        set
        {
            _battery = value;
            OnPropertyChanged();
        }
    }
    public bool PowerSpot
    {
        get => _powerSpot;
        set
        {
            _powerSpot = value;
            OnPropertyChanged();
        }
    }
    private int _spikes = 0;
    private int _toxSpikes = 0;
    private bool _seaOfFlames = false;
    private bool _moor = false;
    private bool _rainbow = false;
    private bool _steelsurge = false;
    private bool _vinelash = false;
    private bool _wildfire = false;
    private bool _cannonade = false;
    private bool _volcalith = false;
    private bool _sr = false;
    private bool _reflect = false;
    private bool _lightScreen = false;
    private bool _protected = false;
    private bool _seeded = false;
    private bool _foresight = false;
    private bool _tailwind = false;
    private bool _helpingHand = false;
    private bool _flowerGift = false;
    private bool _friendGuard = false;
    private bool _auroraVeil = false;
    private bool _battery = false;
    private bool _powerSpot = false;
    public void AddEffect(string effect)
    {
        switch (effect)
        {
            case "Tailwind":
                Tailwind = true;
                break;
            case "Sea of Flames":
                SeaOfFlames = true;
                break;
            case "Moor":
                Moor = true;
                break;
            case "Rainbow":
                Rainbow = true;
                break;
            case "Reflect":
                Reflect = true;
                break;
            case "Light Screen":
                LightScreen = true;
                break;
            case "Aurora Veil":
                AuroraVeil = true;
                break;
            case "Stealth Rock":
                SR = true;
                break;
            case "Spikes (1)":
                Spikes = 1;
                break;
            case "Spikes (2)":
                Spikes = 2;
                break;
            case "Spikes (3)":
                Spikes = 3;
                break;
            case "Toxic Spikes (1)":
                ToxSpikes = 1;
                break;
            case "Toxic Spikes (2)":
                ToxSpikes = 2;
                break;
            case "G-Max Steelsurge":
                Steelsurge = true;
                break;
            case "G-Max Vine Lash":
                Vinelash = true;
                break;
            case "G-Max Wildfire":
                Wildfire = true;
                break;
            case "G-Max Cannonade":
                Cannonade = true;
                break;
            case "G-Max Volcalith":
                Volcalith = true;
                break;
        }
    }
    public void RemoveEffect(string effect)
    {
        switch (effect)
        {
            case "Tailwind":
                Tailwind = false;
                break;
            case "Sea of Flames":
                SeaOfFlames = false;
                break;
            case "Moor":
                Moor = false;
                break;
            case "Rainbow":
                Rainbow = false;
                break;
            case "Reflect":
                Reflect = false;
                break;
            case "Light Screen":
                LightScreen = false;
                break;
            case "Aurora Veil":
                AuroraVeil = false;
                break;
            case "Stealth Rock":
                SR = false;
                break;
            case "Spikes (1)":
            case "Spikes (2)":
            case "Spikes (3)":
                Spikes = 0;
                break;
            case "Toxic Spikes (1)":
            case "Toxic Spikes (2)":
                ToxSpikes = 0;
                break;
            case "G-Max Steelsurge":
                Steelsurge = false;
                break;
            case "G-Max Vine Lash":
                Vinelash = false;
                break;
            case "G-Max Wildfire":
                Wildfire = false;
                break;
            case "G-Max Cannonade":
                Cannonade = false;
                break;
            case "G-Max Volcalith":
                Volcalith = false;
                break;
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}