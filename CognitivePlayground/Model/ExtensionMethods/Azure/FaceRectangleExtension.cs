using azure = Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CognitivePlayground.Model.ExtensionMethods.Azure
{
    public static class FaceRectangleExtension
    {
        public static FaceRectangle CreateFaceRectangle(this azure.FaceRectangle faceRectangle)
        {
            return new FaceRectangle
            {
                Width = faceRectangle.Width,
                Height = faceRectangle.Height,
                Left = faceRectangle.Left,
                Top = faceRectangle.Top
            };
        }

        public static void UpdateFaceRectangle(this azure.FaceRectangle sourceRectangle, ref FaceRectangle targetRectangle)
        {
            targetRectangle.Width = sourceRectangle.Width;
            targetRectangle.Height = sourceRectangle.Height;
            targetRectangle.Left = sourceRectangle.Left;
            targetRectangle.Top = sourceRectangle.Top;
        }
    }
}