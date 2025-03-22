using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DialogHostAvalonia;
using ReactiveUI;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using Avalonia.Threading;

namespace HandsomeBot.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public new event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private ViewModelBase _currentPage = new BotTeamPageViewModel(); // Current app page to display

    private int nextPageNumber = 1; // Page number to display next

    private string _currentButtonLabel = "Confirm Team"; // Label to display on the change page button

    public ViewModelBase CurrentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            OnPropertyChanged();
        }
    }

    public string CurrentButtonLabel
    {
        get => _currentButtonLabel;
        set
        {
            _currentButtonLabel = value;
            OnPropertyChanged();
        }
    }
    
    private bool _mainDialogOpen = false;

    public bool MainDialogOpen
    {
        get => _mainDialogOpen;
        set
        {
            _mainDialogOpen = value;
            OnPropertyChanged();
        }
    }

    private bool _dialogButtonVisible = false;

    public bool DialogButtonVisible
    {
        get => _dialogButtonVisible;
        set
        {
            _dialogButtonVisible = value;
            OnPropertyChanged();
        }
    }

    private string _dialogMessage = "";

    public string DialogMessage
    {
        get => _dialogMessage;
        set
        {
            _dialogMessage = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<PageNumberTemplate> PageNumberList { get; } = new() // Collection of pages to cycle through
    {
        new PageNumberTemplate(typeof(BotTeamPageViewModel), 0, "Confirm Team"),
        new PageNumberTemplate(typeof(OppTeamPageViewModel), 1, "Confirm Team"),
        new PageNumberTemplate(typeof(OpenerPageViewModel), 2, "Begin Battle!"),
        new PageNumberTemplate(typeof(BattlePageViewModel), 3, "Reset")
    };

    public class PageNumberTemplate // Template for pages containing page info
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

    public void NextPage() // Page changing function called when button is pressed
    {
        DialogButtonVisible = false;
        DialogMessage = "Calculating...";
        MainDialogOpen = true;
        if (nextPageNumber == 1 && !File.Exists("Data/newBotTeam.json"))
        {
            DialogButtonVisible = true;
            DialogMessage = "Please load a team";
            return;
        }
        if (nextPageNumber == 2 && !File.Exists("Data/newOppTeam.json"))
        {
            DialogButtonVisible = true;
            DialogMessage = "Please enter a team";
            return;
        }
        Task.Run(() => InstanceCreator());
    }

    void InstanceCreator()
    {
        PageNumberTemplate targetPage = PageNumberList[nextPageNumber];
        var instance = Activator.CreateInstance(targetPage.ModelType);
        Dispatcher.UIThread.Post(() => PageLoader(instance, targetPage));
    }

    void PageLoader(object? instance, PageNumberTemplate? targetPage)
    {
        if (instance is null || targetPage is null)
        {
            MainDialogOpen = false;
            return;
        }
        CurrentPage = (ViewModelBase)instance;
        CurrentButtonLabel = targetPage.ButtonLabel;
        if (nextPageNumber == 3)
        {
            File.Move("Data/newBotTeam.json","Data/botTeam.json",true);
            File.Move("Data/newOppTeam.json","Data/oppTeam.json",true);
            File.Move("Data/newGameInfo.json","Data/gameInfo.json",true);
            nextPageNumber = 0;
            return;
        }
        nextPageNumber++;
        MainDialogOpen = false;
    }
}
