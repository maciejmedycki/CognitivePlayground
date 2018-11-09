using Hodor.Model;
using Hodor.Model.ExtensionMethods.Azure;
using Hodor.Model.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using log4net;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using azure = Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace Hodor.ViewModel.Tabs
{
    public class HodorTabViewModel : TabItemViewModelBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(HodorTabViewModel));
        private readonly IFaceClient _faceClient;
        private readonly PersonGroupViewModel _personGroupViewModel;
        private ICommand _detectFaceCommand;
        private FaceRectangle _faceRectangle = new FaceRectangle();
        private string _imagePath = string.Empty;

        public HodorTabViewModel()
        {
            var subscriptionKey = ConfigurationManager.GetAzureSubscriptionKey();
            var apiUri = ConfigurationManager.GetAzureApiUri();
            _faceClient = new FaceClient(new ApiKeyServiceClientCredentials(subscriptionKey), new System.Net.Http.DelegatingHandler[] { });
            _faceClient.Endpoint = apiUri;
            _personGroupViewModel = new PersonGroupViewModel(_faceClient, () => { return ImagePath; });

            Messenger.Default?.Register<FaceDetectedMessage>(this, FaceDetectedMessageHandler);
            Messenger.Default?.Register<CapturingStartedMessage>(this, CapturingStartedMessageHandler);
        }

        private async void CapturingStartedMessageHandler(CapturingStartedMessage obj)
        {
            if (PersonGroupViewModel?.SelectedPersonGroup == null || PersonGroupViewModel?.SelectedPersonGroup.PersonGroupId == null)
            {
                if (PersonGroupViewModel.PersonGroups == null || PersonGroupViewModel.PersonGroups.Count()==0)
                {
                    await PersonGroupViewModel.ListPersonGroups();
                    if (PersonGroupViewModel.PersonGroups != null)
                    {
                        //TODO: use last used person group from config/settings
                        PersonGroupViewModel.SelectedPersonGroup = PersonGroupViewModel.PersonGroups.FirstOrDefault();
                    }
                }
            }
        }

        private void FaceDetectedMessageHandler(FaceDetectedMessage faceDetectedMessage)
        {
            ImagePath = faceDetectedMessage.Path;
            PersonGroupViewModel?.RecognizeFaceCommand.Execute(null);
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
                    return new BitmapImage(new Uri(Path.GetFullPath(ImagePath), UriKind.Absolute));
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