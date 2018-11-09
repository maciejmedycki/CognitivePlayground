using Hodor.Model.Messages;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;

namespace Hodor
{
    /// <summary>
    ///    Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowContentRendered(object sender, EventArgs e)
        {
            Messenger.Default?.Send(new StartupMessage());
        }
    }
}