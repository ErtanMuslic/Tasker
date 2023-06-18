using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Tasker
{
    /// <summary>
    /// Interaction logic for CreateTaskWindow.xaml
    /// </summary>
    public partial class CreateTaskWindow : Window
    {
        public DateTime TaskDate { get; set; }

        public int Priority;

        public string[] Members { get; set; }





        public CreateTaskWindow()
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

            Memberscbx.ItemsSource = Members;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (name.Text == "" || description.Text == "" || priority.Text == "" || Memberscbx.SelectedIndex == 0)
            {
                MessageBox.Show("All Fields must be filled", "Empty Fields", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                try
                {
                    TaskDate = DateTime.Parse((date.Text).ToString()); //Parse date into String

                }
                catch
                {
                    MessageBox.Show("Invalid Deadline date format! (MM/dd/yyyy)", "Invalid Date Format", MessageBoxButton.OK, MessageBoxImage.Warning); //if date input is invalid or wrong format

                }
                if (!int.TryParse(priority.Text, out Priority))
                {
                    MessageBox.Show("Priority must be a number", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                if (TaskDate != DateTime.MinValue && int.TryParse(priority.Text, out Priority)) //if dateTime is not empty or uninitialised
                {
                    Close(); //close Window
                }

            }
        }
    }
}
