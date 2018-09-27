using System.Collections.Generic;

namespace CognitivePlayground.Model.Interface
{
    public interface IConfigurationManager
    {
        IEnumerable<Action> GetActions();

        string GetAzureApiUri();

        string GetAzureSubscriptionKey();

        string GetVideoStreamAddress();

        void SaveActions(IEnumerable<Action> actions);

        void SaveVideoStreamAddress(string address);
    }
}