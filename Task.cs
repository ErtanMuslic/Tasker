using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker
{
    public class Task : INotifyPropertyChanged
    {
        private string name;
        public string Name {
            get { return name; }
            set 
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            } 
        }
        public string Description { get; set; }
        public int Priority { get; set; }
        public DateTime Date { get; set; }
        public List<string> Members { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
