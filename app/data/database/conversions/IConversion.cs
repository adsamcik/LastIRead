using LiteDB;

namespace LastIRead.data.database.conversions {
	public interface IConversion {
		public void Convert(LiteDatabase database);
	}
}