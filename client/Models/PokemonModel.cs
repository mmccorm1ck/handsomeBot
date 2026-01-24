using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;

public class PokemonModel() : INotifyPropertyChanged // Class to convert pokemon info into server-compatable format
{
    public PokemonModel(int inputGen, TeamModel inputModel) : this()
    {
        gen = inputGen;
        name = inputModel.Name;
        options = new Options(inputModel);
    }
    public int gen // Lower case variable names for server compatability
    {
        get => _gen;
        set
        {
            _gen = value;
            OnPropertyChanged();
        }
    }
    public string name 
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }
    public Options options // Battle modifiers
    {
        get => _options;
        set
        {
            _options = value;
            OnPropertyChanged();
        } 
    }
    private int _gen = -1;
    private string _name = "None";
    private Options _options = new() {};
    public class Options() // Class to hold battle modifiers
    {
        public Options(TeamModel inputModel) : this()
        {
            if (inputModel.Transform != null)
            {
                inputModel = inputModel.Transform;
            }

            level = inputModel.Level;
            if (inputModel.Gender != 'R') gender = inputModel.Gender;
            if (inputModel.Ability != "None") ability = inputModel.Ability;
            if (inputModel.AbilityActive) abilityOn = true;
            if (inputModel.Item != "None" && !inputModel.ItemRemoved) item = inputModel.Item;
            if (inputModel.Tera != "None" && inputModel.TeraActive) teraType = inputModel.Tera;
            nature = inputModel.Nature;
            if (inputModel.NonVolStatus != "") status = ParseStatus(inputModel);
            ivs = new EVIV(inputModel.IV);
            evs = new EVIV(inputModel.EV);
            boosts = new EVIV(inputModel.StatChanges);
            moves = [..inputModel.Moves];
            moves.RemoveAll((m) => m == "");
        }
        public int level
        {
            get => _level;
            set
            {
                _level = value;
            }
        }
        public char? gender
        {
            get => _gender;
            set
            {
                _gender = value;
            }
        }
        public string? ability
        {
            get => _ability;
            set
            {
                _ability = value;
            }
        }
        public bool? abilityOn
        {
            get => _abilityOn;
            set
            {
                _abilityOn = value;
            }
        }
        public string? item
        {
            get => _item;
            set
            {
                _item = value;
            }
        }
        public string? teraType
        {
            get => _teraType;
            set
            {
                _teraType = value;
            }
        }
        public string? nature
        {
            get => _nature;
            set
            {
                _nature = value;
            }
        }
        public string? status
        {
            get => _status;
            set
            {
                _status = value;
            }
        }
        public EVIV ivs
        {
            get => _ivs;
            set
            {
                _ivs = value;
            }
        }
        public EVIV evs
        {
            get => _evs;
            set
            {
                _evs = value;
            }
        }
        public EVIV? boosts
        {
            get => _boosts;
            set
            {
                _boosts = value;
            }
        }
        public List<string>? moves
        {
            get => _moves;
            set
            {
                _moves = value;
            }
        }
        private int _level = 50;
        private char? _gender;
        private string? _ability;
        private bool? _abilityOn;
        private string? _item;
        private string? _teraType;
        private string? _nature;
        private string? _status;
        private EVIV _ivs = new();
        private EVIV _evs = new();
        private EVIV? _boosts;
        private List<string>? _moves;
        }

    public class EVIV() // Class to hold info about EV and IV values
    {
        public EVIV(EVIVModel inputModel) : this()
        {
            hp = inputModel.HP;
            atk = inputModel.Atk;
            def = inputModel.Def;
            spa = inputModel.SpA;
            spd = inputModel.SpD;
            spe = inputModel.Spe;
        }
        public int hp
        {
            get => _hp;
            set
            {
                _hp = value;
            }
        }
        public int atk
        {
            get => _atk;
            set
            {
                _atk = value;
            }
        }
        public int def
        {
            get => _def;
            set
            {
                _def = value;
            }
        }
        public int spa
        {
            get => _spa;
            set
            {
                _spa = value;
            }
        }
        public int spd
        {
            get => _spd;
            set
            {
                _spd = value;
            }
        }
        public int spe
        {
            get => _spe;
            set
            {
                _spe = value;
            }
        }
        private int _hp;
        private int _atk;
        private int _def;
        private int _spa;
        private int _spd;
        private int _spe;
    }

    /*static private List<string> ParseMoves(TeamModel inputModel) // Convert individual moves into List format
    {
        List<string> moves = [];
        if (inputModel.Move1 != "None") moves.Add(inputModel.Move1);
        if (inputModel.Move2 != "None") moves.Add(inputModel.Move2);
        if (inputModel.Move3 != "None") moves.Add(inputModel.Move3);
        if (inputModel.Move4 != "None") moves.Add(inputModel.Move4);
        return moves;
    }*/

    static private string ParseStatus(TeamModel inputModel)
    {
        return inputModel.NonVolStatus switch
        {
            "Burn" => "brn",
            "Freeze" => "frz",
            "Paralysis" => "par",
            "Poison" => "psn",
            "Badly Poisoned" => "tox",
            "Sleep" => "slp",
            _ => ""
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}