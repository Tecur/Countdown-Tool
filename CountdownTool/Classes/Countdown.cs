using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CountdownTool.Classes
{
    public class Countdown
    {
        private TimeSpan _counter;
        private DateTime _zero;
        private bool _hold;

        internal class HoldChangedEventArgs : EventArgs {
            public TimeSpan Counter;
            public DateTime Zero;
            public bool OldHold;
            public bool NewHold;
        }

        private event EventHandler<HoldChangedEventArgs> _holdChanged;

        public Countdown() {
            _holdChanged += OnHoldChanged;
            Hold = true;
            Counter = TimeSpan.Zero;
            Zero = DateTime.MinValue;
        }
        public Countdown(TimeSpan counter) :this() { 
            Counter = counter;
        }

        public Countdown(TimeSpan counter, bool hold) : this(counter) {
            Hold = hold;
        }

        public TimeSpan Counter { get { return _counter; } set { _counter = value; } }

        public bool Hold { 
            get { 
                return _hold; 
            } 
            set { 
                var __oldhold = _hold;
                _hold = value;
                _holdChanged(this, 
                    new HoldChangedEventArgs { 
                        Counter = this.Counter,
                        OldHold = __oldhold,
                        NewHold = this.Hold,
                        Zero = this.Zero 
                    }); 
                } 
        }

        public DateTime Zero { get { return _zero; } set { _zero = value; } }

        public override string ToString() { 
            if (Hold)
            {
                return Counter.ToString();
            }
            else
            { 
                return (Zero - DateTime.Now).ToString(); 
            }
        }

        private void OnHoldChanged(object sender, HoldChangedEventArgs e)
        {
            if (e.OldHold == e.NewHold) { return; }
            if (e.NewHold == true)
            {
                // Timespan Counter becomes primary var
                Counter = Zero - DateTime.Now;
                Zero = DateTime.MinValue;
            }
            else if (e.NewHold == false)
            {
                // Datetime Zero becomes primary var
                Zero = DateTime.Now + Counter;
                Counter = TimeSpan.Zero;
            }
        }
    }
}
