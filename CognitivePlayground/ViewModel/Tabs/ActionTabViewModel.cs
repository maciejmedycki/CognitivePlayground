using GalaSoft.MvvmLight.Command;
using log4net;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CognitivePlayground.ViewModel.Tabs
{
    public class ActionTabViewModel : TabItemViewModelBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ActionTabViewModel));
        private ObservableCollection<ActionViewModelBase> _actions;
        private ICommand _addActionCommand;
        private ICommand _executeActionsCommand;
        private ICommand _removeActionCommand;
        private ICommand _saveChangesCommand;

        public ActionTabViewModel()
        {
            var actions = ConfigurationManager.GetActions();
            var actionList = new ObservableCollection<ActionViewModelBase>();
            foreach (var action in actions)
            {
                actionList.Add(new ActionViewModel(action));
            }
            Actions = actionList;
        }

        public ObservableCollection<ActionViewModelBase> Actions
        {
            get
            {
                return _actions;
            }
            set
            {
                if (_actions != value)
                {
                    _actions = value;
                    RaisePropertyChanged(() => Actions);
                }
            }
        }

        public ICommand AddActionCommand
        {
            get
            {
                if (_addActionCommand == null)
                {
                    _addActionCommand = new RelayCommand(DoAddAction);
                }
                return _addActionCommand;
            }
        }

        public ICommand ExecuteActionsCommand
        {
            get
            {
                if (_executeActionsCommand == null)
                {
                    _executeActionsCommand = new RelayCommand(DoExecuteActions);
                }
                return _executeActionsCommand;
            }
        }

        public ICommand RemoveActionCommand
        {
            get
            {
                if (_removeActionCommand == null)
                {
                    _removeActionCommand = new RelayCommand<ActionViewModel>(DoRemoveAction);
                }
                return _removeActionCommand;
            }
        }

        public ICommand SaveChangesCommand
        {
            get
            {
                if (_saveChangesCommand == null)
                {
                    _saveChangesCommand = new RelayCommand(DoSaveChange);
                }
                return _saveChangesCommand;
            }
        }

        public override string Title { get; } = "Actions";

        public void DoAddAction()
        {
            _logger.Info("DoAddAction() called");
            Actions.Add(new ActionViewModel(new Model.Action()));
        }

        public void DoExecuteActions()
        {
            _logger.Info("DoExecuteActions() called");
            Parallel.ForEach(Actions, CallAction);
        }

        public void DoRemoveAction(ActionViewModel actionViewModel)
        {
            _logger.Info("DoRemoveAction() called");
            if (actionViewModel != null)
            {
                Actions.Remove(actionViewModel);
                _logger.Info($"DoRemoveAction() removed {actionViewModel?.ToString()}");
            }
        }

        private void CallAction(ActionViewModelBase actionViewModel)
        {
            _logger.Info($"CallAction() called for {actionViewModel?.ToString()}");
            actionViewModel?.DoExecute();
        }

        private void DoSaveChange()
        {
            _logger.Info("DoSaveChange() called");
            ConfigurationManager.SaveActions(Actions.Select(a => a.Action));
        }
    }
}