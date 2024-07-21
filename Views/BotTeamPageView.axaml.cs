using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace HandsomeBot.Views;

public partial class BotTeamPageView : UserControl
{
    
    public BotTeamPageView()
    {
        InitializeComponent();
    }

    public void LoadPaste(object source, RoutedEventArgs args)
    {
        if (pasteLink.Text is null)
        {
            return;
        }
        string httpLink = pasteLink.Text;
        Task task = Task.Run(async () => await LoadPasteHtml(httpLink));
    }
    public async Task LoadPasteHtml(string httpLink)
    {
        HttpClient client = new HttpClient();
        string response = await client.GetStringAsync(httpLink);
        Debug.WriteLine(response);
    }

    public void LoadPrevious(object source, RoutedEventArgs args)
    {
        Debug.WriteLine("Clack!");
    }
}

