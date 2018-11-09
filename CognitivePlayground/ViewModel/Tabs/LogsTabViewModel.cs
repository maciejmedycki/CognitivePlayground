using Hodor.Model;
using GalaSoft.MvvmLight.Threading;
using log4net;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hodor.ViewModel.Tabs
{
    public class LogsTabViewModel : TabItemViewModelBase
    {
        private static int _maxLogCount = 100;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(LogsTabViewModel));

        public LogsTabViewModel()
        {
            var appenders = LogManager.GetRepository().GetAppenders().Where(a=>a.GetType() == typeof(LogsAppender));
            if (appenders != null)
            {
                foreach (var appender in appenders)
                {
                    var logsAppender = appender as LogsAppender;
                    if (logsAppender != null)
                    {
                        logsAppender.LogAdded += LogAdded;
                    }
                }
            }
        }

        private void LogAdded(string message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (Logs.Count > _maxLogCount)
                {
                    Logs.RemoveAt(0);
                }
                Logs.Add(message);
            });
        }

        public ObservableCollection<string> Logs
        {
            get; set;
        } = new ObservableCollection<string>();

        public override string Title { get; } = "Logs";
    }
}