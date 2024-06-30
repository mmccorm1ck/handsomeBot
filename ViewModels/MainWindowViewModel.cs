using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection.Emit;
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

    private int nextPageNumber = 1;

    private string _currentButtonLabel = "Confirm Team";

    public ViewModelBase currentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            OnPropertyChanged();
        }
    }

    public string currentButtonLabel
    {
        get => _currentButtonLabel;
        set
        {
            _currentButtonLabel = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<PageNumberTemplate> PageNumberList { get; } = new()
    {
        new PageNumberTemplate(typeof(BotTeamPageViewModel), 0, "Confirm Team"),
        new PageNumberTemplate(typeof(OppTeamPageViewModel), 1, "Confirm Team"),
        new PageNumberTemplate(typeof(BattlePageViewModel), 2, "Reset")
    };

    public class PageNumberTemplate
    {
        public PageNumberTemplate(Type type, int pgNum, string lab)
        {
            ModelType = type;
            PageNumber = pgNum;
            ButtonLabel = lab; 
        }

        public int PageNumber { get; }
        public string ButtonLabel { get; }
        public Type ModelType { get; }
    }

    public void NextPage()
    {
        PageNumberTemplate targetPage = PageNumberList[nextPageNumber];
        if (targetPage is null) return;
        var instance = Activator.CreateInstance(targetPage.ModelType);
        if (instance is null) return;
        currentPage = (ViewModelBase)instance;
        currentButtonLabel = targetPage.ButtonLabel;
        if (nextPageNumber == 2)
        {
            nextPageNumber = 0;
            return;
        }
        nextPageNumber++;
    }

}
