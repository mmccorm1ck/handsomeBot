using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DialogHostAvalonia;

namespace HandsomeBot.Views;

public partial class BattlePageView : UserControl
{
    public BattlePageView()
    {
        InitializeComponent();
    }

    private async void openerDialog(object? sender, RoutedEventArgs e){
        await DialogHost.Show("openerDialogHost");
    }
}