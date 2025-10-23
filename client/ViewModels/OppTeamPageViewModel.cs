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
        for (int i = 0; i < 6; i++)
        {
            Sprites.Add(new());
            TheGame.OppTeam[i].Attach(Sprites[i]);
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private GameModel _theGame = new();

    public GameModel TheGame
    {
        get => _theGame;
        set
        {
            _theGame = value;
            OnPropertyChanged();
        }
    }
    private AllOptionsModel _allOptions = new();
    public AllOptionsModel AllOptions
    {
        get => _allOptions;
        set
        {
            _allOptions = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<ImageListener> _sprites = [];
    public ObservableCollection<ImageListener> Sprites
    {
        get => _sprites;
        set
        {
            _sprites = value;
            OnPropertyChanged();
        }
    }

    public void LoadPrev()
    {
        TheGame.OppTeam = TheGame.OppTeamPrev;
        for (int i = 0; i < 6; i++)
        {
            TheGame.OppTeam[i].Attach(Sprites[i]);
        }
    }
}
