using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
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
using System.Xml.Linq;

namespace Tasker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<Project> Projects { get; set; }

        public ObservableCollection<Task> Tasks { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Projects = new ObservableCollection<Project>()
            {
                new Project {Name = "Create Project",Goal="",Deadline = new DateTime(6/12/2000)}
            };
            DataContext = this; //Bind Projects List to ComboBox 

            Tasks = new ObservableCollection<Task>()
            {
                new Task {Name = "Name1",Description ="Description1", Priority=1, Date=new DateTime(6/12/2000)},
                new Task {Name = "Name2",Description ="Description2", Priority=1, Date=new DateTime(6/12/2000)},
                new Task {Name = "Name1",Description ="Description1", Priority=1, Date=new DateTime(6/12/2000)},
                new Task {Name = "Name2",Description ="Description2", Priority=1, Date=new DateTime(6/12/2000)},
                new Task {Name = "Name1",Description ="Description1", Priority=1, Date=new DateTime(6/12/2000)},
                new Task {Name = "Name2",Description ="Description2", Priority=1, Date=new DateTime(6/12/2000)},
                new Task {Name = "Name1",Description ="Description1", Priority=1, Date=new DateTime(6/12/2000)},
                new Task {Name = "Name2",Description ="Description2", Priority=1, Date=new DateTime(6/12/2000)},
                new Task {Name = "Name1",Description ="Description1", Priority=1, Date=new DateTime(6/12/2000)},
                new Task {Name = "Name2",Description ="Description2", Priority=1, Date=new DateTime(6/12/2000)},

            };

            taskList.ItemsSource = Tasks; // Bind taskList to Tasks (ItemsControl)
        }

        private void Create_Project(object sender, RoutedEventArgs e)
        {
            CreateProjectWindow newProject = new CreateProjectWindow();
            newProject.Closed += newProject_Closed; //add event handling method for when second window is closed manualy
            newProject.ShowDialog();  //Main Window will stay open until second window is closed

            string name = newProject.name.Text; //get name input from second window
            string goal = newProject.goal.Text; //get goal input from second window
            DateTime deadline = newProject.date; //get deadline date from second window

            newProject.Close(); //close second window

            if (name != "" && goal != "" && deadline != DateTime.MinValue) // If second window was closed manualy,empty project will be created hence this if statement
            {
                Project project = new Project { Name = name, Goal = goal, Deadline = deadline };
                Projects.Add(project);
                MessageBox.Show($"Successfuly created Project: {project.Name}");
            }

            newProject.Closed -= newProject_Closed; //detach event handler 
                          
        }
        
        private void newProject_Closed(object sender, EventArgs e) //Doesn't really serve a purpose right now
        {
            //MessageBox.Show("You Closed the window");
        }
    }
}

        


           
        

    

