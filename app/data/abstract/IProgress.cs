using System;

namespace LastIRead {
	/// <summary>
	///     Progress update interface
	/// </summary>
	public interface IProgress {
		/// <summary>
		///     Date of the progress update
		/// </summary>
		DateTime Date { get; }

		/// <summary>
		///     Progress value
		/// </summary>
		double Value { get; }
	}
}