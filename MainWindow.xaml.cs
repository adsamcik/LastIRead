using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using LastIRead.Data.Instance;
using LastIRead.Extensions;
using LastIRead.Import;
using LastIRead.Import.Implementation;
using Microsoft.Win32;

namespace LastIRead {
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow {
		private readonly List<IReadable> _readableList = new List<IReadable>();

		private readonly string _databasePath =
			$"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}reading-list.json";

		private string _strippedSearchString;

		public MainWindow() {
			InitializeComponent();

			ReadList.ItemsSource = _readableList;

			var view = (CollectionView) CollectionViewSource.GetDefaultView(_readableList);
			view.Filter = ListFilter;

			Load();

			UpdateBrushes();
		}

		private void UpdateBrushes() {
			var brush = (SolidColorBrush) Application.Current.Resources["SystemAltHighColorBrush"];
			brush.Opacity = 0.3;
			Application.Current.Resources["BackgroundBrush"] = brush;
		}

		private bool ListFilter(object item) {
			if (string.IsNullOrEmpty(_strippedSearchString)) {
				return true;
			}

			var readable = item as IReadable;
			Debug.Assert(readable != null, nameof(readable) + " != null");

			var title = readable.Title;
			var titleStripped = StripString(title);
			return titleStripped.Contains(_strippedSearchString, StringComparison.OrdinalIgnoreCase);
		}

		private static string StripString(string text) {
			var selectedCharacters = text
				.Where(
					character =>
						!char.IsPunctuation(character) &&
						!char.IsWhiteSpace(character) &&
						!char.IsSeparator(character)
				);

			return string.Concat(selectedCharacters);
		}

		private void IncrementButton_Click(object sender, RoutedEventArgs e) {
			e.Handled = true;
			var data = (IReadable) ((Button) sender).DataContext;
			data.IncrementProgress();
			Save();
		}

		private void AddButton_Click(object sender, RoutedEventArgs e) {
			var newReadable = new GenericReadable();
			var result = EditItem(newReadable);
			if (!result) {
				return;
			}

			_readableList.Add(newReadable);
			Save();
		}

		private void RemoveButton_Click(object sender, RoutedEventArgs e) {
			if (ReadList.SelectedItems.Count != 1) return;

			var data = (GenericReadable) ReadList.SelectedItem;
			var result = MessageBox.Show(
				$"Are you sure you want to delete {data.Title}?",
				"Delete confirmation",
				MessageBoxButton.YesNo
			);
			if (result != MessageBoxResult.Yes) {
				return;
			}

			_readableList.Remove(data);
			Save();
		}

		private void ReadList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if (ReadList.SelectedItems.Count != 1) {
				return;
			}

			var readable = (IReadable) ReadList.SelectedItem;
			if (EditItem(readable)) {
				Save();
			}
		}

		private bool EditItem(IReadable readable) {
			return new EditWindow(readable).ShowDialog() == true;
		}

		private async void Save() {
			Refresh();

			await new JsonDataHandler().Export(_readableList, new FileInfo(_databasePath)).ConfigureAwait(false);
		}

		private async void Load() {
			try {
				_readableList.Clear();
				var loadedList = await new JsonDataHandler().Import(new FileInfo(_databasePath)).ConfigureAwait(true);
				_readableList.AddRange(loadedList);

				Refresh();
			} catch (Exception) {
				//Ignore for now
			}
		}

		private void Refresh() {
			ReadList.Items.Refresh();
			Filter();
		}

		private void Filter() {
			CollectionViewSource.GetDefaultView(_readableList).Refresh();
		}

		private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) {
			_strippedSearchString = StripString(SearchBox.Text);
			Filter();
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
				_readableList.AddRange(list);
				Save();
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

			var exporter = exporters[dialog.FilterIndex - 1];
			exporter.Export(_readableList, new FileInfo(dialog.FileName));
		}

		private static List<T> GetImplementors<T>() {
			var type = typeof(T);
			return AppDomain.CurrentDomain
			                .GetAssemblies()
			                .SelectMany(x => x.GetTypes())
			                .Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
			                .Select(x => (T) Activator.CreateInstance(x))
			                .ToList();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e) {
			if (SearchBox.IsFocused) return;

			var character = e.Key.ToChar();
			if (!char.IsControl(character) || e.Key == Key.Back) {
				SearchBox.Focus();
			}
		}
	}
}