using log4net;
using System;
using System.Diagnostics;
using System.IO;

namespace CognitivePlayground.Model
{
    public class Action
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Action));
        public string Path { get; set; }
        public bool ShouldExecute { get; set; }

        public void Execute()
        {
            try
            {
                _logger.Info($"Executing Action for path: \"{Path}\"");
                var p = new Process();
                p.EnableRaisingEvents = true;
                string appLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var info = new FileInfo(appLocation);
                p.StartInfo.FileName = Path;
                p.StartInfo.CreateNoWindow = true;
                p.Exited += p_Exited;
                p.Start();
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                _logger.Error($"Action for path: \"{Path}\" throw an exception {ex.Message} {ex.StackTrace}");
            }
        }

        private void p_Exited(object sender, EventArgs e)
        {
            _logger.Info($"Action for path: \"{Path}\" executed.");
        }

        public override string ToString()
        {
            return $"base.ToString() {this.Path}";
        }
    }
}