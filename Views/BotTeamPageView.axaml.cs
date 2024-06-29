using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HandsomeBot.Views;

public partial class BotTeamPageView : UserControl
{
    public BotTeamPageView()
    {
        InitializeComponent();
    }

    public void ConfirmTeam(object source, RoutedEventArgs args)
    {
        Debug.WriteLine("Click!");
    }

    public void LoadPaste(object source, RoutedEventArgs args)
    {
        Debug.WriteLine("Clock!");
    }

    public void LoadPrevious(object source, RoutedEventArgs args)
    {
        Debug.WriteLine("Clack!");
    }
}
