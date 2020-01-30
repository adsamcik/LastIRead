using LastIRead.Data.Instance;
using LastIRead.Extensions;
using LastIRead.Import;
using LastIRead.Import.Implementation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace LastIRead {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private string databasePath = $"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}reading-list.json";
        private List<IReadable> readableList = new List<IReadable>();

        public MainWindow() {
            InitializeComponent();

            ReadList.ItemsSource = readableList;

            var view = (CollectionView)CollectionViewSource.GetDefaultView(readableList);
            view.Filter = ListFilter;

            Load();

            UpdateBrushes();
        }

        private void UpdateBrushes() {
            var brush = (SolidColorBrush)Application.Current.Resources["SystemAltHighColorBrush"];
            brush.Opacity = 0.3;
            Application.Current.Resources["BackgroundBrush"] = brush;
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

        private async void Save() {
            Refresh();

            await new JSONDataHandler().Export(readableList, new FileInfo(databasePath));
        }

        private async void Load() {
            try {
                readableList.Clear();
                var loadedList = await new JSONDataHandler().Import(new FileInfo(databasePath));
                readableList.AddRange(loadedList);

                Refresh();
            } catch (Exception) {
                //Ignore for now
            }
        }

        private void Refresh() {
            ReadList.Items.Refresh();
            CollectionViewSource.GetDefaultView(readableList).Refresh();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) {
            CollectionViewSource.GetDefaultView(readableList).Refresh();
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e) {
            var importers = GetImplementors<IDataImporter>();
            var extensions = importers.SelectMany(x => x.ImportExtensions).Distinct();
            var filterMap = extensions.Select(x => $"{x.ToUpper()}|*.{x.ToLower()}");
            var allExtensions = string.Join(';', extensions.Select(x => $"*.{x.ToLower()}"));

            var dialog = new OpenFileDialog {
                Multiselect = false,
                Filter = $"All|{allExtensions}|{string.Join('|', filterMap)}"
            };
            if (dialog.ShowDialog() == true) {
                var path = dialog.FileName;
                var extension = Path.GetExtension(path);

                var importer = importers.First(importer => importer.ImportExtensions.Any(e => extension.Contains(e, StringComparison.InvariantCultureIgnoreCase)));
                try {
                    var list = await importer.Import(new FileInfo(path));
                    readableList.AddRange(list);
                    Save();
                } catch (Exception exception) {
                    MessageBox.Show($"Import of file {dialog.FileName} failed. {exception.Message}.", "Import failed.", MessageBoxButton.OK);
                    Console.WriteLine(exception.StackTrace);
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e) {
            var exporters = GetImplementors<IDataExporter>();
            var filterMap = exporters.SelectMany(x => x.ExportExtensions).Select(x => $"{x.ToUpper()}|*.{x.ToLower()}");

            var dialog = new SaveFileDialog() {
                Filter = string.Join('|', filterMap)
            };
            if (dialog.ShowDialog() == true) {
                var path = dialog.FileName;
                var exporter = exporters[dialog.FilterIndex - 1];
                exporter.Export(readableList, new FileInfo(dialog.FileName));
            }
        }

        private List<T> GetImplementors<T>() {
            var type = typeof(T);
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                 .Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                 .Select(x => (T)Activator.CreateInstance(x))
                 .ToList();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (SearchBox.IsFocused) return;

            var character = e.Key.ToChar();
            if (!char.IsControl(character)) {
                SearchBox.Focus();
            }
        }
    }
}