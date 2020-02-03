using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows;

namespace LastIRead {
	/// <summary>
	///     Interaction logic for EditWindow.xaml
	/// </summary>
	public partial class EditWindow {
		public EditWindow(IReadable readable) {
			Contract.Requires(readable != null);

			InitializeComponent();

			Readable = readable;
			InitializeFields(readable);
		}

		private IReadable Readable { get; }

		private void InitializeFields(IReadable readable) {
			var culture = CultureInfo.CurrentUICulture;
			LocalizedTitleInput.Text = readable.LocalizedTitle;

			Title = string.IsNullOrEmpty(readable.LocalizedTitle) ? "New readable" : readable.LocalizedTitle;

			LastChapterLabel.Content = $"Last {readable.Progress.ToString(culture.NumberFormat)}";
			OngoingCheckbox.IsChecked = readable.Ongoing;
			AbandonedCheckbox.IsChecked = readable.Abandoned;
			MaxProgressInput.Text = readable.MaxProgress.ToString(culture.NumberFormat);
			if (Readable is IWebReadable webReadable) {
				UrlInput.Text = webReadable.WebAddress;
			} else {
				UrlInput.IsEnabled = false;
			}
		}

		private void UpdateFromFields() {
			Readable.Ongoing = OngoingCheckbox.IsChecked == true;
			Readable.Abandoned = AbandonedCheckbox.IsChecked == true;

			var progressValue = CurrentProgressInput.Value;
			if (progressValue != null) Readable.LogProgress((double)progressValue);

			Readable.MaxProgress = MaxProgressInput.Value ?? 0;
			Readable.LocalizedTitle = LocalizedTitleInput.Text;
			Readable.OriginalTitle = OriginalTitleInput.Text;
			if (Readable is IWebReadable webReadable) {
				webReadable.WebAddress = UrlInput.Text;
			}
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
	}
}