using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.IO;
using HandsomeBot.Models;

namespace HandsomeBot.ViewModels;

public class SettingsPageViewModel : ViewModelBase, INotifyPropertyChanged
{
    public SettingsPageViewModel(GameModel game)
    {
        TheGame = game;
        //LoadSettings();
    }
    public new event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //SaveSettings();
    }

    private GameModel _theGame = new();

    public GameModel TheGame
    {
        get => _theGame;
        set
        {
            _theGame = value;
            OnPropertyChanged();
        }
    }

    /*private string _serverUrl = "";

    public string ServerUrl
    {
        get => _serverUrl;
        set
        {
            _serverUrl = value;
            OnPropertyChanged();
        }
    }

    public string settingsFileName = "Data/serverSettings.json";

    public void LoadSettings()
    {
        string settingsJsonString = "";
        try
        {
            using (StreamReader sr = File.OpenText(settingsFileName))
            {
                settingsJsonString = sr.ReadToEnd();
                sr.Close();
            }
        }
        catch
        {
            return;
        }
        if (settingsJsonString == "")
        {
            return;
        }
        ServerSettings tempSettings = JsonSerializer.Deserialize<ServerSettings>(settingsJsonString)!;
        if (tempSettings.serverUrl == null || tempSettings.serverUrl == "") return;
        ServerUrl = tempSettings.serverUrl;
    }

    public void SaveSettings()
    {
        var options = new JsonSerializerOptions {WriteIndented = true};
        ServerSettings tempSettings = new ServerSettings { serverUrl = ServerUrl };
        using (StreamWriter sw = File.CreateText(settingsFileName))
        {
            string settingsJsonString = JsonSerializer.Serialize(tempSettings, options);
            sw.Write(settingsJsonString);
            sw.Close();
        }
    }

    public class ServerSettings()
    {
        public string? serverUrl;
    }*/
}