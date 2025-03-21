using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Markup.Xaml.MarkupExtensions;

namespace HandsomeBot.Models;

public class TeamModel() : INotifyPropertyChanged // Class to hold info about a pokemon in a team
{
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }
    public char Gender
    {
        get => _gender;
        set
        {
            _gender = value;
            OnPropertyChanged();
        }
    }
    public string Item
    {
        get => _item;
        set
        {
            _item = value;
            OnPropertyChanged();
        }
    }
    public int Level
    {
        get => _level;
        set
        {
            _level = value;
            OnPropertyChanged();
        }
    }
    public string Ability
    {
        get => _ability;
        set
        {
            _ability = value;
            OnPropertyChanged();
        }
    }
    public string Nature
    {
        get => _nature;
        set
        {
            _nature = value;
            OnPropertyChanged();
        }
    }
    public EVIVModel EV
    {
        get => _ev;
        set
        {
            _ev = value;
            OnPropertyChanged();
        }
    }
    public EVIVModel IV
    {
        get => _iv;
        set
        {
            _iv = value;
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
    public string Move1
    {
        get => _move1;
        set
        {
            _move1 = value;
            OnPropertyChanged();
        }
    }
    public string Move2
    {
        get => _move2;
        set
        {
            _move2 = value;
            OnPropertyChanged();
        }
    }
    public string Move3
    {
        get => _move3;
        set
        {
            _move3 = value;
            OnPropertyChanged();
        }
    }
    public string Move4
    {
        get => _move4;
        set
        {
            _move4 = value;
            OnPropertyChanged();
        }
    }
    public string PokeImage
    {
        get => _image;
        set
        {
            _image = value;
            OnPropertyChanged();
        }
    }

    private string _name = "None"; // Pokemon's name
    private char _gender = 'R'; // Pokemon's gender
    private string _item = "None"; // Pokemon's held item
    private int _level = 50; // Pokemon's level
    private string _ability = "None"; // Pokemon's ability
    private string _nature = "None"; // Pokemon's nature
    private EVIVModel _ev = new EVIVModel()
    {
        HP = 0,
        Atk = 0,
        Def = 0,
        SpA = 0,
        SpD = 0,
        Spe = 0
    }; // Array of pokemon's EVs in order: HP, ATK, DEF, SpATK, SpDEF, SPE
    private EVIVModel _iv = new EVIVModel()
    {
        HP = 31,
        Atk = 31,
        Def = 31,
        SpA = 31,
        SpD = 31,
        Spe = 31
    }; // Array of pokemon's IVs in order: HP, ATK, DEF, SpATK, SpDEF, SPE
    private string _tera = "None"; // Pokemon's tera type
    private string _move1 = "None"; // Array of pokemon's moves
    private string _move2 = "None"; // Array of pokemon's moves
    private string _move3 = "None"; // Array of pokemon's moves
    private string _move4 = "None"; // Array of pokemon's moves
    private string _image = ""; // URL of pokemon's image
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}