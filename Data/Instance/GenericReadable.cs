using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastIRead.Data.Instance {

    /// <summary>
    /// Generic readable implementation for most reading materials.
    /// </summary>
    class GenericReadable : IReadable {
        public string Title { get; set; }

        public int MaxProgress { get; set; }

        public bool Ongoing { get; set; }

        public DateTime LastRead => LastProgress.Date;

        public int Progress => LastProgress.Value;

        public IProgress LastProgress => (History.Count > 0) ? History.Last() : new GenericProgress(DateTime.MinValue, 0);


        public IList<IProgress> History { get; } = new List<IProgress>();
        public bool Abandoned { get; set; }

        public void IncrementProgress() {
            LogProgress(Progress + 1);
        }

        public void LogProgress(int progress) {
            if (!Ongoing && MaxProgress > 0) {
                progress = Math.Min(MaxProgress, progress);
            }

            var newProgress = new GenericProgress(DateTime.Today, progress);
            if (LastRead != newProgress.Date) {
                History.Add(newProgress);
            } else {
                History[History.Count - 1] = newProgress;
            }
        }
    }
}
