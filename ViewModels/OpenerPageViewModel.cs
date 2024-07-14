
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.ViewModels;

public class OpenerPageViewModel : ViewModelBase
{
    
    private string[] _opponentsPokemon = [
        "Pokemon A",
        "Pokemon B",
        "pokemon C",
        "pokemon D",
        "Pokemon E",
        "Pokemon F",
    ];
    public string[] OpponentsPokemon
    {
    get => _opponentsPokemon;
        set
        {
            _opponentsPokemon = value;
        }
    }
}
