using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia.Threading;
using System.Text.Json;
using HandsomeBot.Models;

namespace HandsomeBot.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public MainWindowViewModel()
    {
        TheGame = LoadData();
        CurrentPage = new SettingsPageViewModel(TheGame, AllOptions); // Initialise with settings page
    }
    public new event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private GameModel _theGame = new();

    public GameModel TheGame // Holds all game date
    {
        get => _theGame;
        set
        {
            _theGame = value;
            OnPropertyChanged();
        }
    }
    private AllOptionsModel _allOptions = new();

    public AllOptionsModel AllOptions // Holds lists of options for dropdown menus
    {
        get => _allOptions;
        set
        {
            _allOptions = value;
            OnPropertyChanged();
        }
    }
    private ViewModelBase _currentPage = new(); // Current app page to display

    private int nextPageNumber = 1; // Page number to display next

    private string _currentButtonLabel = "Save Settings"; // Label to display on the change page button

    public ViewModelBase CurrentPage // Holds currently displayed page
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            OnPropertyChanged();
        }
    }

    public string CurrentButtonLabel // Label for next page button
    {
        get => _currentButtonLabel;
        set
        {
            _currentButtonLabel = value;
            OnPropertyChanged();
        }
    }

    private bool _mainDialogOpen = false;

    public bool MainDialogOpen // Sets whether the pop-up is shown
    {
        get => _mainDialogOpen;
        set
        {
            _mainDialogOpen = value;
            OnPropertyChanged();
        }
    }

    private bool _dialogButtonVisible = false;

    public bool DialogButtonVisible // Sets whether the pop-up has a button to close it with
    {
        get => _dialogButtonVisible;
        set
        {
            _dialogButtonVisible = value;
            OnPropertyChanged();
        }
    }

    private string _dialogMessage = "";

    public string DialogMessage // Sets the message shown on the pop-up
    {
        get => _dialogMessage;
        set
        {
            _dialogMessage = value;
            OnPropertyChanged();
        }
    }
    string dataFileName = "Data/data.json"; // Path to where TheGame is stored

    public ObservableCollection<PageNumberTemplate> PageNumberList { get; } = new() // Collection of pages to cycle through
    {
        new PageNumberTemplate(typeof(SettingsPageViewModel), 0, "Save Settings"),
        new PageNumberTemplate(typeof(BotTeamPageViewModel), 1, "Confirm Team"),
        new PageNumberTemplate(typeof(OppTeamPageViewModel), 2, "Confirm Team"),
        new PageNumberTemplate(typeof(OpenerPageViewModel), 3, "Begin Battle!"),
        new PageNumberTemplate(typeof(BattlePageViewModel), 4, "Reset")
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
        MainDialogOpen = true; // Shows loading pop-up while loading next page
        Task.Run(() => InstanceCreator());
    }

    void InstanceCreator() // Creates instance of next page to be opened
    {
        PageNumberTemplate targetPage = PageNumberList[nextPageNumber];
        var instance = Activator.CreateInstance(targetPage.ModelType, TheGame, AllOptions);
        Dispatcher.UIThread.Post(() => PageLoader(instance, targetPage));
    }

    async void PageLoader(object? instance, PageNumberTemplate? targetPage) // Updates display with new page instance
    {
        if (instance is null || targetPage is null) // Return if null instance
        {
            MainDialogOpen = false;
            return;
        }
        if (nextPageNumber == 2) // Load options data from server if next page is OppTeamView
        {
            try
            {
                await AllOptions.UpdateInfo(TheGame);
            }
            catch
            {
                DialogMessage = "Could not connect to server, check address and server status"; // Show error if fetch from server fails
                DialogButtonVisible = true;
                return;
            }
        }
        CurrentPage = (ViewModelBase)instance; // Updates page being displayed
        CurrentButtonLabel = targetPage.ButtonLabel;
        if (nextPageNumber == 4) // Save team info to prev if battle started
        {
            TheGame.BotTeamPrev = TheGame.BotTeam;
            TheGame.OppTeamPrev = TheGame.OppTeam;
            nextPageNumber = 1; // Next page button will reset to BotTeamView
            return;
        }
        SaveData();
        nextPageNumber++;
        MainDialogOpen = false; // Close pop-up
    }
    GameModel LoadData() // Load TheGame data
    {
        GameModel temp = new() { }; // Initialise empty gameModel for early returns
        string dataJsonString = "";
        try
        {
            using (StreamReader sr = File.OpenText(dataFileName)) // Try to read file
            {
                dataJsonString = sr.ReadToEnd();
                sr.Close();
            }
        }
        catch
        {
            return temp; // Return empty gameModel on fail
        }
        if (dataJsonString == "")
        {
            return temp; // Return empty gameModel on empty file
        }
        temp = JsonSerializer.Deserialize<GameModel>(dataJsonString)!; // Read file into gameModel
        return temp;
    }
    void SaveData() // Saves TheGame data
    {
        var options = new JsonSerializerOptions {WriteIndented = true};
        using (StreamWriter sw = File.CreateText(dataFileName))
        {
            string dataJsonString = JsonSerializer.Serialize(TheGame, options);
            sw.Write(dataJsonString);
            sw.Close();
        }
    }
}
