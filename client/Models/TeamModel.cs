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
            Task.Run(async () => await DownloadImage()); // Check if sprite needs to be downloaded when name changes
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
        get => _megaAbility != null ? _megaAbility : _ability == null ? _abilityDefault : _ability;
        set
        {
            _ability = value;
            OnPropertyChanged();
        }
    }
    public string? MegaAbility
    {
        get => _megaAbility;
        set
        {
            _megaAbility = value;
            OnPropertyChanged();
        }
    }
    public string AbilityDefault
    {
        get => _abilityDefault;
        set
        {
            _abilityDefault = value;
            OnPropertyChanged();
        }
    }
    public bool AbilityActive
    {
        get => _abilityActive;
        set
        {
            _abilityActive = value;
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
    public string? NatureBoost
    {
        get => _natureBoost;
        set
        {
            _natureBoost = value;
            OnPropertyChanged();
        }
    }
    public string? NatureDrop
    {
        get => _natureDrop;
        set
        {
            _natureDrop = value;
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
    public EVIVModel StatChanges
    {
        get => _statChanges;
        set
        {
            _statChanges = value;
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
    public bool TeraActive
    {
        get => _teraActive;
        set
        {
            _teraActive = value;
            OnPropertyChanged();
        }
    }
    public List<string> Moves
    {
        get => _moves;
        set
        {
            _moves = value;
            OnPropertyChanged();
        }
    }
    /*public string Move1
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
    }*/
    public string PokeImage
    {
        get => _image;
        set
        {
            _image = value;
            Notify(); // Update image listeners when sprite path changes
        }
    }
    public int RemainingHP
    {
        get => _remainingHP;
        set
        {
            _remainingHP = value;
            OnPropertyChanged();
        }
    }
    public List<string> VolStatus
    {
        get => _volStatus;
        set
        {
            _volStatus = value;
            OnPropertyChanged();
        }
    }

    public string NonVolStatus
    {
        get => _nonVolStatus;
        set
        {
            _nonVolStatus = value;
            Notify(); // Update image listeners when status changes
        }
    }

    public string Position
    {
        get => _position;
        set
        {
            _position = value;
            Notify();
        }
    }

    public bool ItemRemoved
    {
        get => _itemRemoved;
        set
        {
            _itemRemoved = value;
            OnPropertyChanged();
        }
    }

    public int TurnDynamaxed
    {
        get => _turnDynamaxed;
        set
        {
            _turnDynamaxed = value;
            OnPropertyChanged();
        }
    }

    public bool GMax
    {
        get => _gMax;
        set
        {
            _gMax = value;
            OnPropertyChanged();
        }
    }

    public string BaseForme
    {
        get => _baseForme;
        set
        {
            _baseForme = value;
            OnPropertyChanged();
        }
    }

    public string TypeChange
    {
        get => _typeChange;
        set
        {
            _typeChange = value;
            OnPropertyChanged();
        }
    }

    public bool ZoroSuspect
    {
        get => _zoroSuspect;
        set
        {
            _zoroSuspect = value;
            OnPropertyChanged();
        }
    }

    public TeamModel? Transform
    {
        get => _transform;
        set
        {
            _transform = value;
            OnPropertyChanged();
        }
    }

    private string _name = "None"; // Pokemon's name
    private char _gender = 'R'; // Pokemon's gender
    private string _item = "None"; // Pokemon's held item
    private int _level = 50; // Pokemon's level
    private string? _ability; // Pokemon's ability
    private string? _megaAbility;
    private string _abilityDefault = "None";
    private bool _abilityActive = false;
    private string _nature = "Hardy"; // Pokemon's nature
    private string? _natureBoost;
    private string? _natureDrop;
    private EVIVModel _ev = new(); // Array of pokemon's EVs
    private EVIVModel _iv = new()
    {
        HP = 31,
        Atk = 31,
        Def = 31,
        SpA = 31,
        SpD = 31,
        Spe = 31
    }; // Array of pokemon's IVs
    private EVIVModel _statChanges = new(); // Array of pokemon's stat changes
    private string _tera = "None"; // Pokemon's tera type
    private bool _teraActive = false;
    private List<string> _moves = ["", "", "", ""];
    /*private string _move1 = "None"; // Array of pokemon's moves
    private string _move2 = "None"; // Array of pokemon's moves
    private string _move3 = "None"; // Array of pokemon's moves
    private string _move4 = "None"; // Array of pokemon's moves*/
    private string _image = "Assets/None.png"; // URI of pokemon's image
    private int _remainingHP = 100;
    private List<string> _volStatus = [];
    private string _nonVolStatus = "";
    private string _position = "Reserve";
    private bool _itemRemoved = false;
    private int _turnDynamaxed = -1;
    private bool _gMax = false;
    private string _baseForme = "";
    private string _typeChange = "";
    private bool _zoroSuspect = false;
    private TeamModel? _transform;
    private List<ImageListener> listeners = []; // List of image listeners
    public void Attach(ImageListener listener) // Add new image listener
    {
        listeners.Add(listener);
        Notify();
    }
    public void Clear() // Remove all image listeners
    {
        listeners = [];
    }
    public void Notify() // Update image listeners with new sprite path
    {
        foreach (ImageListener listener in listeners)
        {
            listener.Update(PokeImage, NonVolStatus, Position);
        }
    }
    public async Task DownloadImage() // Downloads sprite image
    {
        string filename = "Assets/" + Name + ".png";
        if (File.Exists(filename)) // Check if sprite has already been downloaded
        {
            PokeImage = filename; // Just update image path
            return;
        }
        string url = "http://play.pokemonshowdown.com/sprites/gen5/" + Name.ToLower() + ".png"; // Sprites are downloaded from pokemon showdown's gen 5 style sprites
        HttpClient client = new();
        try
        {
            var response = await client.GetAsync(new Uri(url)); // Open connection
            if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK) return; // Return if unable to connect
            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync(); // Read in image as byte array
            File.WriteAllBytes(filename, imageBytes); // Write byte array to file
            PokeImage = filename; // Update image path
        }
        catch
        {
            PokeImage = "Assets/None.png"; // On exception, set path to blank image
        }
    }
    public TeamModel CloneForTransform()
    {
        return new()
        {
            Name = Name,
            Gender = Gender,
            Ability = Ability,
            MegaAbility = MegaAbility,
            Nature = Nature,
            EV = EV, // Copy EV and IV by reference for updating
            IV = IV,
            StatChanges = StatChanges.CloneStats(),
            Moves = Moves
            /*Move1 = Move1,
            Move2 = Move2,
            Move3 = Move3,
            Move4 = Move4*/
        };
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}