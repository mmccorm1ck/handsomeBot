using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;
public class TurnModel() : INotifyPropertyChanged // Class to hold info about a turn during a game
{
    public int TurnNo // Turn number in the game, with turn 0 being when mons are first sent out
    {
        get => _turnNo;
        set
        {
            _turnNo = value;
            OnPropertyChanged();
        }
    }
    public List<Models.EventModel> EventList // List of events from that turn
    {
        get => _eventList;
        set
        {
            _eventList = value;
            OnPropertyChanged();
        }
    }
    public List<int> BotStartMons // Bot's pokemon on the field at the beginning of the turn
    {
        get => _botStartMons;
        set
        {
            _botStartMons = value;
            OnPropertyChanged();
        }
    }
    public List<int> BotEndMons // Bot's pokemon on the field at the end of the turn
    {
        get => _botEndMons;
        set
        {
            _botEndMons = value;
            OnPropertyChanged();
        }
    }
    public List<int> OppStartMons // Opponent's pokemon on the field at the beginning of the turn
    {
        get => _oppStartMons;
        set
        {
            _oppStartMons = value;
            OnPropertyChanged();
        }
    }
    public List<int> OppEndMons // Opponent's pokemon on the field at the end of the turn
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
    private List<int> _botStartMons = [-1, -1];
    private List<int> _botEndMons = [-1, -1];
    private List<int> _oppStartMons = [-1, -1];
    private List<int> _oppEndMons = [-1, -1];
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}