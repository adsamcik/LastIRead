using LiteDB;

namespace LastIRead.data.database {
	public class Preferences : DatabaseCollection<IPreference> {
		public Preferences(LiteDatabase database) : base(database, database.GetPreferenceCollection()) { }
	}
}