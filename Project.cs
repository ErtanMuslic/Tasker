using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Tasker
{
    public class Project: INotifyPropertyChanged
    {
        private string name;
        public string Name {

            get {
                return name;
            } set
            {
                if(name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            } 
        }
        public string Goal { get; set; }
        public DateTime Deadline { get; set; }

        public ObservableCollection<Task> Tasks { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName) //Notifies us whenever the property of Name (in this case) is changed.Used to update names in ComboBox
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
