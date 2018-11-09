using GalaSoft.MvvmLight.Messaging;

namespace Hodor.Model.Messages
{
    public class FaceDetectedMessage : MessageBase
    {
        public FaceDetectedMessage(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }
}