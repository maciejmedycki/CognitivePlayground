using System.Windows;

namespace CognitivePlayground.Model
{
    public class FaceRectangle
    {
        public FaceRectangle()
        {
            Margin = new Thickness();
        }
        public double Height { get; set; }
        public Thickness Margin { get; set; }
        public double Width { get; set; }
    }
}