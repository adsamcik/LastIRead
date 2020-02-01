using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using LastIRead.data.database;
using LastIRead.Data.Instance;
using LastIRead.Extensions;
using LastIRead.Import;
using Microsoft.Win32;

namespace LastIRead {
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow {
		public MainWindow() {
			InitializeCulture();
			InitializeComponent();
			Refresh();
			UpdateBrushes();
		}

		private static void UpdateBrushes() {
			var brush = (SolidColorBrush)Application.Current.Resources["SystemAltHighColorBrush"];
			brush.Opacity = 0.1;
			Application.Current.Resources["BackgroundBrush"] = brush;
		}

		private static void InitializeCulture() {
			LanguageProperty.OverrideMetadata(
				typeof(FrameworkElement),
				new FrameworkPropertyMetadata(
					XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
		}

		private void IncrementButton_Click(object sender, RoutedEventArgs e) {
			e.Handled = true;
			var data = (IReadable)((Button)sender).DataContext;
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
						var data = (GenericReadable)ReadList.SelectedItem;
						result = MessageBox.Show(
							$"Are you sure you want to delete {data.Title}?",
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

		private void ReadList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if (ReadList.SelectedItems.Count != 1) {
				return;
			}

			var readable = (IReadable)ReadList.SelectedItem;
			if (EditItem(readable)) {
				Update(readable);
			}
		}

		private void Update(IReadable item) {
			using var ds = new DataStore();
			ds.Update(item);
			Refresh(ds);
		}

		private void Insert(IReadable item) {
			using var ds = new DataStore();
			ds.Delete(item);
			Refresh(ds);
		}

		private void Insert(IEnumerable<IReadable> items) {
			using var ds = new DataStore();
			ds.Insert(items);
			Refresh(ds);
		}

		private void Delete(IEnumerable<IReadable> items) {
			using var ds = new DataStore();
			ds.Delete(items);
			Refresh(ds);
		}

		private static bool EditItem(IReadable readable) {
			return new EditWindow(readable).ShowDialog() == true;
		}

		private void Refresh() {
			using var ds = new DataStore();
			Refresh(ds);
		}

		private void Refresh(DataStore dataStore) {
			ReadList.ItemsSource = dataStore.GetSelected(SearchBox.Text);
		}

		private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) {
			Refresh();
		}

		private async void ImportButton_Click(object sender, RoutedEventArgs e) {
			var culture = CultureInfo.CurrentUICulture;
			var importers = GetImplementors<IDataImporter>();
			var extensions = importers.SelectMany(x => x.ImportExtensions).Distinct().ToList();
			var filterMap = extensions.Select(x => $"{x.ToUpper(culture)}|*.{x.ToLower(culture)}");
			var allExtensions = string.Join(';', extensions.Select(x => $"*.{x.ToLower(culture)}"));

			var dialog = new OpenFileDialog {
				Multiselect = false,
				Filter = $"All|{allExtensions}|{string.Join('|', filterMap)}"
			};
			if (dialog.ShowDialog() != true) {
				return;
			}

			var path = dialog.FileName;
			var extension = Path.GetExtension(path);

			var importer = importers.First(
				di => di.ImportExtensions.Any(
					ext => extension.Contains(ext, StringComparison.InvariantCultureIgnoreCase)
				)
			);
			try {
				var list = await importer.Import(new FileInfo(path)).ConfigureAwait(true);
				Insert(list);
			} catch (Exception exception) {
				MessageBox.Show(
					$"Import of file {dialog.FileName} failed. {exception.Message}.",
					"Import failed.",
					MessageBoxButton.OK
				);
				Console.WriteLine(exception.StackTrace);
			}
		}

		private void ExportButton_Click(object sender, RoutedEventArgs e) {
			var culture = CultureInfo.CurrentUICulture;
			var exporters = GetImplementors<IDataExporter>();
			var filterMap = exporters.SelectMany(x => x.ExportExtensions)
									 .Select(x => $"{x.ToUpper(culture)}|*.{x.ToLower(culture)}");

			var dialog = new SaveFileDialog {
				Filter = string.Join('|', filterMap)
			};
			if (dialog.ShowDialog() != true) {
				return;
			}

			using var ds = new DataStore();
			var list = ds.GetAll();
			var exporter = exporters[dialog.FilterIndex - 1];
			exporter.Export(list, new FileInfo(dialog.FileName));
		}

		private void Window_KeyDown(object sender, KeyEventArgs e) {
			if (SearchBox.IsFocused) return;

			var character = e.Key.ToChar();
			if (!char.IsControl(character) || e.Key == Key.Back) {
				SearchBox.Focus();
			}
		}

		private static List<T> GetImplementors<T>() {
			var type = typeof(T);
			return AppDomain.CurrentDomain
							.GetAssemblies()
							.SelectMany(x => x.GetTypes())
							.Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
							.Select(x => (T)Activator.CreateInstance(x))
							.ToList();
		}
	}
}