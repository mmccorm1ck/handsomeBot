using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using HandsomeBot.Models;

namespace HandsomeBot.ViewModels;

public class OppTeamPageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public OppTeamPageViewModel(GameModel game, AllOptionsModel options)
    {
        TheGame = game;
        AllOptions = options;
        TheGame.ZoroPresent = false;
        for (int i = 0; i < 6; i++)
        {
            if (TheGame.OppTeam[i].BaseForme != "")
            {
                TheGame.OppTeam[i].Name = TheGame.OppTeam[i].BaseForme;
            }
            TheGame.OppTeam[i].MegaAbility = null;
            Sprites.Add(new());
            TheGame.OppTeam[i].Attach(Sprites[i]);
            if (TheGame.OppTeam[i].Name.Contains("Zoroark") || TheGame.OppTeam[i].Name.Contains("Zorua"))
            {
                TheGame.ZoroPresent = true;
            }
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private GameModel _theGame = new();

    public GameModel TheGame // Holds all game data
    {
        get => _theGame;
        set
        {
            _theGame = value;
            OnPropertyChanged();
        }
    }
    private AllOptionsModel _allOptions = new();
    public AllOptionsModel AllOptions // Holds all option lists, used for list of pokemon names
    {
        get => _allOptions;
        set
        {
            _allOptions = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<ImageListener> _sprites = [];
    public ObservableCollection<ImageListener> Sprites // Image listeners for updating sprites
    {
        get => _sprites;
        set
        {
            _sprites = value;
            OnPropertyChanged();
        }
    }

    public void LoadPrev() // Loads team from previous game, used for best of 3 etc
    {
        TheGame.OppTeam = TheGame.OppTeamPrev;
        for (int i = 0; i < 6; i++)
        {
            TheGame.OppTeam[i].Attach(Sprites[i]); // Reattach image listeners
        }
    }
}
