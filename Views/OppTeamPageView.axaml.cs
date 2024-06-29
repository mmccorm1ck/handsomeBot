using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HandsomeBot.Views;

public partial class OppTeamPageView : UserControl
{
    public OppTeamPageView()
    {
        InitializeComponent();
    }

    public void ConfirmTeam(object source, RoutedEventArgs args)
    {
        Debug.WriteLine("Click!");
    }

    public void LoadPrevious(object source, RoutedEventArgs args)
    {
        Debug.WriteLine("Clack!");
    }
}
