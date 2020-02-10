using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LastIRead.resources;
using Markdown = Markdig.Wpf.Markdown;

namespace LastIRead.windows.settings.pages {
	/// <summary>
	/// Interaction logic for LicensePage.xaml
	/// </summary>
	public partial class LicensePage : Page {
		public LicensePage() {
			InitializeComponent();

			var document = Markdown.ToFlowDocument(LoadLicenses());
			var brush = (SolidColorBrush)Application.Current.Resources["SystemBaseHighColorBrush"];
			var secondaryBrush = (SolidColorBrush)Application.Current.Resources["SystemBaseMediumHighColorBrush"];
			foreach (var block in document.Blocks) {
				if (block.Foreground is SolidColorBrush blockBrush) {
					if (blockBrush.Color == Colors.Black) {
						block.Foreground = brush;
					} else {
						block.Foreground = secondaryBrush;
					}
				} else {
					block.Foreground = secondaryBrush;
				}
			}
			FlowReader.Document = document;
		}

		private static string LoadLicenses() {
			var resources = ResourceLoader.LoadTextResource("resources.licenses.");
			return string.Join(Environment.NewLine, resources);
			//Properties.Resources.ResourceManager.
		}
	}
}