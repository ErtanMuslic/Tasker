using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker
{
    public class Project
    {
        public string Name {  get; set; }
        public string Goal { get; set; }
        public DateTime Deadline { get; set; }

        public ObservableCollection<Task> Tasks { get; set; }
        
    }
}
