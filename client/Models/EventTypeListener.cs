using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;

public class EventTypeListener : INotifyPropertyChanged
{
    private bool _moveEvent = false;
    private bool _itemEvent = false;
    private bool _abilityEvent = false;
    private bool _formeEvent = false;
    private bool _fieldEvent = false;
    private bool _statusEvent = false;
    private bool _typeEvent = false;
    private bool _statEvent = false;
    public bool MoveEvent
    {
        get => _moveEvent;
        set
        {
            _moveEvent = value;
            OnPropertyChanged();
        }
    }
    public bool ItemEvent
    {
        get => _itemEvent;
        set
        {
            _itemEvent = value;
            OnPropertyChanged();
        }
    }
    public bool AbilityEvent
    {
        get => _abilityEvent;
        set
        {
            _abilityEvent = value;
            OnPropertyChanged();
        }
    }
    public bool FormeEvent
    {
        get => _formeEvent;
        set
        {
            _formeEvent = value;
            OnPropertyChanged();
        }
    }
    public bool FieldEvent
    {
        get => _fieldEvent;
        set
        {
            _fieldEvent = value;
            OnPropertyChanged();
        }
    }
    public bool StatusEvent
    {
        get => _statusEvent;
        set
        {
            _statusEvent = value;
            OnPropertyChanged();
        }
    }
    public bool TypeEvent
    {
        get => _typeEvent;
        set
        {
            _typeEvent = value;
            OnPropertyChanged();
        }
    }
    public bool StatEvent
    {
        get => _statEvent;
        set
        {
            _statEvent = value;
            OnPropertyChanged();
        }
    }
    public void Update(string eventType)
    {
        Reset();
        switch (eventType)
        {
            case "Move":
            case "Move Reveal":
                MoveEvent = true;
                break;
            case "Item Activation":
            case "Item Reveal":
            case "Item Change":
                ItemEvent = true;
                break;
            case "Ability Activation":
            case "Ability Reveal":
            case "Ability Change":
                AbilityEvent = true;
                break;
            case "Forme Reveal":
            case "Forme Change":
                FormeEvent = true;
                break;
            case "Field Change":
                FieldEvent = true;
                break;
            case "Stat Level Change":
                StatEvent = true;
                break;
            case "Type Change":
            case "Terastallize":
                TypeEvent = true;
                break;
            case "Status Change":
                StatusEvent = true;
                break;
        }
    }
    public void Reset()
    {
        MoveEvent = false;
        ItemEvent = false;
        AbilityEvent = false;
        FormeEvent = false;
        FieldEvent = false;
        StatEvent = false;
        TypeEvent = false;
        StatusEvent = false;
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}