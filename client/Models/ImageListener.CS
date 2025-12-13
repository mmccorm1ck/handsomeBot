using Avalonia.Media.Imaging;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;

public class ImageListener : INotifyPropertyChanged // Listener for updating sprite images to be displayed
{
    private string _imageUri = "Assets/None.png";
    public string ImageUri // Path to saved image in memory
    {
        get => _imageUri;
        set
        {
            _imageUri = value;
            LoadImage(); // Load new sprite when updated
        }
    }
    public string PrevUri = ""; // For checking if image has been updated
    private Bitmap _image = OpenImage("Assets/None.png"); // Blank image by default
    public Bitmap Image // The sprite to display
    {
        get => _image;
        set
        {
            _image = value;
            OnPropertyChanged();
        }
    }
    private string _borderColour = "White";
    public string BorderColour
    {
        get => _borderColour;
        set
        {
            _borderColour = value;
            OnPropertyChanged();
        }
    }
    private bool _borderVisible = false;
    public bool BorderVisible
    {
        get => _borderVisible;
        set
        {
            _borderVisible = value;
            OnPropertyChanged();
        }
    }
    private string _textColour = "LightBlue";
    public string TextColour
    {
        get => _textColour;
        set
        {
            _textColour = value;
            OnPropertyChanged();
        }
    }
    public void Update(string uri, string status, string position) // Called to update sprite
    {
        ImageUri = uri;
        switch (status)
        {
            case "Burn":
                BorderColour = "Orange";
                BorderVisible = true;
                break;
            case "Freeze":
            case "Frostbite":
                BorderColour = "LightBlue";
                BorderVisible = true;
                break;
            case "Paralysis":
                BorderColour = "Yellow";
                BorderVisible = true;
                break;
            case "Poison":
                BorderColour = "Purple";
                BorderVisible = true;
                break;
            case "Badly Poisoned":
                BorderColour = "DarkViolet";
                BorderVisible = true;
                break;
            case "Sleep":
                BorderColour = "Blue";
                BorderVisible = true;
                break;
            default:
                BorderVisible = false;
                break;
        }
        TextColour = position switch
        {
            "Active" => "LightGreen",
            "KO" => "LightRed",
            "Not Brought" => "Gray",
            _ => "LightBlue"
        };
    }
    public void LoadImage() // Load a new sprite
    {
        if (ImageUri == PrevUri) return; // Do nothing if image path has not changed
        PrevUri = ImageUri;
        string uri;
        if (!File.Exists(ImageUri)) uri = "Assets/None.png"; // Blank image if cannot find sprite
        else uri = ImageUri;
        Image = OpenImage(uri); // Load new sprite
    }
    public static Bitmap OpenImage(string uri) // Opens the sprite
    {
        FileStream stream;
        try
        {
            stream = File.Open(uri, FileMode.Open); // Open stream to read in image
        }
        catch
        {
            stream = File.Open("Assets/None.png", FileMode.Open); // Open blank image if specified image not found
        }
        Bitmap image = new(stream); // Set image to loaded sprite
        stream.Close();
        return image;
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}