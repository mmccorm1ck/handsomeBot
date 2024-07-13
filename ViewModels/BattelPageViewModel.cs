using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.ViewModels;

public class BattlePageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private string _eventNumber = "Event 1";

    public string eventNumber
    {
        get => _eventNumber;
        set
        {
            _eventNumber = value;
            OnPropertyChanged();
        }
    }
}