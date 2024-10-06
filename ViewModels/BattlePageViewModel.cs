using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.ViewModels;

public class BattlePageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private int _eventNumber = 1; // Tracks event number in the chain of events

    public int EventNumber
    {
    get => _eventNumber;
        set
        {
            _eventNumber = value;
            OnPropertyChanged();
        }
    }

    private string[] _availablePokemon = [ // List of pokemon that can trigger events (WIP)
        "Pokemon 1",
        "Pokemon 2",
        "Pokemon A",
        "Pokemon B",
    ];
    private string[] _availableEvents = [ // List of possible events
        "Move",
        "Switch",
        "Ability Activation",
        "Ability Change",
        "Item Activation",
        "Item Reveal",
        "Item Change",
        "Terastallize",
        "Mega Evolution",
        "Dynamax",
        "Gigantamax",
        "Z-Move"
    ];
    public string[] AvailablePokemon
    {
    get => _availablePokemon;
        set
        {
            _availablePokemon = value;
            OnPropertyChanged();
        }
    }
    public string[] AvailableEvents
    {
    get => _availableEvents;
        set
        {
            _availableEvents = value;
            OnPropertyChanged();
        }
    }
    
}