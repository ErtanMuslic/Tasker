using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Tasker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

         List<Project> Projects = new List<Project>();
        public MainWindow()
        {
            InitializeComponent();
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
                Project project = new Project(name.Text, goal.Text, date);
                MessageBox.Show($"Successfuly created Project: {project.name}");
            }
            catch
            {
                MessageBox.Show("All Fields Must Be Filled!");
            }
           
        }

    }
}
