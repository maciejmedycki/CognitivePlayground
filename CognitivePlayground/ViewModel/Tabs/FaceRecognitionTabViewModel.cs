using CognitivePlayground.Model;
using GalaSoft.MvvmLight.Command;
using log4net;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using azure = Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CognitivePlayground.ViewModel.Tabs
{
    public class CognitivePlaygroundTabViewModel : TabItemViewModelBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CognitivePlaygroundTabViewModel));
        private readonly IFaceClient faceClient;
        private ICommand _detectFaceCommand;
        private FaceRectangle _faceRectangle = new FaceRectangle();
        private string _imagePath = @"C:\Users\Dell\Desktop\faceTest3.jpg";

        public CognitivePlaygroundTabViewModel()
        {
            var subscriptionKey = ConfigurationManager.GetAzureSubscriptionKey();
            var apiUri = ConfigurationManager.GetAzureApiUri();
            faceClient = new FaceClient(new ApiKeyServiceClientCredentials(subscriptionKey), new System.Net.Http.DelegatingHandler[] { });
            faceClient.Endpoint = apiUri;
        }

        public ICommand DetectFaceCommand
        {
            get
            {
                if (_detectFaceCommand == null)
                {
                    _detectFaceCommand = new RelayCommand(DoDetectFaceCommand);
                }
                return _detectFaceCommand;
            }
        }

        public FaceRectangle FaceRectangle
        {
            get
            {
                return _faceRectangle;
            }
        }

        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                _imagePath = value;
                RaisePropertyChanged(() => ImagePath);
                RaisePropertyChanged(() => ImageSource);
                HideFaceRectangle();
            }
        }

        public BitmapImage ImageSource
        {
            get
            {            
                return new BitmapImage( new Uri(ImagePath));
            }
        }

        public override string Title { get; } = "Face recognition";

        public async void DoDetectFaceCommand()
        {
            try
            {
                using (var imageFileStream = File.OpenRead(ImagePath))
                {
                    var faceList = await faceClient.Face.DetectWithStreamAsync(imageFileStream, false);
                    var foundFace = faceList.FirstOrDefault();
                    if (foundFace != null)
                    {
                        UpdateFaceRectangle(foundFace.FaceRectangle);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                //TODO: show something on UI?
            }
        }

        private void UpdateFaceRectangle(azure.FaceRectangle faceRectangle)
        {
            _faceRectangle.Width = faceRectangle.Width;
            _faceRectangle.Height= faceRectangle.Height;
            var margin = _faceRectangle.Margin;
            margin.Top = faceRectangle.Top;
            margin.Left = faceRectangle.Left;
            RaisePropertyChanged(() => FaceRectangle);
        }

        private void HideFaceRectangle()
        {
            RaisePropertyChanged(() => FaceRectangle);
            _faceRectangle.Width = 0;
            _faceRectangle.Height = 0;
            var margin = _faceRectangle.Margin;
            margin.Top = 0;
            margin.Left = 0;
        }
    }
}