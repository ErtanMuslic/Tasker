﻿using System;
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
            Projects = new ObservableCollection<Project>()
            {
                new Project {Name = "Create Project",Goal="",Deadline = new DateTime(6/12/2000)}
            };
            DataContext = this; //Bind Projects List to ComboBox 
        }

        private void Create_Project(object sender, RoutedEventArgs e)
        {
            CreateProjectWindow newProject = new CreateProjectWindow();
            newProject.ShowDialog();  //Main Window will stay open until second window is closed

            string name = newProject.name.Text; //get name input from second window
            string goal = newProject.goal.Text; //get goal input from second window
            DateTime deadline = newProject.date; //get deadline date from second window

            newProject.Close(); //close second window

           
             Project project = new Project { Name = name, Goal = goal, Deadline = deadline };
             Projects.Add(project);
             MessageBox.Show($"Successfuly created Project: {project.Name}");
                          
        }
    }
}

        


           
        

    

