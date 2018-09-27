using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using CognitivePlayground.ViewModel.Tabs;
using CognitivePlayground.Model.Interface;
using System.Configuration;
using log4net;

namespace CognitivePlayground.ViewModel
{
    public class ViewModelLocator
    {
        private static IConfigurationManager _configurationManager;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ViewModelLocator));
        private static object _configuraitonManagerLocker = new object();

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<CameraTabViewModel>(false);
            SimpleIoc.Default.Register<CognitivePlaygroundTabViewModel>(false);
            SimpleIoc.Default.Register<ActionTabViewModel>(false);
            SimpleIoc.Default.Register<LogsTabViewModel>(false);

            //TODO: move registering non ViewModels to different file
            SimpleIoc.Default.Register(CreateConfigurationManager, false);
        }

        private IConfigurationManager CreateConfigurationManager()
        {
            if (_configurationManager == null)
            {
                lock (_configuraitonManagerLocker)
                {
                    if (_configurationManager == null)
                    {
                        _configurationManager = new Model.ConfigurationManager();
                    }
                }
            }
            return _configurationManager;
        }

        public CameraTabViewModel CameraTabViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CameraTabViewModel>();
            }
        }

        public CognitivePlaygroundTabViewModel CognitivePlaygroundTabViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CognitivePlaygroundTabViewModel>();
            }
        }

        public ActionTabViewModel ActionTabViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ActionTabViewModel>();
            }
        }

        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public LogsTabViewModel LogsTabViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LogsTabViewModel>();
            }
        }
    }
}