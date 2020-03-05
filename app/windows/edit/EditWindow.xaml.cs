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
		public EditWindow(IBookmark bookmark) {
			if (bookmark == null) {
				Close();
				Bookmark = new GenericBookmark();
				return;
			}

			InitializeComponent();

			Bookmark = bookmark;
			InitializeFields(bookmark);
		}

		private IBookmark Bookmark { get; }

		private void InitializeFields(IBookmark bookmark) {
			var culture = CultureInfo.CurrentUICulture;
			LocalizedTitleInput.Text = bookmark.LocalizedTitle;
			OriginalTitleInput.Text = bookmark.OriginalTitle;

			UpdateTitle();

			LastChapterLabel.Content = $"Last {bookmark.Progress.ToString(culture.NumberFormat)}";
			OngoingCheckbox.IsChecked = bookmark.Ongoing;
			AbandonedCheckbox.IsChecked = bookmark.Abandoned;
			MaxProgressInput.Text = bookmark.MaxProgress.ToString(culture.NumberFormat);
			if (bookmark is IWebBookmark webReadable) {
				UrlInput.Text = webReadable.WebAddress;
			} else {
				UrlInput.IsEnabled = false;
			}

			if (!double.IsNaN(bookmark.ProgressIncrement)) {
				IncrementInput.Value = bookmark.ProgressIncrement;
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
			Bookmark.Ongoing = OngoingCheckbox.IsChecked == true;
			Bookmark.Abandoned = AbandonedCheckbox.IsChecked == true;

			Bookmark.MaxProgress = MaxProgressInput.Value ?? 0;

			var progressValue = CurrentProgressInput.Value;
			if (progressValue.HasValue) {
				Bookmark.LogProgress(progressValue.Value);
			}

			Bookmark.LocalizedTitle = LocalizedTitleInput.Text;
			Bookmark.OriginalTitle = OriginalTitleInput.Text;
			if (Bookmark is IWebBookmark webReadable) {
				webReadable.WebAddress = UrlInput.Text;
			}

			Bookmark.ProgressIncrement = IncrementInput.Value ?? 0.0;
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