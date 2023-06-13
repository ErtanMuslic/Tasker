using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using System.Xml.Linq;
using Microsoft.VisualStudio.CommandBars;

namespace Tasker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<Project> Projects { get; set; }

        public ObservableCollection<Task> Tasks { get; set; }
        public ObservableCollection<string> Members { get; set; }

        public ObservableCollection<Task> Filtered_Tasks { get; set; }

        public ObservableCollection<string> Filters { get; set; }

        public DispatcherTimer Timer; //Timer that checks deadline every 10 sec

        public int SelectedIndex = 0; // Index of the element that will be selected when app starts for the first time

        public int TaskIndex = 0;


        public MainWindow()
        {
            InitializeComponent();

            
            Projects = new ObservableCollection<Project>()
            {
                new Project {Name = "Create Project"},
                new Project {Name = "Project 1", Goal= "sadasd",Deadline = new DateTime(2/2/2000), Tasks = new ObservableCollection<Task>() { new Task {Name = "Task1",Description= "dasd",Priority = 1,Date = new DateTime(2/2/2222), Members = new List<string>() {"Select Member","Ertan","Ramiz","Ermin","Marko","Amel","Samir",},Comments = new ObservableCollection<Comment>() { new Comment { MemberName = "Ertan" , Text="Jebi se"} } } } }
            };

            cbx.ItemsSource = Projects; //Bind Projects List to ComboBox 
            cbx.SelectedIndex = Projects.Count - 1; // Initialy ComboBox will point to the "Create Project" which is on index 0


            //Tasks = new ObservableCollection<Task>() //Will be deleted shortly
            //{
            //    new Task {Name = "Name1",Description ="Description1", Priority=1, Date=new DateTime(2000,1,1), Members = new List < string >() { "Select Memeber","Ramiz","Ertan"}},
            //    new Task {Name = "Name2",Description ="Description2", Priority=1, Date=new DateTime(2000,1,1), Members = new List < string >() { "Select Member","Ramiz","Ertan" }},
            //    new Task {Name = "Name1",Description ="Description1", Priority=1, Date=new DateTime(2000,1,1), Members = new List < string >() { "Select Member","Ramiz","Ertan" }},
            //    new Task {Name = "Name2",Description ="Description2", Priority=1, Date=new DateTime(2000,1,1), Members = new List < string >() { "Select Member", "Ramiz", "Ertan" }},
            //    new Task {Name = "Name1",Description ="Description1", Priority=1, Date=new DateTime(2000,1,1), Members = new List < string >() { "Select Member", "Ramiz", "Ertan" }},
            //    new Task {Name = "Name2",Description ="Description2", Priority=1, Date=new DateTime(2000,1,1), Members = new List < string >() { "Select Member", "Ramiz", "Ertan" }},
            //    new Task {Name = "Name1",Description ="Description1", Priority=1, Date=new DateTime(2000,1,1), Members = new List < string >() { "Select Member", "Ramiz", "Ertan" }},
            //    new Task {Name = "Name2",Description ="Description2", Priority=1, Date=new DateTime(2000,1,1), Members = new List < string >() { "Select Member", "Ramiz", "Ertan" }},
            //    new Task {Name = "Name1",Description ="Description1", Priority=1, Date=new DateTime(2000,1,1), Members = new List < string >() { "Select Member", "Ramiz", "Ertan" }},
            //    new Task {Name = "Name2",Description ="Description2", Priority=1, Date=new DateTime(2000,1,1), Members = new List < string >() { "Select Member", "Ramiz", "Ertan" }},
            //    new Task {Name = "Name1",Description ="Description1", Priority=1, Date=new DateTime(2000,1,1), Members = new List < string >() { "Select Member", "Ramiz", "Ertan" }},
            //    new Task {Name = "Name2",Description ="Description2", Priority=1, Date=new DateTime(2000,1,1), Members = new List < string >() { "Select Member", "Ramiz", "Ertan" }},

            //};

            taskList.ItemsSource = Projects[SelectedIndex].Tasks; // Bind taskList to Projects[Index].Tasks(all tasks in the List) (ItemsControl)
            


            Filters = new ObservableCollection<string>() //Create list of available filters
            {   "Filter",
                "^ Priority",
                "v Priority",
                "^ Date",
                "v Date",
            };

            Cbx_Filter.ItemsSource = Filters; //Bind Filters to Filter ComboBox
            Cbx_Filter.SelectedIndex = 0; //Show first fillter on application start

            Timer = new DispatcherTimer(); //Craete new Timer
            Timer.Interval = TimeSpan.FromSeconds(10); // Set interval to check every 10 seconds
            Timer.Tick += Timer_Tick; //Add a function to be called when 10 seconds have passed
            Timer.Start(); //Start Timer


        }


        private void Timer_Tick(object sender, EventArgs e) //Called every 10 seconds
        {
            CheckDeadline(); //Checks the deadline
            GenerateProgressReports(); //Generate progress Reports
        }

        private void CheckDeadline()
        {
            foreach(Task items in Projects[SelectedIndex].Tasks) //Go through each task
            {
                if(items.Date.Date == DateTime.Now.AddDays(1).Date) // If date on task is 1 day away from current date
                {
                    MessageBox.Show($"Warning: {items.Name} has 1 day left to complete"); //Show Warning
                }
            }
        }

        private void GenerateProgressReports()
        {
            foreach(Task items in Projects[SelectedIndex].Tasks)
            {
                string report = $"Task: {items.Name}\n" +
                                $"Description: {items.Description}\n" +
                                $"Priority: {items.Priority}\n" +
                                $"Deadline: {items.Date}\n" +
                                $"Progress: ADD\n";

                Console.WriteLine(report);
            }
        }



        private void Create_Project(object sender, RoutedEventArgs e)
        {
            if (cbx.SelectedIndex == 0)
            {

                CreateProjectWindow newProject = new CreateProjectWindow();
                newProject.ShowDialog();  //Can't Interact with Main Window until second Window is closed

                string name = newProject.name.Text; //get name input from second window
                string goal = newProject.goal.Text; //get goal input from second window
                DateTime deadline = newProject.date; //get deadline date from second window

                newProject.Close(); //close second window

                if (name != "" && goal != "" && deadline != DateTime.MinValue) // If second window was closed manualy,empty project will be created hence this if statement
                {
                    Project project = new Project { Name = name, Goal = goal, Deadline = deadline, Tasks = new ObservableCollection<Task>() };
                    Projects.Add(project); //Add new Project
                    cbx.SelectedIndex = Projects.Count - 1; //Select newly created Project
                    MessageBox.Show($"Successfuly created Project: {project.Name} , Index: {cbx.SelectedIndex}");
                }

            }
            else
            {
                
                CheckIndex();
                Projects[SelectedIndex].Name = cbx.Text; //Update Project Name
                
               
            }
                          
        }
        


        private void Create_Task(object sender, RoutedEventArgs e)
        {
            CreateTaskWindow newTask = new CreateTaskWindow(); //Same logic for Creating Project applies here
            newTask.ShowDialog();

            
            CheckIndex();
            string name = newTask.name.Text;
            string desc = newTask.description.Text;
            int priority = newTask.Priority;
            DateTime date = newTask.TaskDate;
            List<string> members = newTask.Members;

            if(name != "" && desc != "" && priority != 0 && date != DateTime.MinValue)
            {
                Task task = new Task { Name=name,Description =desc , Priority = priority , Date = date ,Members = members , Comments = new ObservableCollection<Comment>()};
                Projects[SelectedIndex].Tasks.Add(task); //Add newly created Task to the selected Project
                MessageBox.Show($"Successfully created Task: {task.Name}");
            }
            taskList.ItemsSource = Projects[SelectedIndex].Tasks; //Bind ItemsControl to the selected Project to show all Tasks for that specific Project
        }




        private void cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            CheckIndex();
            taskList.ItemsSource = Projects[SelectedIndex].Tasks; // When selection is changed remove previous Tasks and add show Tasks for newly selected Project
            Cbx_Filter.SelectedIndex = 0;
        }






        private int CheckIndex()
        {
            if(cbx.SelectedIndex != -1) //Because Selection changed return -1 when we want to edit text in combobox, i save index of the element that we want to change so we can update name
            {
                SelectedIndex = cbx.SelectedIndex;
                return SelectedIndex;
            }
            return -1;
        }






        private void Cbx_Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var SelectedFilter = Cbx_Filter.SelectedIndex; //Get Index of the choosen filter
           

            if (SelectedFilter == 1) //If first filter is selected (Priority ASC)
            {
                
                 Filtered_Tasks = new ObservableCollection<Task>(Projects[SelectedIndex].Tasks.OrderBy(p => p.Priority)); //Copy Tasks from Selected Project and filter them by Priority then set them to Filtered_Tasks
                Projects[SelectedIndex].Tasks.Clear(); //Clear unfiltered Tasks 
                foreach (var items in Filtered_Tasks)
                {
                    Projects[SelectedIndex].Tasks.Add(items); //Add them in order of Priority
                }
            }
             
            else if(SelectedFilter == 2) //if second filter is selected (Priority DESC)
            {
                Filtered_Tasks = new ObservableCollection<Task>(Projects[SelectedIndex].Tasks.OrderByDescending(p => p.Priority)); //Copy Tasks from Selected Project and filter them by Priority then set them to Filtered_Tasks
                Projects[SelectedIndex].Tasks.Clear(); //Clear unfiltered Tasks 
                foreach (var items in Filtered_Tasks)
                {
                    Projects[SelectedIndex].Tasks.Add(items); //Add them in order of Priority
                }
            }

            else if(SelectedFilter == 3) // if third filter is selected (Date ASC)
            {
                Filtered_Tasks = new ObservableCollection<Task>(Projects[SelectedIndex].Tasks.OrderBy(p => p.Date)); //Copy Tasks from Selected Project and filter them by Priority then set them to Filtered_Tasks
                Projects[SelectedIndex].Tasks.Clear(); //Clear unfiltered Tasks 
                foreach (var items in Filtered_Tasks)
                {
                    Projects[SelectedIndex].Tasks.Add(items); //Add them in order of Priority
                }
            }

            else if(SelectedFilter == 4) // if fourth filter is selecter (Date DESC)
            {
                Filtered_Tasks = new ObservableCollection<Task>(Projects[SelectedIndex].Tasks.OrderByDescending(p => p.Date)); //Copy Tasks from Selected Project and filter them by Priority then set them to Filtered_Tasks
                Projects[SelectedIndex].Tasks.Clear(); //Clear unfiltered Tasks 
                foreach (var items in Filtered_Tasks)
                {
                    Projects[SelectedIndex].Tasks.Add(items); //Add them in order of Priority
                }
            }
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            Task selectedTask = border.DataContext as Task;
            TaskIndex = Projects[SelectedIndex].Tasks.IndexOf(selectedTask); //Get Index of task that the mouse is placed over
        }


        private void Remove_Task(object sender, MouseButtonEventArgs e)  //Left Click
        {
            Border border= (Border)sender; //Get Clicked Border from sender casted as Border 
            Task selectedTask = border.DataContext as Task; //Convert DataContex of border as Task
            Projects[SelectedIndex].Tasks.Remove(selectedTask); // Delete Selected Task

        }




        private void Update_Task(object sender, MouseButtonEventArgs e)  //Right Click
        {
            Border border = (Border)sender; //Get clicked Border from sender casted as Border
            Task selectedTask = border.DataContext as Task; //Convert DataContex of border as Task

            CreateTaskWindow updateTask = new CreateTaskWindow(); //Create new Window to Update Task
            updateTask.ShowDialog();

            string name = updateTask.name.Text;  //Get updated data
            string desc = updateTask.description.Text;
            int priority = updateTask.Priority;
            DateTime date = updateTask.TaskDate;
            List<string> members = updateTask.Members;

            if (name != "" && desc != "" && priority != 0 && date != DateTime.MinValue) //Check if all fields are filled correctly
            {
                Task task = new Task { Name = name, Description = desc, Priority = priority, Date = date, Members = members }; //Create new Task
                Projects[SelectedIndex].Tasks.Add(task); //Add newly updated Task to the selected Project
                Projects[SelectedIndex].Tasks.Remove(selectedTask); //Remove Old Task 
                MessageBox.Show($"Successfully updated Task: {task.Name}"); 
            }
            

        }

        private void Add_Comment(object sender, RoutedEventArgs e)
        {
            ItemsControl items = (ItemsControl)FindName("taskList");
            TextBox text = FindChild<TextBox>(items,"comment_Text");
            ComboBox name = FindChild<ComboBox>(items, "comment_Name");

            if (name.SelectedIndex == 0)
            {
                MessageBox.Show("You must select a member");
            }
            else
            {
                string commentName = name.Text;
                string commentText = text.Text;

                Comment comment = new Comment { MemberName = commentName, Text = commentText };

                Projects[SelectedIndex].Tasks[TaskIndex].Comments.Add(comment);
            }
            
            
           

        }
        private T FindChild<T>(DependencyObject parent, string childName) where T : FrameworkElement //Recursively search element by name
        {                                                                                            //Copied from the internet
            if (parent == null)                                                                      //Used to get access to TextBox which is deeply nested 
                return null;                                                                         //in taskList. Could not access it otherwise
                                                                                                     
            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T childElement && childElement.Name == childName)
                {
                    foundChild = childElement;
                    break;
                }
                else
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null)
                        break;
                }
            }

            return foundChild;
        }

        
    }
}

        


           
        

    

