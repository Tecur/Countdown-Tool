using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace CountdownTool.Classes
{
    public class Countdown : INotifyPropertyChanged
    {
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
                /*if (RemainingTime < interval)
                {
                    interval = RemainingTime;
                }*/

                // NOTE: arbitrary update rate of 100 ms (initialized above). This
                // should be a value at least somewhat less than the minimum precision
                // displayed (e.g. here it's 1/10th the displayed precision of one
                // second), to avoid potentially distracting/annoying "stutters" in
                // the countdown.

                await Task.Delay(interval);
                remainingTime = DateTime.UtcNow - endTime;
            }

            RemainingTime = TimeSpan.Zero;
            StartTime = null;
            Running = false;
        }

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

        public Countdown()
        {
            _startCountdownCommand = new DelegateCommand(_StartCountdown, () => !Running);
        }

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
