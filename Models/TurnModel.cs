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
    public List<int> BotStartMons
    {
        get => _botStartMons;
        set
        {
            _botStartMons = value;
            OnPropertyChanged();
        }
    }
    public List<int> BotEndMons
    {
        get => _botEndMons;
        set
        {
            _botEndMons = value;
            OnPropertyChanged();
        }
    }
    public List<int> OppStartMons
    {
        get => _oppStartMons;
        set
        {
            _oppStartMons = value;
            OnPropertyChanged();
        }
    }
    public List<int> OppEndMons
    {
        get => _oppEndMons;
        set
        {
            _oppEndMons = value;
            OnPropertyChanged();
        }
    }
    private int _turnNo;
    private List<Models.EventModel> _eventList = [];
    private List<int> _botStartMons = [];
    private List<int> _botEndMons = [];
    private List<int> _oppStartMons = [];
    private List<int> _oppEndMons = [];
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}