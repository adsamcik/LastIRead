using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace LastIRead {
	/// <summary>
	///     Interaction logic for EditWindow.xaml
	/// </summary>
	public partial class EditWindow {
		public EditWindow(IReadable readable) {
			Contract.Requires(readable != null);

			InitializeComponent();

			this.Readable = readable;
			InitializeFields(readable);
		}

		private IReadable Readable { get; }

		private void InitializeFields(IReadable readable) {
			var culture = CultureInfo.CurrentUICulture;
			TitleInput.Text = readable.Title;

			Title = string.IsNullOrEmpty(readable.Title) ? "New readable" : readable.Title;

			LastChapterLabel.Content = readable.Progress.ToString(culture.NumberFormat);
			OngoingCheckbox.IsChecked = readable.Ongoing;
			AbandonedCheckbox.IsChecked = readable.Abandoned;
			MaxProgressInput.Text = readable.MaxProgress.ToString(culture.NumberFormat);
		}

		private void UpdateFromFields() {
			Readable.Ongoing = OngoingCheckbox.IsChecked == true;
			Readable.Abandoned = AbandonedCheckbox.IsChecked == true;

			var progressValue = CurrenProgressInput.Value;
			if (progressValue != null) Readable.LogProgress((double) progressValue);

			Readable.MaxProgress = MaxProgressInput.Value ?? 0;
			Readable.Title = TitleInput.Text;
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