using LastIRead;
using LastIRead.Data.Instance;
using LastIRead.windows.settings;
using Xunit;

namespace Tests {
	public class WindowLaunchTests {
		[Fact]
		public void MainLaunchTest() {
			new MainWindow().Show();
		}

		[Fact]
		public void EditLaunchTest() {
			var item = new GenericReadable();
			new EditWindow(item).Show();
		}

		[Fact]
		public void SettingsLaunchTest() {
			new SettingsWindow().Show();
		}
	}
}