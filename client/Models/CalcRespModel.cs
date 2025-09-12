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
    public string UserMon
    {
        get => _userMon;
        set
        {
            _userMon = value;
            OnPropertyChanged();
        }
    }
    public string TargetMon
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
    public string Damage
    {
        get => _damage;
        set
        {
            _damage = value;
            OnPropertyChanged();
        }
    }
    private bool _botUser = true;
    private string _userMon = "";
    private string _targetMon = "";
    private int _moveNo = -1;
    private string _damage = "";
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}