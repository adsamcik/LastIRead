using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using LastIRead.data.database;
using LastIRead.Data.Instance;

namespace LastIRead.windows.main.pages {
	public partial class ListPage {
		private readonly DataStore _dataStore;
		private readonly TextBox _searchBox;

		private FilterData _filterData;

		private ListSortDirection _lastDirection = ListSortDirection.Ascending;

		private GridViewColumnHeader? _lastHeaderClicked;

		public ListPage(DataStore dataStore, TextBox searchBox) {
			_dataStore = dataStore;
			_searchBox = searchBox;
			InitializeComponent();
			Loaded += Page_Loaded;
			Unloaded += Page_Unloaded;
		}

		public void UpdateFilter(FilterData filterData) {
			_filterData = filterData;
			Refresh();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e) {
			_searchBox.TextChanged += SearchBox_TextChanged;
			Refresh();
		}

		private void Page_Unloaded(object sender, RoutedEventArgs e) {
			_searchBox.TextChanged -= SearchBox_TextChanged;
		}

		private void Refresh() {
			ReadList.ItemsSource = _dataStore.GetSelected(_searchBox.Text, _filterData);
		}

		private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) {
			Refresh();
		}

		private void ReadList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			EditSelectedItem();
		}

		private void EditButton_Click(object sender, RoutedEventArgs e) {
			EditSelectedItem();
		}

		private void Update(IPersistentBookmark item) {
			_dataStore.Update(item);
			Refresh();
		}

		private void Insert(IPersistentBookmark item) {
			_dataStore.Insert(item);
			Refresh();
		}

		private void Delete(IEnumerable<IPersistentBookmark> items) {
			_dataStore.Delete(items);
			Refresh();
		}

		private void EditSelectedItem() {
			if (ReadList.SelectedItems.Count != 1) {
				return;
			}

			var readable = (WrapperUserBookmark) ReadList.SelectedItem;
			if (!EditItem(readable)) {
				return;
			}

			Update(readable.Bookmark);
		}

		private static bool EditItem(IBookmark bookmark) {
			return new EditWindow(bookmark).ShowDialog() == true;
		}

		private void IncrementButton_Click(object sender, RoutedEventArgs e) {
			e.Handled = true;
			var data = (WrapperUserBookmark) ((Button) sender).DataContext;
			data.IncrementProgress();
			Update(data.Bookmark);
		}

		private void AddButton_Click(object sender, RoutedEventArgs e) {
			var newReadable = new GenericBookmark();
			var result = EditItem(newReadable);
			if (!result) {
				return;
			}

			Insert(newReadable);
		}

		private void RemoveButton_Click(object sender, RoutedEventArgs e) {
			var count = ReadList.SelectedItems.Count;
			MessageBoxResult result;
			switch (count) {
				case 0:
					return;
				case 1: {
					var data = (GenericBookmark) ReadList.SelectedItem;
					result = MessageBox.Show(
						$"Are you sure you want to delete {data.LocalizedTitle}?",
						"Delete confirmation",
						MessageBoxButton.YesNo
					);
					if (result != MessageBoxResult.Yes) {
						return;
					}

					break;
				}
				default:
					result = MessageBox.Show(
						$"Are you sure you want to delete {count} records?",
						"Mass delete confirmation",
						MessageBoxButton.YesNo
					);
					if (result != MessageBoxResult.Yes) {
						return;
					}

					break;
			}

			var selected = ReadList
			               .SelectedItems
			               .Cast<WrapperUserBookmark>()
			               .Select(x => x.Bookmark);
			Delete(selected);
		}

		private void GridViewColumnHeaderClickedHandler(
			object sender,
			RoutedEventArgs e
		) {
			if (!(e.OriginalSource is GridViewColumnHeader headerClicked)) {
				return;
			}

			if (headerClicked.Role == GridViewColumnHeaderRole.Padding) {
				return;
			}

			ListSortDirection direction;
			if (headerClicked != _lastHeaderClicked) {
				direction = ListSortDirection.Ascending;
			} else {
				direction = _lastDirection == ListSortDirection.Ascending
					? ListSortDirection.Descending
					: ListSortDirection.Ascending;
			}

			var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
			var sortBy = columnBinding?.Path.Path ?? (string) headerClicked.Column.Header;

			Sort(sortBy, direction);

			if (direction == ListSortDirection.Ascending) {
				headerClicked.Column.HeaderTemplate =
					Resources["HeaderTemplateArrowUp"] as DataTemplate;
			} else {
				headerClicked.Column.HeaderTemplate =
					Resources["HeaderTemplateArrowDown"] as DataTemplate;
			}

			// Remove arrow from previously sorted header
			if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked) {
				_lastHeaderClicked.Column.HeaderTemplate = null;
			}

			_lastHeaderClicked = headerClicked;
			_lastDirection = direction;
		}

		private void Sort(string sortBy, ListSortDirection direction) {
			var dataView = CollectionViewSource.GetDefaultView(ReadList.ItemsSource);

			dataView.SortDescriptions.Clear();
			var sd = new SortDescription(sortBy, direction);
			dataView.SortDescriptions.Add(sd);
			dataView.Refresh();
		}
	}
}