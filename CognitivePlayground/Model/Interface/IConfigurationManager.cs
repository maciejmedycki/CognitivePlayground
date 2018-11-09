namespace Hodor.Model.Interface
{
    public interface IConfigurationManager
    {
        ActionsWrapper GetActions();

        string GetAzureApiUri();

        string GetAzureSubscriptionKey();

        string GetVideoStreamAddress();

        void SaveActions(ActionsWrapper actions);

        void SaveVideoStreamAddress(string address);
    }
}