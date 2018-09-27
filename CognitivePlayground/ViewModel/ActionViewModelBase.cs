using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
using System.Windows.Input;

namespace CognitivePlayground.ViewModel
{
    public abstract class ActionViewModelBase : ViewModelBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ActionViewModelBase));
        public ICommand _executeCommand;
        protected readonly Model.Action _action;
        private string _path;
        private bool _shouldExecute;

        public ActionViewModelBase(Model.Action action)
        {
            _action = action;
            Path = action.Path;
            ShouldExecute = action.ShouldExecute;
        }

        public Model.Action Action
        {
            get
            {
                return _action;
            }
        }

        public ICommand ExecuteCommand
        {
            get
            {
                if (_executeCommand == null)
                {
                    _executeCommand = new RelayCommand(DoExecute);
                }
                return _executeCommand;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    RaisePropertyChanged(() => Path);
                }
            }
        }

        public bool ShouldExecute
        {
            get
            {
                return _shouldExecute;
            }
            set
            {
                if (_shouldExecute != value)
                {
                    _shouldExecute = value;
                    RaisePropertyChanged(() => ShouldExecute);
                }
            }
        }

        public abstract void DoExecute();
    }
}