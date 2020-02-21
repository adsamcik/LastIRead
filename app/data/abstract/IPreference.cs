using LastIRead.data.database;

namespace LastIRead {
	public interface IPreference : IDatabaseItem {
		public string Name { get; set; }
		public string Value { get; set; }
	}
}