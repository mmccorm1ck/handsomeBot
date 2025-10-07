using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandsomeBot.Models;

    public class TargetSelectorModel(int target)
    {
        private readonly int _targetNo = target;
        private bool _selected = false;
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                Update();
                OnPropertyChanged();
            }
        }
        private List<EventModel> events = [];
        public void Attach(EventModel model)
        {
            events.Add(model);
            foreach (EventModel ev in events)
            {
                Selected = ev.TargetMons.Contains(_targetNo);
            }
        }
        public void Detach()
        {
            events = [];
        }
        private void Update()
        {
            foreach (EventModel ev in events)
            {
                if (Selected)
                {
                    if (!ev.TargetMons.Contains(_targetNo)) ev.TargetMons.Add(_targetNo);
                }
                else
                {
                    ev.TargetMons.Remove(_targetNo);
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged; // Event handler to update UI when variables change
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Function to trigger above event handler
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }