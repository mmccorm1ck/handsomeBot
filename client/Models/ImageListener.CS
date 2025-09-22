using Avalonia.Media.Imaging;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;

public class ImageListener : INotifyPropertyChanged
{
    private string _imageUri = "Assets/None.png";
    public string ImageUri
    {
        get => _imageUri;
        set
        {
            _imageUri = value;
            LoadImage();
        }
    }
    public string PrevUri = "";
    private Bitmap _image = OpenImage("Assets/None.png");
    public Bitmap Image
    {
        get => _image;
        set
        {
            _image = value;
            OnPropertyChanged();
        }
    }

    public void Update(string uri)
    {
        ImageUri = uri;
    }
    public void LoadImage()
    {
        if (ImageUri == PrevUri) return;
        PrevUri = ImageUri;
        string uri;
        if (!File.Exists(ImageUri)) uri = "Assets/None.png";
        else uri = ImageUri;
        Image = OpenImage(uri);
    }
    public static Bitmap OpenImage(string uri)
    {
        FileStream stream;
        try
        {
            stream = File.Open(uri, FileMode.Open);
        }
        catch
        {
            stream = File.Open("Assets/None1.png", FileMode.Open);
        }
        Bitmap image = new(stream);
        stream.Close();
        return image;
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}