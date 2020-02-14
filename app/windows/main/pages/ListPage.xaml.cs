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
	public partial class ListPage : Page {
		private readonly DataStore _dataStore;
		private readonly TextBox _searchBox;

		private ListSortDirection _lastDirection = ListSortDirection.Ascending;

		private GridViewColumnHeader _lastHeaderClicked;

		public ListPage(DataStore dataStore, TextBox searchBox) {
			_dataStore = dataStore;
			_searchBox = searchBox;
			InitializeComponent();
			Loaded += Page_Loaded;
			Unloaded += Page_Unloaded;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e) {
			_searchBox.TextChanged += SearchBox_TextChanged;
			Refresh();
		}

		private void Page_Unloaded(object sender, RoutedEventArgs e) {
			_searchBox.TextChanged -= SearchBox_TextChanged;
		}

		public void Refresh() {
			ReadList.ItemsSource = _dataStore.GetSelected(_searchBox.Text);
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

		private void Update(IReadable item) {
			_dataStore.Update(item);
			Refresh();
		}

		private void Insert(IReadable item) {
			_dataStore.Insert(item);
			Refresh();
		}

		private void Insert(IEnumerable<IReadable> items) {
			_dataStore.Insert(items);
			Refresh();
		}

		private void Delete(IEnumerable<IReadable> items) {
			_dataStore.Delete(items);
			Refresh();
		}

		private void EditSelectedItem() {
			if (ReadList.SelectedItems.Count != 1) {
				return;
			}

			var readable = (IReadable) ReadList.SelectedItem;
			if (!EditItem(readable)) {
				return;
			}

			Update(readable);
		}

		private static bool EditItem(IReadable readable) {
			return new EditWindow(readable).ShowDialog() == true;
		}

		private void IncrementButton_Click(object sender, RoutedEventArgs e) {
			e.Handled = true;
			var data = (IReadable) ((Button) sender).DataContext;
			data.IncrementProgress();
			Update(data);
		}

		private void AddButton_Click(object sender, RoutedEventArgs e) {
			var newReadable = new GenericReadable();
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
					var data = (GenericReadable) ReadList.SelectedItem;
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

			var selected = ReadList.SelectedItems.Cast<IReadable>();
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