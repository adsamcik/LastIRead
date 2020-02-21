using System;
using System.Linq;
using LastIRead.data.database;
using Sdl.MultiSelectComboBox.EventArgs;

namespace LastIRead.windows.main.pages {
	/// <summary>
	///     Interaction logic for FilterPage.xaml
	/// </summary>
	public partial class FilterPage : IDisposable {
		private readonly string[] _filterItems = {
			Filter.Reading.ToString(),
			Filter.Abandoned.ToString(),
			Filter.Ongoing.ToString(),
			Filter.Ended.ToString(),
			Filter.Finished.ToString()
		};

		private readonly Preferences _preferences = new Preferences(AppDatabase.GetDatabase());

		private FilterData _filterData;

		public FilterPage(FilterData filterData) {
			_filterData = filterData;

			InitializeComponent();

			HideComboBox.ItemsSource = _filterItems;
			SetHideComboBox(filterData);

			HideComboBox.SelectedItemsChanged += OnHideSelectionChanged;
		}

		public FilterData FilterData => _filterData;

		public void Dispose() {
			AppDatabase.Dispose();
		}

		private void OnHideSelectionChanged(object? obj, SelectedItemsChangedEventArgs args) {
			foreach (string? removed in args.Removed) {
				if (Enum.TryParse<Filter>(removed, out var result)) {
					_filterData.Hide &= ~result;
				}
			}

			foreach (string? removed in args.Added) {
				if (Enum.TryParse<Filter>(removed, out var result)) {
					_filterData.Hide |= result;
				}
			}

			_preferences.UpdatePreference(Preferences.PrefListHide, _filterData.Hide);
		}

		private void SetHideComboBox(FilterData filterData) {
			var selectedList =
				_filterItems
					.Where(item => filterData.Hide.HasFlag(Enum.Parse<Filter>(item)))
					.ToList();

			HideComboBox.SelectedItems = selectedList;
		}
	}

	public struct FilterData {
		public Filter Hide;
	}

	[Flags]
	public enum Filter {
		Reading = 1,
		Abandoned = 2,
		Ongoing = 4,
		Ended = 8,
		Finished = 16
	}
}