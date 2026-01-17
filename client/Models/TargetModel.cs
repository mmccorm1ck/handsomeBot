using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;

public class TargetModel(Dictionary<string, int> nameToNo) : INotifyPropertyChanged // Class for recording which pokemon are selected as targets in an event
{
    private readonly Dictionary<string, int> _nameToNo = nameToNo;
    private string _monName = "";
    public string MonName
    {
        get => _monName;
        set
        {
            _monName = value;
            _monNo = _nameToNo[value];
            _targetMonModel.Name = _monName;
            OnPropertyChanged();
        }
    }
    private int _monNo = -1;
    public int MonNo
    {
        get => _monNo;
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