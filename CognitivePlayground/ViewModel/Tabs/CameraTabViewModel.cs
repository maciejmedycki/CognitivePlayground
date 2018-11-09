using Emgu.CV;
using Emgu.CV.Structure;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Hodor.Model.Messages;
using log4net;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace Hodor.ViewModel.Tabs
{
    public class CameraTabViewModel : TabItemViewModelBase
    {
        public string _cameraAddress;
        private const string _saveFolder = "FrontFaces";
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CameraTabViewModel));
        private static CascadeClassifier _frontFaceClassifier = new CascadeClassifier("Resources\\haarcascades\\haarcascade_frontalface_alt_tree.xml");
        private static Size _minSize = new Size(80, 80);
        private bool _autoStart;

        private VideoCapture _capture;

        private BitmapSource _image;

        private bool _saveNextFrame = false;

        private ICommand _startCommand;

        private ICommand _takePictureCommand;

        public CameraTabViewModel()
        {
            Messenger.Default?.Register<StartupActionsCalled>(this, StartupActionsCalledHandler);
            if (!Directory.Exists(_saveFolder))
            {
                Directory.CreateDirectory(_saveFolder);
            }
        }

        public bool AutoStart
        {
            get
            {
                return _autoStart;
            }
            set
            {
                _autoStart = value;
                RaisePropertyChanged(() => AutoStart);
            }
        }

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

        public BitmapSource Image
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

        public ICommand TakePictureCommand
        {
            get
            {
                if (_takePictureCommand == null)
                {
                    _takePictureCommand = new RelayCommand(DoTakePictureCommand);
                }
                return _takePictureCommand;
            }
        }

        public override string Title { get; } = "Camera";

        public void DoTakePictureCommand()
        {
            _saveNextFrame = true;
        }

        private async void DoStart()
        {
            _logger.Info($"DoStart() called for CameraAddress: \"{CameraAddress}\"");
            if (_capture != null)
            {
                _capture.Dispose();
                _capture = null;
            }
            _capture = new VideoCapture(CameraAddress);
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Buffersuze, 3);
            var fps = _capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);
            var intervalMs = 200;
            if (fps > 0)
            {
                intervalMs = (int)(1000 / fps);
            }

            Messenger.Default?.Send(new CapturingStartedMessage());

            while (true)
            {
                Mat m = new Mat();
                _capture.Read(m);
                if (!m.IsEmpty)
                {
                    var imageMemoryStream = new MemoryStream();
                    m.Bitmap.Save(imageMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imageMemoryStream.Position = 0;
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = imageMemoryStream;
                    bi.EndInit();
                    Image = bi;
                    var now = DateTime.Now;
                    var fileName = now.ToString("yyyy.MM.dd HH.mm.ss.fff") + ".jpg";
                    var filePath = Path.Combine(_saveFolder, fileName);
                    FoundFace(m, _frontFaceClassifier, filePath);
                }
                m.Dispose();
                await Task.Delay(intervalMs);
            }
        }

        private void FoundFace(Mat frame, CascadeClassifier classifier, string filePathAndName)
        {
            var foundObjects = classifier.DetectMultiScale(frame, 1.1, 10, _minSize);
            if (foundObjects != null && foundObjects.Length > 0 || _saveNextFrame)
            {
                _logger.Info($"Capture saved as {filePathAndName}");
                frame.Save(filePathAndName);
                foreach (var objectRoi in foundObjects)
                {
                    var image = frame.ToImage<Bgr, Byte>();
                    image.Draw(objectRoi, new Bgr(Color.Red), 2);
                    image.Save(filePathAndName.Replace(".jpg", "Rectangle.jpg"));
                    image.Dispose();
                }
                Messenger.Default?.Send(new FaceDetectedMessage(filePathAndName));
                _saveNextFrame = false;
            }
            frame.Dispose();
        }

        private void StartupActionsCalledHandler(StartupActionsCalled obj)
        {
            if (AutoStart)
            {
                DoStart();
            }
        }
    }
}