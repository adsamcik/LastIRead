using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace LastIRead.windows.main.pages {
	/// <summary>
	///     Interaction logic for FilterPage.xaml
	/// </summary>
	public partial class FilterPage : Page {
		public readonly FilterData filterData;

		private readonly string[] filterItems = {
			Filter.Reading.ToString(),
			Filter.Dropped.ToString(),
			Filter.Ongoing.ToString(),
			Filter.Finished.ToString()
		};

		public FilterPage(FilterData filterData) {
			InitializeComponent();

			this.filterData = filterData;

			HideComboBox.ItemsSource = filterItems;
			SetHideComboBox(filterData);
		}

		private void SetHideComboBox(FilterData filterData) {
			var selectedList = new List<string>();
			for (var i = 0; i < filterItems.Length; i++) {
				if (filterData.filter.HasFlag(Enum.Parse<Filter>(filterItems[i]))) {
					selectedList.Add(filterItems[i]);
				}
			}

			HideComboBox.SelectedItems = selectedList;
		}

		public Filter GetHideComboBox() {
			return (Filter) HideComboBox.SelectedItems.Cast<string>().Sum(x => (int) Enum.Parse<Filter>(x));
		}
	}

	public struct FilterData {
		public Filter filter;
	}

	[Flags]
	public enum Filter {
		Reading = 1,
		Dropped = 2,
		Ongoing = 4,
		Finished = 8
	}
}