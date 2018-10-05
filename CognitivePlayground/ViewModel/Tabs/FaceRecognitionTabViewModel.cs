using CognitivePlayground.Model;
using GalaSoft.MvvmLight.Command;
using log4net;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using azure = Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CognitivePlayground.ViewModel.Tabs
{
    public class CognitivePlaygroundTabViewModel : TabItemViewModelBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CognitivePlaygroundTabViewModel));
        //TODO: get it from ConfigurationManager
        private static string _personGroupId = "systellGroup";
        private readonly IFaceClient faceClient;
        private ICommand _addPersonGroupCommand;
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

        public ICommand AddPersonGroupCommand
        {
            get
            {
                if (_addPersonGroupCommand == null)
                {
                    _addPersonGroupCommand = new RelayCommand(DoAddPersonGroup);
                }
                return _addPersonGroupCommand;
            }
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
                return new BitmapImage(new Uri(ImagePath));
            }
        }

        public override string Title { get; } = "Face recognition";

        public async void DoAddPersonGroup()
        {
            try
            {
                await faceClient.PersonGroup.CreateAsync(_personGroupId, "Systell");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

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

        //TODO: add command
        private async void DoAddPerson()
        {
            //TODO: get from somewhere
            var name = "Maciej Medycki";
            var person = await faceClient.PersonGroupPerson.CreateAsync(_personGroupId, name);
            //TODO: get from somewhere
            var files = new string[] { "", "" };
            foreach (var file in files)
            {
                using (var imageFileStream = File.OpenRead(file))
                {
                    var persistedFace = await faceClient.PersonGroupPerson.AddFaceFromStreamAsync(_personGroupId, person.PersonId, imageFileStream);
                }
            }
        }

        //TODO: add command
        private async void DoIdentifyFace()
        {
            //TODO: get from somewhere
            var faceFile = "";
            using (var imageFileStream = File.OpenRead(faceFile))
            {
                var faces = await faceClient.Face.DetectWithStreamAsync(imageFileStream);
                var faceIds = faces.Select(f => f.FaceId.Value).ToList();
                var identifies = await faceClient.Face.IdentifyAsync(faceIds, _personGroupId, null, 1, 0.7);
                foreach (var identify in identifies)
                {
                    if (identify.Candidates.Count > 0)
                    {
                        var candidate = identify.Candidates[0];
                        var personId = candidate.PersonId;
                        var person = await faceClient.PersonGroupPerson.GetAsync(_personGroupId, personId);
                        _logger.Info($"Person found: {person.Name} with confidence: {candidate.Confidence * 100}%");
                    }
                    else
                    {
                        _logger.Info("No candidates");
                    }
                }
            }
        }

        //TODO: add command
        private async void DoTrainGroup()
        {
            await faceClient.PersonGroup.TrainAsync(_personGroupId);
            while (true)
            {
                var status = await faceClient.PersonGroup.GetTrainingStatusAsync(_personGroupId);
                if (status.Status != azure.TrainingStatusType.Running)
                {
                    _logger.Info($"PersonGroup {_personGroupId} finshed training with status {status.Status}");
                    break;
                }
                await Task.Delay(500);
            }
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

        private void UpdateFaceRectangle(azure.FaceRectangle faceRectangle)
        {
            _faceRectangle.Width = faceRectangle.Width;
            _faceRectangle.Height = faceRectangle.Height;
            var margin = _faceRectangle.Margin;
            margin.Top = faceRectangle.Top;
            margin.Left = faceRectangle.Left;
            RaisePropertyChanged(() => FaceRectangle);
        }
    }
}