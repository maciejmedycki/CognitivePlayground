using Hodor.Model;
using Hodor.Model.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hodor.ViewModel.Tabs
{
    public class ActionTabViewModel : TabItemViewModelBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ActionTabViewModel));
        private ICommand _addFaceRecognizedActionCommand;
        private ICommand _addStartupActionCommand;
        private ICommand _executeActionsCommand;
        private ObservableCollection<ActionViewModelBase> _faceRecognizedActions;
        private ICommand _removeFaceRecognizedActionCommand;
        private ICommand _removeStartupActionCommand;
        private ICommand _saveChangesCommand;
        private ObservableCollection<ActionViewModelBase> _startupActions;

        public ActionTabViewModel()
        {
            var actionWrapper = ConfigurationManager.GetActions();
            if (actionWrapper != null)
            {
                FaceRecognizedActions = new ObservableCollection<ActionViewModelBase>(actionWrapper.FaceRecognizedActions.Select(a => new ActionViewModel(a)));
                StartupActions = new ObservableCollection<ActionViewModelBase>(actionWrapper.StartupActions.Select(a => new ActionViewModel(a)));
            }
            else
            {
                FaceRecognizedActions = new ObservableCollection<ActionViewModelBase>();
                StartupActions = new ObservableCollection<ActionViewModelBase>();
            }
            Messenger.Default?.Register<FaceRecognizedMessage>(this, FaceRecognizedMessageHandler);
            Messenger.Default?.Register<StartupMessage>(this, StartupMessageHandler);
            Messenger.Default?.Register<AppExitingMessage>(this, AppExitingHandler);
        }

        private void AppExitingHandler(AppExitingMessage obj)
        {
            foreach (var action in FaceRecognizedActions)
            {
                action.Kill();
            }
            foreach (var action in StartupActions)
            {
                action.Kill();
            }
        }

        public ICommand AddFaceRecognizedActionCommand
        {
            get
            {
                if (_addFaceRecognizedActionCommand == null)
                {
                    _addFaceRecognizedActionCommand = new RelayCommand(DoAddFaceRecognizedAction);
                }
                return _addFaceRecognizedActionCommand;
            }
        }

        public ICommand AddStartupActionCommand
        {
            get
            {
                if (_addStartupActionCommand == null)
                {
                    _addStartupActionCommand = new RelayCommand(DoAddStartupAction);
                }
                return _addStartupActionCommand;
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

        public ObservableCollection<ActionViewModelBase> FaceRecognizedActions
        {
            get
            {
                return _faceRecognizedActions;
            }
            set
            {
                if (_faceRecognizedActions != value)
                {
                    _faceRecognizedActions = value;
                    RaisePropertyChanged(() => FaceRecognizedActions);
                }
            }
        }

        public ICommand RemoveFaceRecognizedActionCommand
        {
            get
            {
                if (_removeFaceRecognizedActionCommand == null)
                {
                    _removeFaceRecognizedActionCommand = new RelayCommand<ActionViewModel>(DoRemoveFaceRecognizedAction);
                }
                return _removeFaceRecognizedActionCommand;
            }
        }

        public ICommand RemoveStartupActionCommand
        {
            get
            {
                if (_removeStartupActionCommand == null)
                {
                    _removeStartupActionCommand = new RelayCommand<ActionViewModel>(DoRemoveStartupAction);
                }
                return _removeStartupActionCommand;
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

        public ObservableCollection<ActionViewModelBase> StartupActions
        {
            get
            {
                return _startupActions;
            }
            set
            {
                _startupActions = value;
                RaisePropertyChanged(() => StartupActions);
            }
        }

        public override string Title { get; } = "Actions";

        public void DoAddFaceRecognizedAction()
        {
            _logger.Info("DoAddFaceRecognizedActionCommand() called");
            FaceRecognizedActions.Add(new ActionViewModel(new Model.Action()));
        }

        public void DoAddStartupAction()
        {
            _logger.Info("DoAddStartupActionCommand() called");
            StartupActions.Add(new ActionViewModel(new Model.Action()));
        }

        public void DoExecuteActions()
        {
            _logger.Info("DoExecuteActions() called");
            Parallel.ForEach(FaceRecognizedActions, CallAction);
        }

        public void DoRemoveFaceRecognizedAction(ActionViewModel actionViewModel)
        {
            _logger.Info("DoRemoveFaceRecognizedAction() called");
            if (actionViewModel != null)
            {
                FaceRecognizedActions.Remove(actionViewModel);
                _logger.Info($"DoRemoveFaceRecognizedAction() removed {actionViewModel?.ToString()}");
            }
        }

        public void DoRemoveStartupAction(ActionViewModel actionViewModel)
        {
            _logger.Info("DoRemoveStartupAction() called");
            if (actionViewModel != null)
            {
                StartupActions.Remove(actionViewModel);
                _logger.Info($"DoRemoveStartupAction() removed {actionViewModel?.ToString()}");
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
            var actionsWrapper = new ActionsWrapper
            {
                FaceRecognizedActions = FaceRecognizedActions.Select(a => a.Action),
                StartupActions = StartupActions.Select(a => a.Action),
            };
            ConfigurationManager.SaveActions(actionsWrapper);
        }

        private void ExecuteActions(IEnumerable<ActionViewModelBase> actionsToExecute)
        {
            if (actionsToExecute != null)
            {
                foreach (var action in actionsToExecute)
                {
                    action.DoExecute();
                }
            }
        }

        private void FaceRecognizedMessageHandler(FaceRecognizedMessage faceRecognizedMessage)
        {
            _logger.Info($"FaceRecognizedMessageHandler() called");
            ExecuteActions(FaceRecognizedActions);
        }

        private void StartupMessageHandler(StartupMessage startupMessage)
        {
            _logger.Info($"StartupMessageHandler() called");
            ExecuteActions(StartupActions);
            Messenger.Default?.Send<StartupActionsCalled>(new StartupActionsCalled());
        }
    }
}