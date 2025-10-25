using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;

public class MoveModel(Dictionary<int, string> noToName) : INotifyPropertyChanged // Class to hold info about next recommended move
{
    private Dictionary<int, string> _noToName = noToName;
    private string _moveType = "Calculating";
    private int _targetNo = -1;
    private TeamModel _targetMon = new();
    private string _tera = "";
    private bool _mega = false;
    private bool _dynamax = false;
    private bool _zMove = false;
    public string MoveType
    {
        get => _moveType;
        set
        {
            _moveType = value;
            OnPropertyChanged();
        }
    }
    public int TargetNo
    {
        get => _targetNo;
        set
        {
            _targetNo = value;
            Update();
            OnPropertyChanged();
        }
    }
    public TeamModel TargetMon
    {
        get => _targetMon;
        set
        {
            _targetMon = value;
            OnPropertyChanged();
        }
    }
    public string Tera
    {
        get => _tera;
        set
        {
            _tera = value;
            OnPropertyChanged();
        }
    }
    public bool Mega
    {
        get => _mega;
        set
        {
            _mega = value;
            OnPropertyChanged();
        }
    }
    public bool Dynamax
    {
        get => _dynamax;
        set
        {
            _dynamax = value;
            OnPropertyChanged();
        }
    }
    public bool ZMove
    {
        get => _zMove;
        set
        {
            _zMove = value;
            OnPropertyChanged();
        }
    }
    public void Update()
    {
        if (TargetNo == -1)
        {
            TargetMon.Name = "None";
            return;
        }
        TargetMon.Name = _noToName[TargetNo].Replace("Opponent's ", "");
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}