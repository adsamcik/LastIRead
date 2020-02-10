using LastIRead.windows.settings.pages;
using System.Windows;

namespace LastIRead.windows.settings {
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window {
		public SettingsWindow() {
			InitializeComponent();

			Content.Content = new LicensePage();
		}
	}
}
