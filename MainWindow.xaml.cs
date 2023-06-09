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
        public MainWindow()
        {
            InitializeComponent();
            Projects = new ObservableCollection<Project>();
            DataContext = this;
        }

    


    private void Button_Click(object sender, RoutedEventArgs e)
        {
            label.Content = "You Clicked The Button";
        }

        private void Create_Project(object sender, RoutedEventArgs e)
        {
            try
            {
                var date = DateTime.Parse((deadline.Text).ToString());
                Project project = new Project { Name= name.Text,Goal = goal.Text, Deadline= date };
                Projects.Add(project);
                MessageBox.Show($"Successfuly created Project: {project.Name}");
            }
            catch
            {
                MessageBox.Show("All Fields Must Be Filled!");
            }
        }
    }
}

        


           
        

    

