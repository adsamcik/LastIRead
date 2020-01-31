using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;

namespace LastIRead.Data.Instance {
	/// <summary>
	///     Generic readable implementation for most reading materials.
	/// </summary>
	internal class GenericReadable : IReadable {
		/// <summary>
		///     Last progress
		/// </summary>
		[Ignore]
		[JsonIgnore]
		public IProgress LastProgress =>
			History.Count > 0 ? History.Last() : new GenericProgress(DateTime.MinValue, 0);

		public string Title { get; set; }

		[Optional] public double MaxProgress { get; set; }

		[Optional] public bool Ongoing { get; set; }

		[Optional] public bool Abandoned { get; set; }

		/// <summary>
		///     Last date this was read.
		/// </summary>
		[Ignore]
		[JsonIgnore]
		public DateTime LastRead => LastProgress.Date;

		[Optional]
		[JsonIgnore]
		public double Progress {
			get => LastProgress.Value;
			set => LogProgress(value);
		}


		[Optional] [JsonProperty] public IList<IProgress> History { get; private set; } = new List<IProgress>();

		public void IncrementProgress() {
			LogProgress(Progress + 1);
		}

		public void LogProgress(double progress) {
			if (!Ongoing && MaxProgress > 0) progress = Math.Min(MaxProgress, progress);

			var newProgress = new GenericProgress(DateTime.Today, progress);
			if (LastRead != newProgress.Date)
				History.Add(newProgress);
			else
				History[History.Count - 1] = newProgress;
		}
	}
}