/*using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;

public class TargetSelectorModel(int target) : INotifyPropertyChanged // Class for recording which pokemon are selected as targets in an event
{
    private readonly int _targetNo = target; // Which pokemon the selector is linked to
    private bool _selected = false;
    public bool Selected // Whether the pokemon is selected as a target
    {
        get => _selected;
        set
        {
            _selected = value;
            Update();
            OnPropertyChanged();
        }
    }
    private List<EventModel> events = []; // Events to update
    public void Attach(EventModel model) // Add new event for updating
    {
        events.Add(model);
        foreach (EventModel ev in events)
        {
            Selected = ev.TargetMons.Contains(_targetNo); // Update selevted based on existing settings
        }
    }
    public void Detach() // Remove attached events
    {
        events = [];
    }
    private void Update() // Update events on change
    {
        foreach (EventModel ev in events)
        {
            if (Selected) // If mon has been selected as a target
            {
                if (!ev.TargetMons.Contains(_targetNo)) ev.TargetMons.Add(_targetNo); // Add target number to target list if not already added
            }
            else
            {
                ev.TargetMons.Remove(_targetNo); // Remove target from target list
            }
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}*/