using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;
public class TurnModel() : INotifyPropertyChanged // Class to hold info about a turn during a game
{
    public int TurnNo
    {
        get => _turnNo;
        set
        {
            _turnNo = value;
            OnPropertyChanged();
        }
    }
    public List<Models.EventModel> EventList
    {
        get => _eventList;
        set
        {
            _eventList = value;
            OnPropertyChanged();
        }
    }
    private int _turnNo;
    private List<Models.EventModel> _eventList = [];
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}