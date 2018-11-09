using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
using System;
using System.Windows.Input;

namespace Hodor.ViewModel
{
    public abstract class ActionViewModelBase : ViewModelBase
    {
        public ICommand _executeCommand;
        protected readonly Model.Action _action;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ActionViewModelBase));

        public ActionViewModelBase(Model.Action action)
        {
            if (action is null)
            {
                throw new ArgumentException("Argument cannot be null", "action");
            }
            _action = action;
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

        public bool KeepAlive
        {
            get
            {
                return _action.KeepAlive;
            }
            set
            {
                if (_action.KeepAlive != value)
                {
                    _action.KeepAlive = value;
                    RaisePropertyChanged(() => KeepAlive);
                }
            }
        }

        public string Path
        {
            get
            {
                return _action.Path;
            }
            set
            {
                if (_action.Path != value)
                {
                    _action.Path = value;
                    RaisePropertyChanged(() => Path);
                }
            }
        }

        public bool ShouldExecute
        {
            get
            {
                return _action.ShouldExecute;
            }
            set
            {
                if (_action.ShouldExecute != value)
                {
                    _action.ShouldExecute = value;
                    RaisePropertyChanged(() => ShouldExecute);
                }
            }
        }

        public abstract void DoExecute();

        public void Kill()
        {
            Action.KeepAlive = false;
            Action.Kill();
        }
    }
}