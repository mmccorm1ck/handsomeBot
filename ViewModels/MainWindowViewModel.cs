using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private ViewModelBase _currentPage = new BotTeamPageViewModel();

    public ViewModelBase currentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            OnPropertyChanged();
        }
    }
}
