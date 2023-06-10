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
    /// Interaction logic for CreateTaskWindow.xaml
    /// </summary>
    public partial class CreateTaskWindow : Window
    {
        public DateTime TaskDate { get; set; }

        public int Priority;

        public List<string> Members {get; set; }

        
        public CreateTaskWindow()
        {
            InitializeComponent();
            Members = new List<string>()
            {
                "Select Member",
                "Ertan",
                "Ramiz",
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (name.Text == "" || description.Text == "" || priority.Text == "")
            {
                MessageBox.Show("All fields must be filled");
            }
            else
            {
                try
                {
                    TaskDate = DateTime.Parse((date.Text).ToString()); //Parse date into String

                }
                catch
                {
                    MessageBox.Show("Invalid Deadline date format! (MM/dd/yyyy)"); //if date input is invalid or wrong format

                }
                if(!int.TryParse(priority.Text, out Priority))
                {
                    MessageBox.Show("Priority must be a number");
                }
                
                if (TaskDate != DateTime.MinValue && int.TryParse(priority.Text, out Priority)) //if dateTime is not empty or uninitialised
                {
                    Close(); //close Window
                }
            
            }
        }
    }
}
