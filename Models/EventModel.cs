using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;
public class EventModel() : INotifyPropertyChanged // Class to hold info about an event during a game
{
    public int EventNo
    {
        get => _eventNo;
        set
        {
            _eventNo = value;
            OnPropertyChanged();
        }
    }
    public string EventType
    {
        get => _eventType;
        set
        {
            _eventType = value;
            OnPropertyChanged();
        }
    }
    public string MoveName
    {
        get => _moveName;
        set
        {
            _moveName = value;
            OnPropertyChanged();
        }
    }
    public string AbilityName
    {
        get => _abilityName;
        set
        {
            _abilityName = value;
            OnPropertyChanged();
        }
    }
    public string ItemName
    {
        get => _itemName;
        set
        {
            _itemName = value;
            OnPropertyChanged();
        }
    }
    public int UserMon
    {
        get => _userMon;
        set
        {
            _userMon = value;
            OnPropertyChanged();
        }
    }
    public List<int> TargetMons
    {
        get => _targetMons;
        set
        {
            _targetMons = value;
            OnPropertyChanged();
        }
    }
    private int _eventNo;
    private string _eventType = "";
    private string _moveName = "";
    private string _abilityName = "";
    private string _itemName = "";
    private int _userMon;
    private List<int> _targetMons = [];
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}