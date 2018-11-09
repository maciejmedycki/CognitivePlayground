using log4net.Appender;
using log4net.Core;
using System;

namespace Hodor.Model
{
    public class LogsAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            LogAdded?.Invoke(loggingEvent.RenderedMessage);
        }

        public Action<string> LogAdded { get; set; }
    }
}