using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;

public class TargetModel() : INotifyPropertyChanged // Class for recording which pokemon are selected as targets in an event
{
    public TargetModel(Dictionary<string, int> nameToNo) : this()
    {
        _nameToNo = nameToNo;
    }
    private readonly Dictionary<string, int> _nameToNo = [];
    private string _monName = "";
    public string MonName
    {
        get => _monName;
        set
        {
            _monName = value;
            _nameToNo.TryGetValue(value, out _monNo);
            _targetMonModel.Name = _monName.Replace("Opponent's ", "");
            OnPropertyChanged();
        }
    }
    private int _monNo = -1;
    public int MonNo
    {
        get => _monNo;
        set
        {
            _monNo = value;
            OnPropertyChanged();
        }
    }
    private int _allyNo = -1;
    public int AllyNo
    {
        get => _allyNo;
        set
        {
            _allyNo = value;
            OnPropertyChanged();
        }
    }
    private string _moveResult = "";
    public string MoveResult
    {
        get => _moveResult;
        set
        {
            _moveResult = value;
            OnPropertyChanged();
        }
    }
    private int? _damage;
    public int? Damage
    {
        get => _damage;
        set
        {
            _damage = value;
            OnPropertyChanged();
        }
    }
    private int _startingHP;
    public int StartingHP
    {
        get => _startingHP;
        set
        {
            _startingHP = value;
            OnPropertyChanged();
        }
    }
    private bool _crit = false;
    public bool Crit
    {
        get => _crit;
        set
        {
            _crit = value;
            OnPropertyChanged();
        }
    }
    private TeamModel _targetMonModel = new();
    public void Attach(ImageListener listener)
    {
        _targetMonModel.Attach(listener);
    }
    public void Clear()
    {
        _targetMonModel.Clear();
    }

    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}