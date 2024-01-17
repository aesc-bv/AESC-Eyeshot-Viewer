﻿using System;
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

namespace AESC_Eyeshot_Viewer
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public string LoadingText { get; set; }
        public CloseLoadingWindow CloseLoadingWindow;
        public LoadingWindow()
        {
            InitializeComponent();

            CloseLoadingWindow = new CloseLoadingWindow(CloseWindow);
        }

        private void CloseWindow() => Close();
    }
}
