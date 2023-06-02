using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker
{
    internal class Project
    {
        public string name {  get; set; }
        public string goal { get; set; }
        public DateTime deadline { get; set; }
        public Project(string n,string g,DateTime d)
        {
            this.name = n;
            this.goal = g; 
            this.deadline = d;
        }
    }
}
