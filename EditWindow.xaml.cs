using LastIRead.Data.Instance;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LastIRead {
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window {
        public IReadable readable { get; private set; }

        public EditWindow(IReadable readable) {
            InitializeComponent();

            this.readable = readable;
            InitializeFields(readable);
        }

        private void InitializeFields(IReadable readable) {
            TitleInput.Text = readable.Title;

            if (string.IsNullOrEmpty(readable.Title)) {
                Title = "New readable";
            } else {
                Title = readable.Title;
            }

            LastChapterLabel.Content = readable.Progress.ToString();
            OngoingCheckbox.IsChecked = readable.Ongoing;
            AbandonedCheckbox.IsChecked = readable.Abandoned;
            MaxProgressInput.Text = readable.MaxProgress.ToString();
        }

        private void UpdateFromFields() {
            readable.Ongoing = OngoingCheckbox.IsChecked == true;
            readable.Abandoned = AbandonedCheckbox.IsChecked == true;

            var progressValue = CurrenProgressInput.Value;
            if (progressValue != null) {
                readable.LogProgress((double)progressValue);
            }

            readable.MaxProgress = MaxProgressInput.Value ?? 0;
            readable.Title = TitleInput.Text;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e) {

            UpdateFromFields();

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9,.-]+");
        }
    }
}
