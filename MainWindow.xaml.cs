using LastIRead.Data.Instance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using LastIRead.Import;

namespace LastIRead {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private List<IReadable> readableList = new List<IReadable>();

        public MainWindow() {
            InitializeComponent();

            ReadList.ItemsSource = readableList;

            var view = (CollectionView)CollectionViewSource.GetDefaultView(readableList);
            view.Filter = ListFilter;
        }

        private bool ListFilter(object item) {
            var searchText = SearchBox.Text;
            if (string.IsNullOrEmpty(searchText)) {
                return true;
            } else {
                return (item as IReadable).Title.Contains(searchText, StringComparison.OrdinalIgnoreCase);
            }
        }

        private void IncrementButton_Click(object sender, RoutedEventArgs e) {
            e.Handled = true;
            var data = (IReadable)((Button)sender).DataContext;
            data.IncrementProgress();
            Save();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e) {
            var newReadable = new GenericReadable();
            var result = EditItem(newReadable);
            if (result) {
                readableList.Add(newReadable);
                Save();
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e) {
            if (ReadList.SelectedItems.Count != 1) return;

            var data = (GenericReadable)ReadList.SelectedItem;
            var result = MessageBox.Show($"Are you sure you want to delete {data.Title}?", "Delete confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                readableList.Remove(data);
                Save();
            }

        }

        private void ReadList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (ReadList.SelectedItems.Count == 1) {
                var readable = (IReadable)ReadList.SelectedItem;
                if (EditItem(readable)) {
                    Save();
                }
            }
        }

        private bool EditItem(IReadable readable) {
            return new EditWindow(readable).ShowDialog() == true;
        }

        private void Save() {
            ReadList.Items.Refresh();
            CollectionViewSource.GetDefaultView(readableList).Refresh();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) {
            CollectionViewSource.GetDefaultView(readableList).Refresh();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e) {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "*.json|*.csv";
            if (dialog.ShowDialog() == true) {
                var path = dialog.FileName;
                var extension = Path.GetExtension(path);

                if (extension == ".json") {

                } else if (extension == ".csv") {
                    var file = new FileInfo(dialog.FileName);
                    try {
                        var list = new CSVImport().Import(file);
                        readableList.AddRange(list);
                        Save();
                    } catch (Exception exception) {
                        MessageBox.Show($"Import of file {dialog.FileName} failed. {exception.Message}.", "Import failed.", MessageBoxButton.OK);
                    }
                }
            }
        }
    }
}