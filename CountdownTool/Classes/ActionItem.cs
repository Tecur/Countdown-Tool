using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace CountdownTool.Classes
{
    public class ActionItem : INotifyPropertyChanged
    {
        public ActionItem() {
            _startCountdownCommand = new DelegateCommand(_StartCountdown, () => !Running);
        }
        public ActionItem(string title, string description, Color color, TimeSpan timestamp) : this()//, Task action = null)
        {
            _title = title;
            _description = description;
            _color = color;
            RemainingTime = timestamp;
            //_action = action;
        }

        private async void _StartCountdown()
        {
            Running = true;
            Finished = false;

            // NOTE: UTC times used internally to ensure proper operation
            // across Daylight Saving Time changes. An IValueConverter can
            // be used to present the user a local time.

            // NOTE: RemainingTime is the raw data. It may be desirable to
            // use an IValueConverter to always round up to the nearest integer
            // value for whatever is the least-significant component displayed
            // (e.g. minutes, seconds, milliseconds), so that the displayed
            // value doesn't reach the zero value until the timer has completed.

            DateTime startTime = DateTime.UtcNow, endTime = startTime + Duration;
            TimeSpan remainingTime, interval = TimeSpan.FromMilliseconds(100);

            StartTime = startTime;
            remainingTime = startTime - endTime;

            while (!Finished)
            {
                RemainingTime = remainingTime;
                if (RemainingTime < interval)
                {
                    interval = RemainingTime;
                }

                // NOTE: arbitrary update rate of 100 ms (initialized above). This
                // should be a value at least somewhat less than the minimum precision
                // displayed (e.g. here it's 1/10th the displayed precision of one
                // second), to avoid potentially distracting/annoying "stutters" in
                // the countdown.

                await Task.Delay(interval);
                remainingTime = DateTime.UtcNow - endTime;

                if (remainingTime == TimeSpan.Zero) { Finished = true; }
            }

            RemainingTime = TimeSpan.Zero;
            StartTime = null;
            Running = false;
        }

        private string _title;
        public string Title 
        { 
            get { return _title; } 
            set { _UpdateField(ref _title, value); } 
        }

        private string _description;
        public string Description 
        { 
            get { return _description;} 
            set { _UpdateField(ref _description, value); } 
        }

        private Color _color;
        public Color Color 
        { 
            get { return _color; } 
            set { _UpdateField(ref _color, value); } 
        }

        //private Action _action;
        //public Task Action { get { return _action; } set { _UpdateField(ref _action, value); } }

        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get { return _duration; }
            set { _UpdateField(ref _duration, value); }
        }

        private DateTime? _startTime;
        public DateTime? StartTime
        {
            get { return _startTime; }
            private set { _UpdateField(ref _startTime, value); }
        }

        private TimeSpan _remainingTime;
        public TimeSpan RemainingTime
        {
            get { return _remainingTime; }
            private set { _UpdateField(ref _remainingTime, value); }
        }

        private bool _running;
        public bool Running
        {
            get { return _running; }
            private set { _UpdateField(ref _running, value, _OnRunningChanged); }
        }

        private bool _finished;
        public bool Finished
        {
            get { return _finished; }
            private set { _UpdateField(ref _finished, value); }
        }

        private void _OnRunningChanged(bool obj)
        {
            _startCountdownCommand.RaiseCanExecuteChanged();
        }

        private readonly DelegateCommand _startCountdownCommand;
        public ICommand StartCountdownCommand { get { return _startCountdownCommand; } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void _UpdateField<T>(ref T field, T newValue,
            Action<T> onChangedCallback = null,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return;
            }

            T oldValue = field;

            field = newValue;
            onChangedCallback?.Invoke(oldValue);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
