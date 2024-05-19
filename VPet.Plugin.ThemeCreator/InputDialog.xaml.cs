using LinePutScript.Localization.WPF;
using System;
using System.Windows;
using System.Windows.Input;

namespace VPet.Plugin.ThemeCreator
{
    public partial class InputDialog : Window
    {
        public string Code { get; private set; }
        public event EventHandler<string> OnTextEntered;

        public InputDialog()
        {
            this.InitializeComponent();
        }

        private void OkButton(object sender, RoutedEventArgs e)
        {
            this.Code = this.InputTextBox.Text;
            this.Close();
        }

        private void CancelButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Move(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
