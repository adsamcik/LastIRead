using System;
using System.Collections.Generic;
using LiteDB;

namespace LastIRead.Data.Instance {
	public class UserReadableBookmark : IUserReadable, IBookmark {
		private readonly IBookmark _bookmark;

		public UserReadableBookmark(IBookmark bookmark) {
			_bookmark = bookmark ?? throw new ArgumentNullException(nameof(bookmark));
		}

		public string Title => _bookmark.LocalizedTitle ??
		                       _bookmark.OriginalTitle ??
		                       string.Empty;

		public string FormattedProgress => 
		public IProgress LastProgress { get; }
		public DateTime StartedReading { get; }
		public DateTime LastRead { get; }
		public ObjectId? Id { get; set; }
		public string? LocalizedTitle { get; set; }
		public string? OriginalTitle { get; set; }
		public double MaxProgress { get; set; }
		public bool Ongoing { get; set; }
		public bool Abandoned { get; set; }
		public IList<IProgress> History { get; }
		public double ProgressIncrement { get; set; }
		public double Progress { get; set; }
		public void IncrementProgress()

		public void LogProgress(double progress) {
			throw new NotImplementedException();
		}
	}
}