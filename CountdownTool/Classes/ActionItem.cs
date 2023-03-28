using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CountdownTool.Classes
{
    internal class ActionItem
    {
        private string _title;
        private string _description;
        private Color _color;
        private long _timestamp;
        private Task _action;

        public ActionItem() { }
        public ActionItem(string title, string description, Color color, long timestamp, Task action)
        {
            _title = title;
            _description = description;
            _color = color;
            _timestamp = timestamp;
            _action = action;
        }

        public string Title { get { return _title; } set { _title = value; } }
        public string Description { get { return _description;} set { _description = value; } }
        public Color Color { get { return _color; } set { _color = value; } }
        public long Timestamp { get { return _timestamp;} set { _timestamp = value; } }
        public Task Action { get { return _action; } set { _action = value; } }
        
    }
}
