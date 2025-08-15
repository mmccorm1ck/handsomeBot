using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;
public class CalcRespModel() : INotifyPropertyChanged // Class to hold info about an event during a game
{
    public bool BotUser
    {
        get => _botUser;
        set
        {
            _botUser = value;
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
    public int TargetMon
    {
        get => _targetMon;
        set
        {
            _targetMon = value;
            OnPropertyChanged();
        }
    }
    public int MoveNo
    {
        get => _moveNo;
        set
        {
            _moveNo = value;
            OnPropertyChanged();
        }
    }
    public List<int> DamageRange
    {
        get => _damageRange;
        set
        {
            _damageRange = value;
            OnPropertyChanged();
        }
    }
    private bool _botUser = true;
    private int _userMon = -1;
    private int _targetMon = -1;
    private int _moveNo = -1;
    private List<int> _damageRange = new();
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}