using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System;

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
            Task.Run(async () => await DownloadImage());
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
            Notify();
        }
    }

    private string _name = "None"; // Pokemon's name
    private char _gender = 'R'; // Pokemon's gender
    private string _item = "None"; // Pokemon's held item
    private int _level = 50; // Pokemon's level
    private string _ability = "None"; // Pokemon's ability
    private string _nature = "None"; // Pokemon's nature
    private EVIVModel _ev = new()
    {
        HP = 0,
        Atk = 0,
        Def = 0,
        SpA = 0,
        SpD = 0,
        Spe = 0
    }; // Array of pokemon's EVs in order: HP, ATK, DEF, SpATK, SpDEF, SPE
    private EVIVModel _iv = new()
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
    private string _image = "Assets/None.png"; // URI of pokemon's image
    private List<ImageListener> listeners = [];
    public void Attach(ImageListener listener)
    {
        listeners.Add(listener);
        Notify();
    }
    public void Clear()
    {
        listeners = [];
    }
    public void Notify()
    {
        foreach (ImageListener listener in listeners)
        {
            listener.Update(PokeImage);
        }
    }
    public async Task DownloadImage()
    {
        string filename = "Assets/" + Name + ".png";
        if (File.Exists(filename))
        {
            PokeImage = filename;
            return;
        }
        string url = "http://play.pokemonshowdown.com/sprites/gen5/" + Name.ToLower() + ".png";
        HttpClient client = new();
        try
        {
            var response = await client.GetAsync(new Uri(url));
            if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK) return;
            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
            File.WriteAllBytes(filename, imageBytes);
            PokeImage = filename;
        }
        catch
        {
            PokeImage = "Assets/None.png";
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}