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
using System.Windows.Shapes;

namespace Tasker
{
    /// <summary>
    /// Interaction logic for CreateProjectWindow.xaml
    /// </summary>
    public partial class CreateProjectWindow : Window
    {

        public DateTime date { get; set; } //Deadline date 
        
        public CreateProjectWindow()
        {
            InitializeComponent();
        }

        public void CreateProject(object sender, RoutedEventArgs e)
        {
           

            if (name.Text == "" || goal.Text == "" || deadline.Text == "")
            {
                MessageBox.Show("All Fields must be filled"); //If all fields are empty show this message
            }
            else
            {
                try
                {
                    date = DateTime.Parse((deadline.Text).ToString()); //Parse date into String

                }
                catch
                {
                    MessageBox.Show("Invalid Deadline date format! (MM/dd/yyyy)"); //if date input is invalid or wrong format

                }
                if (date != DateTime.MinValue) //if dateTime is empty or uninitialised
                {
                    Close(); //close Window
                }
            }

        }
    }
}
