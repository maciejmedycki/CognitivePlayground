using log4net;

namespace CognitivePlayground.ViewModel
{
    public class ActionViewModel : ActionViewModelBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ActionViewModel));

        public ActionViewModel(Model.Action action)
            : base(action)
        {
        }

        public override void DoExecute()
        {
            _action?.Execute();
        }

        public override string ToString()
        {
            return $"{base.ToString()} {_action?.ToString()}";
        }
    }
}