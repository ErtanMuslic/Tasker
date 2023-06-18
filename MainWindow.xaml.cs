using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
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
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tasker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<Project> Projects { get; set; }

        public ObservableCollection<Task> Filtered_Tasks {get; set; }
        public string[] Members { get; set; }

        public ObservableCollection<string> Filters { get; set; }

        public DispatcherTimer Timer; //Timer that checks deadline every 10 sec

        public int SelectedIndex = 0; // Index of the element that will be selected when app starts for the first time

        public int TaskIndex = 0; //Index of the task inside Project

        public MySqlConnection connection { get; set; } 

        private Border SelectedBorder; //Task on which the mouse is placed on


        public MainWindow()
        {
            InitializeComponent();

            Members = new string[]
            {
                "Select Member",
                "Ertan",
                "Ramiz",
                "Ermin",
                "Marko",
                "Amel",
                "Samir",
            };


            string connectionString = "Server=localhost;Database=tasker;Uid=root;Pwd='';";

            connection = new MySqlConnection(connectionString);

            GetProjectsFromDatabase();

            foreach(Project project in Projects)
            {
                foreach(Task task in project.Tasks)
                {
                    task.Members = Members;
                }
            }

            //Projects = new ObservableCollection<Project>()
            //{
            //    new Project {Name = "Create Project"},
            //    new Project {Name = "Project 1", Goal= "sadasd",Deadline = new DateTime(2023,10,10), Tasks = new ObservableCollection<Task>() { new Task {Name = "Task1",Description= "dasd",Priority = 1,Date = new DateTime(2/2/2222), Members = new List<string>() {"Select Member","Ertan","Ramiz","Ermin","Marko","Amel","Samir",}, member ="Ertan",Comments = new ObservableCollection<Comment>() { new Comment { MemberName = "Ertan" , Text="No Comment"} } } } }
            //};

            cbx.ItemsSource = Projects; //Bind Projects List to ComboBox 
            cbx.SelectedIndex = Projects.Count - 1; // Initialy ComboBox will point to the "Create Project" which is on index 0
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




        
        //DATABASE OPERATIONS:

        public void StoreToDataBase(Project project, ObservableCollection<Task> task)
        {
            string query = "INSERT INTO project (Name, Goal, Deadline,Tasks) VALUES (@Name, @Goal,@Deadline,@Tasks)";

            connection.Open();

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", project.Name);
            command.Parameters.AddWithValue("@Goal", project.Goal);
            command.Parameters.AddWithValue("@Deadline", project.Deadline);
            command.Parameters.AddWithValue("@Tasks", System.Text.Json.JsonSerializer.Serialize(task));


            command.ExecuteNonQuery();
            connection.Close();
        }


        public  void GetProjectsFromDatabase()
        {
            string query = "SELECT * FROM project";

            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection); ;

            Projects = new ObservableCollection<Project>();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = reader.GetString("Name");
                    string goal = reader.GetString("Goal");
                    DateTime deadline = reader.GetDateTime("Deadline");
                    Task[] tasks = System.Text.Json.JsonSerializer.Deserialize<Task[]>(reader.GetString("Tasks"));
                    Project project = new Project {Name= name,Goal= goal,Deadline= deadline,Tasks=new ObservableCollection<Task>(tasks) };
                    Projects.Add(project);
                }
                
            }
            connection.Close();

        }

        public void AddTaskToDatabase(string name,Task task)
        {
            string query = "UPDATE project SET Tasks = JSON_ARRAY_APPEND(Tasks,'$',JSON_OBJECT('name',@tName,'Description',@Desc,'Priority',@Priority,'Date',@Date,'member',@member)) WHERE name = @name";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@tName", task.Name);
            command.Parameters.AddWithValue("@Desc", task.Description);
            command.Parameters.AddWithValue("@Priority", task.Priority);
            command.Parameters.AddWithValue("@Date", task.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@member", task.member);

            

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }


        
        public void UpdateInDatabase(string name,int id)
        {
            string query = "UPDATE project SET name = @name WHERE ID = @id";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", name);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void DeleteTask(string name, int id)
        {
            string query = $"UPDATE project SET Tasks = JSON_REMOVE(Tasks,'$[{id}]') WHERE name = @name";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", name);
            

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }










        //Check Deadline time every 10 seconds
        private void Timer_Tick(object sender, EventArgs e) //Called every 10 seconds
        {
            CheckDeadline(); //Checks the deadline
        }





        private void CheckDeadline()
        {
            if (Projects[SelectedIndex].Tasks == null)
            {
            }
            else
            {
                foreach (Task items in Projects[SelectedIndex].Tasks) //Go through each task
                {
                    if (items.Date.Date == DateTime.Now.AddDays(1).Date) // If date on task is 1 day away from current date
                    {
                        MessageBox.Show($"{items.Name} has 1 day left to complete", "", MessageBoxButton.OK, MessageBoxImage.Information); //Show Warning
                    }
                }
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
                    StoreToDataBase(project, new ObservableCollection<Task>() { });
                    Projects.Add(project); //Add new Project
                    cbx.SelectedIndex = Projects.Count - 1; //Select newly created Project
                    MessageBox.Show($"Successfuly created Project: {project.Name}");
                }

            }
            else
            {

                CheckIndex();
                UpdateInDatabase(cbx.Text,SelectedIndex); //Update Project Name in Database
                Projects[SelectedIndex].Name = cbx.Text; //Update Project Name


            }

        }






        private void Create_Task(object sender, RoutedEventArgs e)
        {
            if (cbx.SelectedIndex == 0)
            {
                MessageBox.Show("You have to create a Project First!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {

                CreateTaskWindow newTask = new CreateTaskWindow(); //Same logic as for Creating Project applies here
                newTask.ShowDialog();


                CheckIndex();
                string name = newTask.name.Text;
                string desc = newTask.description.Text;
                int priority = newTask.Priority;
                DateTime date = newTask.TaskDate;
                string[] members = newTask.Members;
                string member = newTask.Memberscbx.Text;

                if (name != "" && desc != "" && priority != 0 && date != DateTime.MinValue && (date <= Projects[SelectedIndex].Deadline || date > DateTime.Now))
                {
                    Task task = new Task { Name = name, Description = desc, Priority = priority, Date = date, Members = members, member = member, Comments = new ObservableCollection<Comment>() };
                    Projects[SelectedIndex].Tasks.Add(task); //Add newly created Task to the selected Project
                    AddTaskToDatabase(Projects[SelectedIndex].Name, task); //Add task to the project database
                    MessageBox.Show($"Successfully created Task: {task.Name}");
                }
                else
                {
                    MessageBox.Show("Task deadline can not be set  before Project Deadline", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                taskList.ItemsSource = Projects[SelectedIndex].Tasks; //Bind ItemsControl to the selected Project to show all Tasks for that specific Project
            }
        }






        //When Project is Changed
        private void cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            CheckIndex();
            taskList.ItemsSource = Projects[SelectedIndex].Tasks; // When selection is changed remove previous Tasks and add show Tasks for newly selected Project
            Cbx_Filter.SelectedIndex = 0; //Select "Filter" option
            if (cbx.SelectedIndex == 0)
            {
                ProjectName.Visibility = Visibility.Hidden;
                DeadlineTime.Visibility = Visibility.Hidden;
                Goal.Visibility = Visibility.Hidden;
                DueTo.Visibility = Visibility.Hidden;
                CreateProjectText.Visibility = Visibility.Visible;
            }
            else
            {
                CreateProjectText.Visibility = Visibility.Hidden;
                ProjectName.Visibility = Visibility.Visible;
                DeadlineTime.Visibility = Visibility.Visible;
                Goal.Visibility = Visibility.Visible;
                DueTo.Visibility = Visibility.Visible;
                ProjectName.Content = Projects[SelectedIndex].Name; //Add Project Name on the Main Window
                DeadlineTime.Content = Projects[SelectedIndex].Deadline.ToString("dd.MM.yyyy"); //Add Project Deadline Date on the Main Window
                Goal.Content = Projects[SelectedIndex].Goal; //Add Project Goal to the Main Window
            }
        }








        private int CheckIndex()
        {
            if (cbx.SelectedIndex != -1) //Because Selection changed return -1 when we want to edit text in combobox, i save index of the element that we want to change so we can update name
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

            else if (SelectedFilter == 2) //if second filter is selected (Priority DESC)
            {
                Filtered_Tasks = new ObservableCollection<Task>(Projects[SelectedIndex].Tasks.OrderByDescending(p => p.Priority)); //Copy Tasks from Selected Project and filter them by Priority then set them to Filtered_Tasks
                Projects[SelectedIndex].Tasks.Clear(); //Clear unfiltered Tasks 
                foreach (var items in Filtered_Tasks)
                {
                    Projects[SelectedIndex].Tasks.Add(items); //Add them in order of Priority
                }
            }

            else if (SelectedFilter == 3) // if third filter is selected (Date ASC)
            {
                Filtered_Tasks = new ObservableCollection<Task>(Projects[SelectedIndex].Tasks.OrderBy(p => p.Date)); //Copy Tasks from Selected Project and filter them by Priority then set them to Filtered_Tasks
                Projects[SelectedIndex].Tasks.Clear(); //Clear unfiltered Tasks 
                foreach (var items in Filtered_Tasks)
                {
                    Projects[SelectedIndex].Tasks.Add(items); //Add them in order of Priority
                }
            }

            else if (SelectedFilter == 4) // if fourth filter is selecter (Date DESC)
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
            border.BorderThickness = new Thickness(2);
            border.BorderBrush = new SolidColorBrush(Colors.Green);
            SelectedBorder = border;
            Task selectedTask = (Task)border.DataContext;
            TaskIndex = Projects[SelectedIndex].Tasks.IndexOf(selectedTask); //Get Index of task that the mouse is placed over
        }



        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            SelectedBorder.BorderBrush = new SolidColorBrush(Colors.Black);
            SelectedBorder.BorderThickness = new Thickness(1);
        }





        private void Remove_Task(object sender, MouseButtonEventArgs e)  //Left Click
        {
            Border border = (Border)sender; //Get Clicked Border from sender casted as Border 
            Task selectedTask = (Task)border.DataContext; //Convert DataContex of border as Task
            Projects[SelectedIndex].Tasks.Remove(selectedTask); // Delete Selected Task
            DeleteTask(Projects[SelectedIndex].Name, TaskIndex);

        }






        private void Update_Task(object sender, MouseButtonEventArgs e)  //Right Click
        {
            Border border = (Border)sender; //Get clicked Border from sender casted as Border
            Task selectedTask = (Task)border.DataContext; //Convert DataContex of border as Task

            CreateTaskWindow updateTask = new CreateTaskWindow(); //Create new Window to Update Task
            updateTask.ShowDialog();

            string name = updateTask.name.Text;  //Get updated data
            string desc = updateTask.description.Text;
            int priority = updateTask.Priority;
            DateTime date = updateTask.TaskDate;
            string[] members = updateTask.Members;
            string member = updateTask.Memberscbx.Text;

            if (name != "" && desc != "" && priority != 0 && date != DateTime.MinValue) //Check if all fields are filled correctly
            {
                Task task = new Task { Name = name, Description = desc, Priority = priority, Date = date, Members = members, member = member }; //Create new Task
                Projects[SelectedIndex].Tasks.Add(task); //Add newly updated Task to the selected Project
                AddTaskToDatabase(Projects[SelectedIndex].Name, task); //Add task to the database
                Projects[SelectedIndex].Tasks.Remove(selectedTask); //Remove Old Task 
                DeleteTask(Projects[SelectedIndex].Name,TaskIndex); //Remove Task From database
                MessageBox.Show($"Successfully updated Task: {task.Name}");
            }


        }





        private void Add_Comment(object sender, RoutedEventArgs e) //Add Comment to specific Task
        {

            TextBox text = FindChild<TextBox>(SelectedBorder, "comment_Text"); //Get text to be sent as a comment
            ComboBox name = FindChild<ComboBox>(SelectedBorder, "comment_Name"); // Get name of a user which adds a comment

            if (name.SelectedIndex == 0) //if "Select Member" is selected when the button is pressed 
            {
                MessageBox.Show("You must select a member", "Member not selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                string commentName = name.Text;
                string commentText = text.Text;

                Comment comment = new Comment { MemberName = commentName, Text = commentText }; //Add new comment 

                Projects[SelectedIndex].Tasks[TaskIndex].Comments.Add(comment); //Add comment to the selected Task
                text.Text = string.Empty; //Empty comment TextBox
            }





        }
        private T FindChild<T>(DependencyObject parent, string childName) where T : FrameworkElement //Recursively search element by name
        {                                                                                            //Used to get access to TextBox which is deeply nested
            if (parent == null)                                                                      //in taskList. Could not access it otherwise
                return null;                                                                         

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