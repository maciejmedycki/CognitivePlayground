using log4net;
using System;
using System.Diagnostics;
using System.Threading;

namespace Hodor.Model
{
    public class Action
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Action));
        private readonly Timer _keepAliveTimer;
        private Process _process;
        private static object _processLocker = new object();

        public Action()
        {
            _keepAliveTimer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        public bool KeepAlive { get; set; }
        public string Path { get; set; }
        public bool ShouldExecute { get; set; }

        public void Execute()
        {
            try
            {
                lock (_processLocker)
                {
                    if (_process != null && !_process.HasExited)
                    {
                        _logger.Warn($"Process {Path} is already running.");
                        return;
                    }
                    if (ShouldExecute)
                    {                        
                        _logger.Info($"Executing Action for path: \"{Path}\"");
                        var workingDirectory = System.IO.Path.GetDirectoryName(Path);
                        //var fileName = System.IO.Path.GetFileName(Path);
                        _process = new Process();
                        _process.StartInfo.WorkingDirectory = workingDirectory;
                        _process.StartInfo.FileName = Path;
                        _process.StartInfo.CreateNoWindow = false;
                        _process.StartInfo.UseShellExecute = false;
                        _process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        _process.StartInfo.RedirectStandardOutput = true;
                        _process.StartInfo.RedirectStandardError = true;
                        _process.Exited += ProcessExited;
                        _process.OutputDataReceived += ProcessOutputHandler;
                        _process.ErrorDataReceived += ProcessErrorHandler;
                        _process.EnableRaisingEvents = true;
                        _process.Start();
                        _process.BeginOutputReadLine();
                        _process.BeginErrorReadLine();
                        _keepAliveTimer.Change(0, 2000);
                    }
                    else
                    {
                        _logger.Info($"Not executing Action for path: \"{Path}\" ShouldExecute is false.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Action for path: \"{Path}\" throw an exception {ex.Message} {ex.StackTrace}");
            }
        }

        private void ProcessErrorHandler(object sender, DataReceivedEventArgs e)
        {
            if (e != null)
            {
                _logger.Debug($"Process {Path} error: {e.Data}");
            }
        }

        private void ProcessOutputHandler(object sender, DataReceivedEventArgs e)
        {
            if (e != null)
            {
                _logger.Debug($"Process {Path} output: {e.Data}");
            }
        }

        public void Kill()
        {
            try
            {
                lock (_processLocker)
                {
                    _keepAliveTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    _process.OutputDataReceived -= ProcessOutputHandler;
                    _process.ErrorDataReceived -= ProcessOutputHandler;
                    _process?.CloseMainWindow();
                    _process?.Kill();
                }
            }
            catch (Exception ex)
            {
                _logger.Warn($"Unable to kill process {Path}: {ex.Message} {ex.StackTrace}");
            }
        }

        public override string ToString()
        {
            return $"{base.ToString()} {this.Path}";
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            lock (_processLocker)
            {
                _logger.Info($"Action for path: \"{Path}\" exited.");
                _keepAliveTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _process.OutputDataReceived -= ProcessOutputHandler;
                _process.ErrorDataReceived -= ProcessOutputHandler;
                _process = null;
                if (KeepAlive)
                {
                    _logger.Info($"Action for path: \"{Path}\" should by kept alive. Restarting!");
                    Execute();
                }
            }
        }

        private void TimerCallback(object state)
        {
            lock (_processLocker)
            {
                if (KeepAlive && _process != null && _process.HasExited)
                {
                    Execute();
                }
            }
        }
    }
}