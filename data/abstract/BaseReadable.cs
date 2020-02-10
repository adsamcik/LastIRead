using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper.Configuration.Attributes;
using LastIRead.data.extensions;
using LiteDB;
using Newtonsoft.Json;

namespace LastIRead {
	/// <summary>
	///     Base readable implementation providing utility methods for UI.
	/// </summary>
	public abstract class BaseReadable : IReadable, IUiReadable {
		[Ignore]
		[JsonIgnore]
		public abstract ObjectId? Id { get; set; }

		[Optional]
		public abstract string? LocalizedTitle { get; set; }

		[Optional]
		public abstract string? OriginalTitle { get; set; }

		[Optional]
		public abstract double MaxProgress { get; set; }

		[Optional]
		public abstract bool Ongoing { get; set; }

		[Optional]
		public abstract bool Abandoned { get; set; }

		public abstract IList<IProgress> History { get; protected set; }
		public abstract double ProgressIncrement { get; set; }

		[Optional]
		[JsonIgnore]
		[BsonIgnore]
		public virtual double Progress {
			get => LastProgress?.Value ?? 0.0;
			set => LogProgress(value);
		}

		public virtual void IncrementProgress() {
			LogProgress(Progress + 1);
		}

		public virtual void LogProgress(double progress) {
			if (!Ongoing && MaxProgress > 0) {
				progress = Math.Min(MaxProgress, progress);
			}

			if (Ongoing) {
				MaxProgress = Math.Max(progress, MaxProgress);
			}

			var newProgress = CreateNewProgress(progress);
			if (LastRead != newProgress.Date) {
				History.Add(newProgress);
			} else {
				History[^1] = newProgress;
			}
		}

		[Optional]
		[JsonIgnore]
		[BsonIgnore]
		public virtual DateTime StartedReading => History.FirstOrDefault()?.Date ?? DateTime.MinValue;

		[Ignore]
		[JsonIgnore]
		[BsonIgnore]
		public virtual DateTime LastRead => LastProgress?.Date ?? DateTime.MinValue;

		[Ignore]
		[JsonIgnore]
		[BsonIgnore]
		public virtual string Title => this.GetTitle();

		[Ignore]
		[JsonIgnore]
		[BsonIgnore]
		public virtual IProgress LastProgress => History.LastOrDefault();

		protected abstract IProgress CreateNewProgress(double progress);
	}
}