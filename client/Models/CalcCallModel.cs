using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;
public class CalcCallModel() : INotifyPropertyChanged // Class to hold info to send to calc server
{
    public int Gen
    {
        get => _gen;
        set
        {
            _gen = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<TeamModel> BotMons
    {
        get => _botMons;
        set
        {
            _botMons = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<TeamModel> OppMons
    {
        get => _oppMons;
        set
        {
            _oppMons = value;
            OnPropertyChanged();
        }
    }
    public FieldModel Field
    {
        get => _field;
        set
        {
            _field = value;
            OnPropertyChanged();
        }
    }
    private int _gen = -1;
    private ObservableCollection<TeamModel> _botMons = [];
    private ObservableCollection<TeamModel> _oppMons = [];
    private FieldModel _field = new();
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}