using System;

namespace LastIRead {
	/// <summary>
	/// Interface containing methods and properties required by UI.
	/// </summary>
	public interface IUiReadable {
		/// <summary>
		/// 	Title of the reading material either localized or original.
		/// </summary>
		string Title { get; }

		/// <summary>
		///     Returns last progress instance or default value if no progress is logged.
		/// </summary>
		public IProgress LastProgress { get; }

		/// <summary>
		///     Date of the first progress entry.
		/// </summary>
		public DateTime StartedReading { get; }

		/// <summary>
		///     Date of last progress change.
		/// </summary>
		public DateTime LastRead { get; }
	}
}