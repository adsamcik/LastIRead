using System;
using System.Collections.Generic;

namespace LastIRead {
	public interface IReadable {
		/// <summary>
		/// Title of the reading material
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// Max progress indicates how long the reading material is.
		/// </summary>
		double MaxProgress { get; set; }

		/// <summary>
		/// Indicates whether the reading material is still being added to.
		/// This allows max progress to expand if actual progress is larger.
		/// </summary>
		bool Ongoing { get; set; }

		/// <summary>
		/// Is abandoned.
		/// </summary>
		bool Abandoned { get; set; }

		/// <summary>
		/// Date of last progress change.
		/// </summary>
		DateTime LastRead { get; }

		/// <summary>
		/// Latest progress data.
		/// </summary>
		double Progress { get; }

		/// <summary>
		/// Complete progress history of reading.
		/// </summary>
		IList<IProgress> History { get; }

		/// <summary>
		/// Increments progress by 1. If this is first time reading that day
		/// also creates a new record.
		/// </summary>
		void IncrementProgress();

		/// <summary>
		/// Updates reading progress with given progress value.
		/// </summary>
		/// <param name="progress">Progress value</param>
		void LogProgress(double progress);
	}
}