using LastIRead;
using LastIRead.Data.Instance;
using LastIRead.windows.settings;
using Xunit;

namespace Tests {
	public class WindowLaunchTests {
		[StaFact]
		public void MainLaunchTest() {
			new MainWindow().Show();
		}

		[StaFact]
		public void EditLaunchTest() {
			var item = new GenericReadable();
			new EditWindow(item).Show();
		}

		[StaFact]
		public void SettingsLaunchTest() {
			new SettingsWindow().Show();
		}
	}
}