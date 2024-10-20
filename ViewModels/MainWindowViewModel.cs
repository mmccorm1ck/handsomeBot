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

namespace HandsomeBot.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public MainWindowViewModel()
    {
        Envoke();
    }

    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

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
    
    private bool _loadTeamErrorOpen = false;

    public bool LoadTeamErrorOpen
    {
        get => _loadTeamErrorOpen;
        set
        {
            _loadTeamErrorOpen = value;
            OnPropertyChanged();
        }
    }
    public Process p = new Process();

    public void Envoke()
    {
        Console.WriteLine("Setting up process");
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.WorkingDirectory = "S:/Documents/Projects/Programming/HandsomeBot/Javascript";
        p.StartInfo.Arguments = "npm run dev";
        Console.WriteLine("Starting event listener");
        p.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine(e.Data);
            }
        });
        Console.WriteLine("Starting process");
        p.Start();
        Console.WriteLine("Listening for output");
        p.BeginOutputReadLine();
        p.WaitForExit();
        p.Close();
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
        PageNumberTemplate targetPage = PageNumberList[nextPageNumber];
        if (targetPage is null) return;
        if (nextPageNumber == 1 && !File.Exists("Data/newBotTeam.json"))
        {
            LoadTeamErrorOpen = true;
            return;
        }
        if (nextPageNumber == 2 && !File.Exists("Data/newOppTeam.json"))
        {
            LoadTeamErrorOpen = true;
            return;
        }
        var instance = Activator.CreateInstance(targetPage.ModelType);
        if (instance is null) return;
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
    }
}
