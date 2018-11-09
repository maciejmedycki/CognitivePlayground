using Hodor.Model.Messages;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace Hodor
{
    /// <summary>
    ///    Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            Messenger.Default?.Send<AppExitingMessage>(new AppExitingMessage());
        }
    }
}