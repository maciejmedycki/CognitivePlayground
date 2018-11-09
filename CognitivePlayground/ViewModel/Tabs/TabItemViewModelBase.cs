using Hodor.Model.Interface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace Hodor.ViewModel.Tabs
{
    public abstract class TabItemViewModelBase : ViewModelBase
    {
        private IConfigurationManager _configurationManager;
        public IConfigurationManager ConfigurationManager
        {
            get
            {
                if (_configurationManager == null)
                {
                    _configurationManager = SimpleIoc.Default.GetInstance<IConfigurationManager>();
                }
                return _configurationManager;
            }
        }
        public abstract string Title { get; }        
    }
}