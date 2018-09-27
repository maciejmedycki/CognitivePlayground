using GalaSoft.MvvmLight.Command;
using log4net;
using System.Windows.Input;

namespace CognitivePlayground.ViewModel.Tabs
{
    public class CameraTabViewModel : TabItemViewModelBase
    {
        public string _cameraAddress;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CameraTabViewModel));
        private ICommand _startCommand;

        public string CameraAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_cameraAddress))
                {
                    _cameraAddress = ConfigurationManager.GetVideoStreamAddress();
                }
                return _cameraAddress;
            }
            set
            {
                if (_cameraAddress != value)
                {
                    _cameraAddress = value;
                    RaisePropertyChanged(() => CameraAddress);
                    ConfigurationManager.SaveVideoStreamAddress(_cameraAddress);
                }
            }
        }

        public ICommand StartCommand
        {
            get
            {
                if (_startCommand == null)
                {
                    _startCommand = new RelayCommand(DoStart);
                }
                return _startCommand;
            }
        }

        public override string Title { get; } = "Camera";

        private void DoStart()
        {
            _logger.Info($"DoStart() called for CameraAddress: \"{CameraAddress}\"");
            //TODO: start capturing video from CameraAddress.
        }
    }
}