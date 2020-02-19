using System;
using System.Linq;

namespace LastIRead.windows.main.pages {
	/// <summary>
	///     Interaction logic for FilterPage.xaml
	/// </summary>
	public partial class FilterPage {
		private readonly string[] _filterItems = {
			Filter.Reading.ToString(),
			Filter.Abandoned.ToString(),
			Filter.Ongoing.ToString(),
			Filter.Ended.ToString(),
			Filter.Finished.ToString()
		};

		public FilterPage(FilterData filterData) {
			InitializeComponent();

			HideComboBox.ItemsSource = _filterItems;
			SetHideComboBox(filterData);
		}

		private void SetHideComboBox(FilterData filterData) {
			var selectedList = _filterItems
			                   .Where(item => filterData.Hide.HasFlag(Enum.Parse<Filter>(item)))
			                   .ToList();

			HideComboBox.SelectedItems = selectedList;
		}

		private Filter GetHideComboBox() {
			return (Filter) HideComboBox.SelectedItems.Cast<string>().Sum(x => (int) Enum.Parse<Filter>(x));
		}

		public FilterData FilterData =>
			new FilterData {
				Hide = GetHideComboBox()
			};
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