using CountdownTool.Classes;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace CountdownTool
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ViewModel() { 
            Countdown = new Countdown();

            Countdown.Duration = TimeSpan.FromSeconds(10);
            Countdown.StartCountdownCommand.Execute(null);

            Items = new ObservableCollection<ActionItem> { };
            Items.Add(new ActionItem("Ereignis", "Test-Beschreibung", Colors.White, TimeSpan.FromSeconds(3)));
        }
        public Countdown Countdown { get; set; }

        public ObservableCollection<ActionItem> Items { get; set; }
    }
}
