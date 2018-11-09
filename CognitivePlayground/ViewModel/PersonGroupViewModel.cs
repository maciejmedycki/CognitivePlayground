using Hodor.Model;
using Hodor.Model.ExtensionMethods.Azure;
using Hodor.Model.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using log4net;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using azure = Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace Hodor.ViewModel
{
    public class PersonGroupViewModel : ViewModelBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(PersonGroupViewModel));
        private readonly IFaceClient _faceClient;
        private readonly Func<string> _getImagePath;
        private ICommand _addFaceToPersonCommand;
        private ICommand _addNewPersonCommand;
        private ICommand _createPersonGroupCommand;
        private ICommand _doTrainCommand;
        private ICommand _listPersonGroupsCommand;
        private ICommand _listPersonsCommand;
        private string _newPersonGroupName;
        private string _newPersonName;
        private IEnumerable<PersonGroup> _personGroups;
        private IEnumerable<Person> _persons;
        private ICommand _recognizeFaceCommand;
        private Person _selectedPerson;
        private PersonGroup _selectedPersonGroup;

        public PersonGroupViewModel(IFaceClient faceClient, Func<string> getImagePath)
        {
            _faceClient = faceClient;
            _getImagePath = getImagePath;
        }

        public ICommand AddFaceToPersonCommand
        {
            get
            {
                if (_addFaceToPersonCommand == null)
                {
                    _addFaceToPersonCommand = new RelayCommand(DoAddFaceToPerson);
                }
                return _addFaceToPersonCommand;
            }
        }

        public ICommand AddNewPersonCommand
        {
            get
            {
                if (_addNewPersonCommand == null)
                {
                    _addNewPersonCommand = new RelayCommand(DoAddPerson);
                }
                return _addNewPersonCommand;
            }
        }

        public ICommand CreatePersonGroupCommand
        {
            get
            {
                if (_createPersonGroupCommand == null)
                {
                    _createPersonGroupCommand = new RelayCommand(DoCreatePersonGroup);
                }
                return _createPersonGroupCommand;
            }
        }

        public ICommand DoTrainCommand
        {
            get
            {
                if (_doTrainCommand == null)
                {
                    _doTrainCommand = new RelayCommand(DoTrain);
                }
                return _doTrainCommand;
            }
        }

        public ICommand ListPersonGroupsCommand
        {
            get
            {
                if (_listPersonGroupsCommand == null)
                {
                    _listPersonGroupsCommand = new RelayCommand(DoListPersonGroups);
                }
                return _listPersonGroupsCommand;
            }
        }

        public ICommand ListPersonsCommand
        {
            get
            {
                if (_listPersonsCommand == null)
                {
                    _listPersonsCommand = new RelayCommand(DoListPersons);
                }
                return _listPersonsCommand;
            }
        }

        public string NewPersonGroupName
        {
            get
            {
                return _newPersonGroupName;
                ;
            }
            set
            {
                _newPersonGroupName = value;
                RaisePropertyChanged(() => NewPersonGroupName);
            }
        }

        public string NewPersonName
        {
            get
            {
                return _newPersonName;
            }
            set
            {
                _newPersonName = value;
                RaisePropertyChanged(() => NewPersonName);
            }
        }

        public IEnumerable<PersonGroup> PersonGroups
        {
            get
            {
                return _personGroups;
            }
            set
            {
                _personGroups = value;
                RaisePropertyChanged(() => PersonGroups);
            }
        }

        public IEnumerable<Person> Persons
        {
            get
            {
                return _persons;
            }
            set
            {
                _persons = value;
                RaisePropertyChanged(() => Persons);
            }
        }

        public ICommand RecognizeFaceCommand
        {
            get
            {
                if (_recognizeFaceCommand == null)
                {
                    _recognizeFaceCommand = new RelayCommand(DoRecognizeFace);
                }
                return _recognizeFaceCommand;
            }
        }

        public Person SelectedPerson
        {
            get
            {
                return _selectedPerson;
            }
            set
            {
                _selectedPerson = value;
                RaisePropertyChanged(() => SelectedPerson);
            }
        }

        public PersonGroup SelectedPersonGroup
        {
            get
            {
                return _selectedPersonGroup;
            }
            set
            {
                _selectedPersonGroup = value;
                RaisePropertyChanged(() => SelectedPersonGroup);
            }
        }

        private async void DoAddFaceToPerson()
        {
            if (SelectedPerson.PersonId == null || SelectedPerson.PersonId == Guid.Empty)
            {
                _logger.Warn("Unable to add face to person, SelectedPerson is empty");
                return;
            }
            var imagePath = _getImagePath?.Invoke();
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                _logger.Warn("Unable to add face to person, imagePath is empty");
                return;
            }
            try
            {
                var files = new string[] { imagePath };
                foreach (var file in files)
                {
                    using (var imageFileStream = File.OpenRead(file))
                    {
                        var persistedFace = await _faceClient.PersonGroupPerson.AddFaceFromStreamAsync(SelectedPersonGroup.PersonGroupId, SelectedPerson.PersonId, imageFileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private async void DoAddPerson()
        {
            if (string.IsNullOrWhiteSpace(NewPersonName))
            {
                _logger.Warn("Unable to add person, NewPersonName is empty");
                return;
            }
            if (string.IsNullOrWhiteSpace(SelectedPersonGroup.PersonGroupId))
            {
                _logger.Warn("Unable to add person, no person group is selected");
                return;
            }
            try
            {
                var person = await _faceClient.PersonGroupPerson.CreateAsync(SelectedPersonGroup.PersonGroupId, NewPersonName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private async void DoCreatePersonGroup()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(NewPersonGroupName))
                {
                    await _faceClient.PersonGroup.CreateAsync(NewPersonGroupName, NewPersonGroupName);
                    _logger.Info($"New PersonGroupCreated {NewPersonGroupName} created.");
                }
                else
                {
                    _logger.Warn("NewPersonGroupName is null or empty.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public async Task ListPersonGroups()
        {
            try
            {
                var personGroups = await _faceClient.PersonGroup.ListAsync();
                PersonGroups = personGroups.Select(pg => pg.CreatePersonGroup());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private async void DoListPersonGroups()
        {
            await ListPersonGroups();
        }

        private async void DoListPersons()
        {
            if (string.IsNullOrWhiteSpace(SelectedPersonGroup.PersonGroupId))
            {
                _logger.Warn("Unable to list persons, no person group is selected");
                return;
            }
            try
            {
                var persons = await _faceClient.PersonGroupPerson.ListAsync(SelectedPersonGroup.PersonGroupId);
                Persons = persons.Select(p => p.CreatePerson());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private async void DoRecognizeFace()
        {
            if (string.IsNullOrWhiteSpace(SelectedPersonGroup.PersonGroupId))
            {
                _logger.Warn("Unable to DoRecognizeFace, no person group is selected");
                return;
            }
            var imagePath = _getImagePath?.Invoke();
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                _logger.Warn("Unable to add face to person, imagePath is empty");
                return;
            }
            try
            {
                using (var imageFileStream = File.OpenRead(imagePath))
                {
                    var faces = await _faceClient.Face.DetectWithStreamAsync(imageFileStream);
                    var faceIds = faces.Select(f => f.FaceId.Value).ToList();
                    if (faceIds.Count > 0)
                    {
                        _logger.Info("Face detected");
                        var identifies = await _faceClient.Face.IdentifyAsync(faceIds, SelectedPersonGroup.PersonGroupId, null, 1, 0.7);
                        foreach (var identify in identifies)
                        {
                            if (identify.Candidates.Count > 0)
                            {
                                var candidate = identify.Candidates[0];
                                var personId = candidate.PersonId;
                                var person = await _faceClient.PersonGroupPerson.GetAsync(SelectedPersonGroup.PersonGroupId, personId);
                                _logger.Info($"Person found: {person.Name} with confidence: {candidate.Confidence * 100}%");
                                Messenger.Default?.Send(new FaceRecognizedMessage());
                            }
                            else
                            {
                                _logger.Info("No candidates - unabel to recognize face");
                            }
                        }
                    }
                    else
                    {
                        _logger.Info("No faces detected");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private async void DoTrain()
        {
            if (string.IsNullOrWhiteSpace(SelectedPersonGroup.PersonGroupId))
            {
                _logger.Warn("Unable to DoTrain, no person group is selected");
                return;
            }
            try
            {
                await _faceClient.PersonGroup.TrainAsync(SelectedPersonGroup.PersonGroupId);
                while (true)
                {
                    var status = await _faceClient.PersonGroup.GetTrainingStatusAsync(SelectedPersonGroup.PersonGroupId);
                    if (status.Status != azure.TrainingStatusType.Running)
                    {
                        _logger.Info($"PersonGroup {SelectedPersonGroup.PersonGroupId} finshed training with status {status.Status}");
                        break;
                    }
                    await Task.Delay(500);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}