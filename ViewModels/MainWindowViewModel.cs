using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace HandsomeBot.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private ViewModelBase _currentPage = new BotTeamPageViewModel();

    private int currentPageNumber = 0;

    public ViewModelBase currentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<PageNumberTemplate> PageNumberList { get; } = new()
    {
        new PageNumberTemplate(typeof(BotTeamPageViewModel), 0),
        new PageNumberTemplate(typeof(OppTeamPageViewModel), 1)
    };

    public class PageNumberTemplate
    {
        public PageNumberTemplate(Type type, int pgNum)
        {
            ModelType = type;
            pageNumber = pgNum; 
        }

        public int pageNumber { get; }
        public Type ModelType { get; }
    }

    public void NextPage()
    {
        PageNumberTemplate targetPage = PageNumberList[currentPageNumber+1];
        if (targetPage is null) return;
        var instance = Activator.CreateInstance(targetPage.ModelType);
        if (instance is null) return;
        currentPage = (ViewModelBase)instance;
        currentPageNumber++;
    }

}
