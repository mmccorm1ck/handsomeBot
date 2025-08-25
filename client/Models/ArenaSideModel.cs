
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
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}