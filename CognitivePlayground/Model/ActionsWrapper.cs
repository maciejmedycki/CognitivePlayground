using System.Collections.Generic;

namespace Hodor.Model
{
    public class ActionsWrapper
    {
        public IEnumerable<Action> FaceRecognizedActions { get; set; }
        public IEnumerable<Action> StartupActions { get; set; }
    }
}