using System.Globalization;
using System.Windows;
using LastIRead.data.extensions;
using LastIRead.Data.Instance;

namespace LastIRead {
	/// <summary>
	///     Interaction logic for EditWindow.xaml
	/// </summary>
	public partial class EditWindow {
		private IReadable Readable { get; }

		public EditWindow(IReadable readable) {
			if (readable == null) {
				Close();
				Readable = new GenericReadable();
				return;
			}

			InitializeComponent();

			Readable = readable;
			InitializeFields(readable);
		}

		private void InitializeFields(IReadable readable) {
			var culture = CultureInfo.CurrentUICulture;
			LocalizedTitleInput.Text = readable.LocalizedTitle;
			OriginalTitleInput.Text = readable.OriginalTitle;

			Title = readable.GetTitle() ?? "New readable";

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

			Readable.MaxProgress = MaxProgressInput.Value ?? 0;

			var progressValue = CurrentProgressInput.Value;
			if (progressValue.HasValue) {
				Readable.LogProgress(progressValue.Value);
			}

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

		private void StatsButton_Click(object sender, RoutedEventArgs e) { }
	}
}