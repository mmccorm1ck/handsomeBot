using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;
public class CalcRespModel() : INotifyPropertyChanged // Class to hold info about an event during a game
{
    public bool BotUser // Whether the move user was on the bot's team or not
    {
        get => _botUser;
        set
        {
            _botUser = value;
            OnPropertyChanged();
        }
    }
    public string UserMon // The mon that used the move
    {
        get => _userMon;
        set
        {
            _userMon = value;
            OnPropertyChanged();
        }
    }
    public string TargetMon // The target of the move
    {
        get => _targetMon;
        set
        {
            _targetMon = value;
            OnPropertyChanged();
        }
    }
    public int MoveNo // The move number out of the mon's available moves
    {
        get => _moveNo;
        set
        {
            _moveNo = value;
            OnPropertyChanged();
        }
    }
    public string Damage // The string describing how much damage would be dealt
    {
        get => _damage;
        set
        {
            _damage = value;
            ParseDamageRange();
            OnPropertyChanged();
        }
    }
    public float MaxDamage
    {
        get => _maxDamage;
        set
        {
            _maxDamage = value;
            OnPropertyChanged();
        }
    }
    public float MinDamage
    {
        get => _minDamage;
        set
        {
            _minDamage = value;
            OnPropertyChanged();
        }
    }
    private bool _botUser = true;
    private string _userMon = "";
    private string _targetMon = "";
    private int _moveNo = -1;
    private string _damage = "";
    private float _maxDamage = 0;
    private float _minDamage = 0;
    private void ParseDamageRange()
    {
        string[] splitInput = _damage.Split(':')[1].Split('(')[1].Split("%)")[0].Split(" - ");
        if (float.TryParse(splitInput[0], out float damage))
        {
            MinDamage = damage;
        }
        else
        {
            MinDamage = 0;
        }
        if (float.TryParse(splitInput[1], out damage))
        {
            MaxDamage = damage;
        }
        else
        {
            MaxDamage = 0;
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}