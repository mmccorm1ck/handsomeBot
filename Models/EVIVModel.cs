using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Markup.Xaml.MarkupExtensions;

public class EVIVModel() : INotifyPropertyChanged // Class to hold info about a pokemon in a team
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
    private int _hp; // Pokemon's hit points stat
    private int _atk; // Pokemon's physical attack stat
    private int _def; // Pokemon's physical defence stat
    private int _spa; // Pokemon's special attack stat
    private int _spd; // Pokemon's special defence stat
    private int _spe; // Pokemon's speed stat

    public event PropertyChangedEventHandler PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}