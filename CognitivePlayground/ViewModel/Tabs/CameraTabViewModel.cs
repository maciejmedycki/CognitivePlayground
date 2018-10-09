using Emgu.CV;
using GalaSoft.MvvmLight.Command;
using log4net;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CognitivePlayground.ViewModel.Tabs
{
    public class CameraTabViewModel : TabItemViewModelBase
    {
        public string _cameraAddress;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CameraTabViewModel));
        private VideoCapture _capture;
        private ImageSource _image;
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

        public ImageSource Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                RaisePropertyChanged(() => Image);
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

        private async void DoStart()
        {
            _logger.Info($"DoStart() called for CameraAddress: \"{CameraAddress}\"");
            if (_capture != null)
            {
                _capture.Dispose();
                _capture = null;
            }
            _capture = new VideoCapture(CameraAddress);
            var fps = _capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);
            var interval = TimeSpan.FromSeconds(1 / fps);
            while (true)
            {
                Mat m = new Mat();
                _capture.Read(m);
                //var m =_capture.QueryFrame();//old <v.3
                if (!m.IsEmpty)
                {
                    Image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(m.Bitmap.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty,
                        BitmapSizeOptions.FromWidthAndHeight(m.Bitmap.Width, m.Bitmap.Height));
                }
                await Task.Delay(interval);//1000/fps capture.GetCapturePropoerty(CapProp.Fps)
            }
        }
    }
}