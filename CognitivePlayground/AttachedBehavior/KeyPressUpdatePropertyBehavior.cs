using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace CognitivePlayground.AttachedBehavior
{
    public class KeyPressUpdatePropertyBehavior
    {
        public static readonly DependencyProperty KeyPressUpdateProperty = DependencyProperty.RegisterAttached(
            "KeyPressUpdateProperty", typeof(DependencyProperty), typeof(KeyPressUpdatePropertyBehavior), new PropertyMetadata(null, OnUpdateKeyPressUpdateProperty));

        static KeyPressUpdatePropertyBehavior()
        {
        }

        public static DependencyProperty GetKeyPressUpdateProperty(DependencyObject dp)
        {
            return (DependencyProperty)dp.GetValue(KeyPressUpdateProperty);
        }

        public static void SetKeyPressUpdateProperty(DependencyObject dp, DependencyProperty value)
        {
            dp.SetValue(KeyPressUpdateProperty, value);
        }

        private static void DoUpdateSource(object source)
        {
            var property = GetKeyPressUpdateProperty(source as DependencyObject);
            if (property == null)
            {
                return;
            }

            var elt = source as UIElement;
            if (elt == null)
            {
                return;
            }

            var binding = BindingOperations.GetBindingExpression(elt, property);
            if (binding != null)
            {
                binding.UpdateSource();
            }
        }

        private static void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoUpdateSource(e.Source);
            }
        }

        private static void OnUpdateKeyPressUpdateProperty(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            var element = dp as UIElement;

            if (element == null)
            {
                return;
            }

            if (e.OldValue != null)
            {
                element.PreviewKeyDown -= HandlePreviewKeyDown;
            }

            if (e.NewValue != null)
            {
                element.PreviewKeyDown += new KeyEventHandler(HandlePreviewKeyDown);
            }
        }
    }
}