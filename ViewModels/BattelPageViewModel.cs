using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.ViewModels;

public class BattlePageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private int _eventNumber = 1;

    public string eventNumber
    {
    get => String.Format("Event #{0}",_eventNumber);
        set
        {
            _eventNumber = Int32.Parse(value.Split('#')[1]);
            OnPropertyChanged();
        }
    }

    private string[] _availablePokemon = [
        "Pokemon 1",
        "Pokemon 2",
        "Pokemon A",
        "Pokemon B",
    ];
    private string[] _availableEvents = [
        "Move",
        "Switch",
        "Ability Activation",
        "Item Activation",
        "Terastallize",
        "Mega Evolution",
        "Dynamax",
        "Gigantamax",
        "Z-Move"
    ];
    public string[] availablePokemon
    {
    get => _availablePokemon;
        set
        {
            _availablePokemon = value;
            OnPropertyChanged();
        }
    }
    public string[] availableEvents
    {
    get => _availableEvents;
        set
        {
            _availableEvents = value;
            OnPropertyChanged();
        }
    }
    
}