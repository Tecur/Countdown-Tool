using CountdownTool.Classes;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CountdownTool
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ViewModel VM { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            /*
            Loaded += (sender, args) =>
            {
                Wpf.Ui.Appearance.Watcher.Watch(
                    this,                                  // Window class
                    Wpf.Ui.Appearance.BackgroundType.Mica, // Background type
                    true                                   // Whether to change accents automatically
                );
            };
            */

            VM = new ViewModel();
            VM.Duration = TimeSpan.FromSeconds(10);
            VM.StartCountdownCommand.Execute(null);

            DataContext = VM;
        }
    }

    public class ViewModel : INotifyPropertyChanged
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

        public ViewModel()
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

    class UtcToLocalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;

                return dateTime.ToLocalTime();
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;

                return dateTime.ToUniversalTime();
            }

            return Binding.DoNothing;
        }
    }

    class TimeSpanRoundUpConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan && parameter is TimeSpan))
            {
                return Binding.DoNothing;
            }

            return RoundUpTimeSpan((TimeSpan)value, (TimeSpan)parameter);
        }

        private static TimeSpan RoundUpTimeSpan(TimeSpan value, TimeSpan roundTo)
        {
            if (value < TimeSpan.Zero) return RoundUpTimeSpan(-value, roundTo);

            double quantization = roundTo.TotalMilliseconds, input = value.TotalMilliseconds;
            double normalized = input / quantization;
            int wholeMultiple = (int)normalized;
            double fraction = normalized - wholeMultiple;

            return TimeSpan.FromMilliseconds((fraction == 0 ? wholeMultiple : wholeMultiple + 1) * quantization);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class RunningToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                return Binding.DoNothing;
            }

            return ConvertToString((bool)value);
        }

        private static string ConvertToString(bool value)
        {
            return value? " C" : "H";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class SignToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan))
            {
                return Binding.DoNothing;
            }

            return ConvertToString((TimeSpan)value);
        }

        private static string ConvertToString(TimeSpan value)
        {
            return value < TimeSpan.Zero ? "T- " : "T+ ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
