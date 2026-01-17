using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;
public class EVIVModel() : INotifyPropertyChanged // Class to hold info about a pokemon's stats
{
    public int HP
    {
        get => _hp;
        set
        {
            _hp = value;
            OnPropertyChanged();
        }
    }
    public int Atk
    {
        get => _atk;
        set
        {
            _atk = value;
            OnPropertyChanged();
        }
    }
    public int Def
    {
        get => _def;
        set
        {
            _def = value;
            OnPropertyChanged();
        }
    }
    public int SpA
    {
        get => _spa;
        set
        {
            _spa = value;
            OnPropertyChanged();
        }
    }
    public int SpD
    {
        get => _spd;
        set
        {
            _spd = value;
            OnPropertyChanged();
        }
    }
    public int Spe
    {
        get => _spe;
        set
        {
            _spe = value;
            OnPropertyChanged();
        }
    }
    public int Acc
    {
        get => _acc;
        set
        {
            _acc = value;
            OnPropertyChanged();
        }
    }
    public int Eva
    {
        get => _eva;
        set
        {
            _eva = value;
            OnPropertyChanged();
        }
    }
    public int Crt
    {
        get => _crt;
        set
        {
            _crt = value;
            OnPropertyChanged();
        }
    }
    private int _hp; // Pokemon's hit points stat
    private int _atk; // Pokemon's physical attack stat
    private int _def; // Pokemon's physical defence stat
    private int _spa; // Pokemon's special attack stat
    private int _spd; // Pokemon's special defence stat
    private int _spe; // Pokemon's speed stat
    private int _acc; // Pokemon's accuracy stat
    private int _eva; // Pokemon's evastion stat
    private int _crt; // Pokemon's crit chance stat

    public void IncrementStat(string statName, int statChange)
    {
        switch (statName)
        {
            case "Atk":
                Atk = ClampStat(Atk + statChange, -6, 6);
                break;
            case "Def":
                Def = ClampStat(Def + statChange, -6, 6);
                break;
            case "SpA":
                SpA = ClampStat(SpA + statChange, -6, 6);
                break;
            case "SpD":
                SpD = ClampStat(SpD + statChange, -6, 6);
                break;
            case "Spe":
                Spe = ClampStat(Spe + statChange, -6, 6);
                break;
            case "Acc":
                Acc = ClampStat(Acc + statChange, -6, 6);
                break;
            case "Eva":
                Eva = ClampStat(Eva + statChange, -6, 6);
                break;
            case "Crt":
                Crt = ClampStat(Crt + statChange, 0, 6);
                break;
        }
    }

    public void SetStat(string statName, int statValue)
    {
        statValue = ClampStat(statValue, -6, 6);
        switch (statName)
        {
            case "Atk":
                Atk = statValue;
                break;
            case "Def":
                Def = statValue;
                break;
            case "SpA":
                SpA = statValue;
                break;
            case "SpD":
                SpD = statValue;
                break;
            case "Spe":
                Spe = statValue;
                break;
            case "Acc":
                Acc = statValue;
                break;
            case "Eva":
                Eva = statValue;
                break;
            case "Crt":
                statValue = ClampStat(statValue, 0, 6);
                Crt = statValue;
                break;
        }
    }

    private static int ClampStat(int value, int min, int max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    public EVIVModel CloneStats()
    {
        return new()
        {
            Atk = Atk,
            Def = Def,
            SpA = SpA,
            SpD = SpD,
            Spe = Spe,
            Acc = Acc,
            Eva = Eva,
            Crt = Crt
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}