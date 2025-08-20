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
        options = ParseOptions(inputModel);
    }
    public int gen // Lower case variable names for server compatability
    {
        get => _gen;
        set
        {
            gen = value;
            OnPropertyChanged();
        }
    }
    public string name 
    {
        get => _name;
        set
        {
            name = value;
            OnPropertyChanged();
        }
    }
    public Options options
    {
        get => _options;
        set
        {
            _options = value;
            OnPropertyChanged();
        } 
    }
    private int _gen;
    private string _name = "None";
    private Options _options;
    public struct Options()
    {
        public int level = 50;
        public char? gender;
        public string? ability;
        public bool? abilityOn;
        public string? item;
        public string? teraType;
        public string? nature;
        public EVIV ivs;
        public EVIV evs;
        public EVIV? boosts;
        public List<string>? moves;
    }

    public struct EVIV()
    {
        public int hp;
        public int atk;
        public int def;
        public int spa;
        public int spd;
        public int spe;
    }

    static private Options ParseOptions(TeamModel inputModel)
    {
        Options tempOptions = new()
        {
            level = inputModel.Level
        };
        if (inputModel.Gender != 'R') tempOptions.gender = inputModel.Gender;
        if (inputModel.Ability != "None") tempOptions.ability = inputModel.Ability;
        if (inputModel.Item != "None") tempOptions.item = inputModel.Item;
        if (inputModel.Tera != "None") tempOptions.teraType = inputModel.Tera;
        if (inputModel.Nature != "None") tempOptions.nature = inputModel.Nature;
        tempOptions.ivs = ParseStats(inputModel.IV);
        tempOptions.evs = ParseStats(inputModel.EV);
        tempOptions.moves = ParseMoves(inputModel);
        return tempOptions;
    }

    static private EVIV ParseStats(EVIVModel inputModel)
    {
        EVIV tempStats = new()
        {
            hp = inputModel.HP,
            atk = inputModel.Atk,
            def = inputModel.Def,
            spa = inputModel.SpA,
            spd = inputModel.SpD,
            spe = inputModel.Spe
        };
        return tempStats;
    }

    static private List<string> ParseMoves(TeamModel inputModel)
    {
        List<string> moves = [];
        if (inputModel.Move1 != "None") moves.Add(inputModel.Move1);
        if (inputModel.Move2 != "None") moves.Add(inputModel.Move2);
        if (inputModel.Move3 != "None") moves.Add(inputModel.Move3);
        if (inputModel.Move4 != "None") moves.Add(inputModel.Move4);
        return moves;
    }

    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}