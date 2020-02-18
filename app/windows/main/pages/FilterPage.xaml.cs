using System;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;

namespace LastIRead.windows.main.pages {
	/// <summary>
	/// Interaction logic for FilterPage.xaml
	/// </summary>
	public partial class FilterPage : Page {
		private readonly string[] filterItems = new string[] {
				Filter.Reading.ToString(),
				Filter.Dropped.ToString(),
				Filter.Ongoing.ToString(),
				Filter.Finished.ToString()
			};

		public readonly FilterData filterData;

		public FilterPage(FilterData filterData) {
			InitializeComponent();

			this.filterData = filterData;

			HideComboBox.ItemsSource = filterItems;
			SetHideComboBox(filterData);
		}

		private void SetHideComboBox(FilterData filterData) {
			var selectedList = new List<string>();
			for (int i = 0; i < filterItems.Length; i++) {
				if (filterData.filter.HasFlag(Enum.Parse<Filter>(filterItems[i]))) {
					selectedList.Add(filterItems[i]);
				}
			}

			HideComboBox.SelectedItems = selectedList;
		}

		public Filter GetHideComboBox() {
			return (Filter)HideComboBox.SelectedItems.Cast<string>().Sum(x => (int)Enum.Parse<Filter>(x));
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
