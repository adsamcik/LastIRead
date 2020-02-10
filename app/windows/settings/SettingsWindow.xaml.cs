using LastIRead.windows.settings.pages;

namespace LastIRead.windows.settings {
	/// <summary>
	///     Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow {
		public SettingsWindow() {
			InitializeComponent();

			MainFrame.Content = new LicensePage();
		}
	}
}