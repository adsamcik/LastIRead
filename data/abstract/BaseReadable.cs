using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper.Configuration.Attributes;
using LiteDB;
using Newtonsoft.Json;

namespace LastIRead {
	/// <summary>
	///     Base readable implementation providing utility methods for UI.
	/// </summary>
	public abstract class BaseReadable : IReadable {
		public abstract ObjectId Id { get; set; }
		public abstract string Title { get; set; }
		public abstract double MaxProgress { get; set; }
		public abstract bool Ongoing { get; set; }
		public abstract bool Abandoned { get; set; }
		public abstract IList<IProgress> History { get; protected set; }
		public abstract double ProgressIncrement { get; set; }

		[Optional]
		[JsonIgnore]
		[BsonIgnore]
		public DateTime StartedReading => History.FirstOrDefault()?.Date ?? DateTime.MinValue;

		[Ignore]
		[JsonIgnore]
		[BsonIgnore]
		public DateTime LastRead => LastProgress?.Date ?? DateTime.MinValue;

		[Optional]
		[JsonIgnore]
		[BsonIgnore]
		public double Progress {
			get => LastProgress?.Value ?? 0.0;
			set => LogProgress(value);
		}

		[Ignore]
		[JsonIgnore]
		[BsonIgnore]
		public IProgress LastProgress => History.LastOrDefault();

		public void IncrementProgress() {
			LogProgress(Progress + 1);
		}

		public void LogProgress(double progress) {
			if (!Ongoing && MaxProgress > 0) progress = Math.Min(MaxProgress, progress);

			var newProgress = CreateNewProgress(progress);
			if (LastRead != newProgress.Date) {
				History.Add(newProgress);
			} else {
				History[^1] = newProgress;
			}
		}

		protected abstract IProgress CreateNewProgress(double progress);
	}
}