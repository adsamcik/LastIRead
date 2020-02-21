using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using LastIRead.data.database;
using LastIRead.Extensions;
using LastIRead.Import;
using static LastIRead.tools.ReflectionTools;
using LastIRead.windows.main.pages;
using LastIRead.windows.settings;
using Microsoft.Win32;

namespace LastIRead {
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow {
		private readonly DataStore _dataStore = new DataStore();

		private FilterData _filterData = new FilterData {
			Hide = Filter.Abandoned | Filter.Finished
		};

		private bool _isInFilterPage;

		private readonly ListPage _listPage;

		public MainWindow() {
			InitializeCulture();
			InitializeComponent();
			UpdateBrushes();

			_listPage = new ListPage(_dataStore, SearchBox);
			_listPage.UpdateFilter(_filterData);
			PageFrame.Content = _listPage;
			PageFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
		}

		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			_dataStore.Dispose();
		}

		private static void UpdateBrushes() {
			var brush = (SolidColorBrush) Application.Current.Resources["SystemAltHighColorBrush"];
			brush = new SolidColorBrush(brush.Color) {Opacity = 0.1};
			Application.Current.Resources["BackgroundBrush"] = brush;
		}

		private static void InitializeCulture() {
			LanguageProperty.OverrideMetadata(
				typeof(FrameworkElement),
				new FrameworkPropertyMetadata(
					XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)
				)
			);
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
				_dataStore.Insert(list);
				TryRefresh();
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

		/// <summary>
		///     Refreshes content if needed.
		/// </summary>
		private void TryRefresh() { }

		private void Window_KeyDown(object sender, KeyEventArgs e) {
			if (SearchBox.IsFocused) return;

			var character = e.Key.ToChar();
			if (!char.IsControl(character) || e.Key == Key.Back) {
				SearchBox.Focus();
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			new SettingsWindow().Show();
		}

		private void FilterButton_Click(object sender, RoutedEventArgs e) {
			if (_isInFilterPage) {
				_filterData = ((FilterPage) PageFrame.Content).FilterData;
				PageFrame.GoBack();
				_isInFilterPage = false;
				_listPage.UpdateFilter(_filterData);
			} else {
				PageFrame.Navigate(new FilterPage(_filterData));
				_isInFilterPage = true;
			}
		}
	}
}