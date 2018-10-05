using CognitivePlayground.Model;
using CognitivePlayground.Model.ExtensionMethods.Azure;
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
        private readonly IFaceClient _faceClient;
        private readonly PersonGroupViewModel _personGroupViewModel;
        private ICommand _detectFaceCommand;
        private FaceRectangle _faceRectangle = new FaceRectangle();
        private string _imagePath = string.Empty;

        public CognitivePlaygroundTabViewModel()
        {
            var subscriptionKey = ConfigurationManager.GetAzureSubscriptionKey();
            var apiUri = ConfigurationManager.GetAzureApiUri();
            _faceClient = new FaceClient(new ApiKeyServiceClientCredentials(subscriptionKey), new System.Net.Http.DelegatingHandler[] { });
            _faceClient.Endpoint = apiUri;
            _personGroupViewModel = new PersonGroupViewModel(_faceClient, () => { return ImagePath; });
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
                DoUpdateImagePath();
            }
        }

        public BitmapImage ImageSource
        {
            get
            {
                if (File.Exists(ImagePath))
                {
                    return new BitmapImage(new Uri(ImagePath));
                }
                else
                {
                    _logger.Warn($"Image '{ImagePath}' does not exist");
                }
                return null;
            }
        }

        public PersonGroupViewModel PersonGroupViewModel
        {
            get
            {
                return _personGroupViewModel;
            }
        }

        public override string Title { get; } = "Face recognition";

        public async void DoDetectFaceCommand()
        {
            try
            {
                using (var imageFileStream = File.OpenRead(ImagePath))
                {
                    var faceList = await _faceClient.Face.DetectWithStreamAsync(imageFileStream, false);
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

        private void DoUpdateImagePath()
        {
            HideFaceRectangle();
            RaisePropertyChanged(() => ImagePath);
            RaisePropertyChanged(() => ImageSource);
            HideFaceRectangle();
        }

        private void HideFaceRectangle()
        {
            _faceRectangle.Width = 0;
            _faceRectangle.Height = 0;
            _faceRectangle.Top = 0;
            _faceRectangle.Left = 0;
            RaisePropertyChanged(() => FaceRectangle);
        }

        private void UpdateFaceRectangle(azure.FaceRectangle faceRectangle)
        {
            faceRectangle.UpdateFaceRectangle(ref _faceRectangle);
            RaisePropertyChanged(() => FaceRectangle);
        }
    }
}