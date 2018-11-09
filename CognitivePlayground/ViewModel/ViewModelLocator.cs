using Hodor.Model.Interface;
using Hodor.ViewModel.Tabs;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using log4net;

namespace Hodor.ViewModel
{
    public class ViewModelLocator
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ViewModelLocator));
        private static object _configuraitonManagerLocker = new object();
        private static IConfigurationManager _configurationManager;

        public ViewModelLocator()
        {
            DispatcherHelper.Initialize();

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<CameraTabViewModel>(false);
            SimpleIoc.Default.Register<HodorTabViewModel>(false);
            SimpleIoc.Default.Register<ActionTabViewModel>(false);
            SimpleIoc.Default.Register<LogsTabViewModel>(false);

            //TODO: move registering non ViewModels to different file
            SimpleIoc.Default.Register(CreateConfigurationManager, false);
        }

        public ActionTabViewModel ActionTabViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ActionTabViewModel>();
            }
        }

        public CameraTabViewModel CameraTabViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CameraTabViewModel>();
            }
        }

        public HodorTabViewModel HodorTabViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HodorTabViewModel>();
            }
        }

        public LogsTabViewModel LogsTabViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LogsTabViewModel>();
            }
        }

        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
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
    }
}