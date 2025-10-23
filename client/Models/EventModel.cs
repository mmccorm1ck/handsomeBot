using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;
public class EventModel() : INotifyPropertyChanged // Class to hold info about an event during a game
{
    public int EventNo // Event number in that turn
    {
        get => _eventNo;
        set
        {
            _eventNo = value;
            OnPropertyChanged();
        }
    }
    public string EventType // Type of event from drop-down menu
    {
        get => _eventType;
        set
        {
            _eventType = value;
            Notify(); // Update event listeners
            OnPropertyChanged();
        }
    }
    public string MoveName // Name of move used
    {
        get => _moveName;
        set
        {
            _moveName = value;
            OnPropertyChanged();
        }
    }
    public string AbilityName // Name of ability involved
    {
        get => _abilityName;
        set
        {
            _abilityName = value;
            OnPropertyChanged();
        }
    }
    public string ItemName // Name of item involved
    {
        get => _itemName;
        set
        {
            _itemName = value;
            OnPropertyChanged();
        }
    }
    public string FieldChange // Name of field change
    {
        get => _fieldChange;
        set
        {
            _fieldChange = value;
            OnPropertyChanged();
        }
    }
    public string StatChange // Stat changed
    {
        get => _statChange;
        set
        {
            _statChange = value;
            OnPropertyChanged();
        }
    }
    public string TypeChange // Type changed to
    {
        get => _typeChange;
        set
        {
            _typeChange = value;
            OnPropertyChanged();
        }
    }
    public string FormeName // Name of forme changed to/revealed
    {
        get => _formeName;
        set
        {
            _formeName = value;
            OnPropertyChanged();
        }
    }
    public int UserMon // Pokemon that caused the event
    {
        get => _userMon;
        set
        {
            _userMon = value;
            OnPropertyChanged();
        }
    }
    public List<int> TargetMons // Mons that were targeted
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
    private string _fieldChange = "";
    private string _statChange = "";
    private string _typeChange = "";
    private string _formeName = "";
    private int _userMon;
    private List<int> _targetMons = [];
    private List<EventTypeListener> listeners = []; // List of event listeners used to update dropdown menu visibility
    public void Attach(EventTypeListener listener) // Add new event listener
    {
        listeners.Add(listener);
        Notify();
    }
    public void Clear() // Remove all event listeners
    {
        listeners = [];
    }
    public void Notify() // Send event type to listeners when changed
    {
        foreach (EventTypeListener listener in listeners)
        {
            listener.Update(EventType);
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}