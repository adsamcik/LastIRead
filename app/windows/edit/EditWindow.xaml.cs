using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using LastIRead.Data.Instance;

namespace LastIRead {
	/// <summary>
	///     Interaction logic for EditWindow.xaml
	/// </summary>
	public partial class EditWindow {
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

		private IReadable Readable { get; }

		private void InitializeFields(IReadable readable) {
			var culture = CultureInfo.CurrentUICulture;
			LocalizedTitleInput.Text = readable.LocalizedTitle;
			OriginalTitleInput.Text = readable.OriginalTitle;

			UpdateTitle();

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

		private void UpdateTitle() {
			var titleValues = new List<string>();

			if (!string.IsNullOrEmpty(LocalizedTitleInput.Text)) {
				titleValues.Add(LocalizedTitleInput.Text);
			}

			if (!string.IsNullOrEmpty(OriginalTitleInput.Text)) {
				titleValues.Add(OriginalTitleInput.Text);
			}

			if (!titleValues.Any()) {
				titleValues.Add("New readable");
			}

			Title = string.Join(" - ", titleValues);
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