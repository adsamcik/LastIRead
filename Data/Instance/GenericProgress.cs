using System;

namespace LastIRead.Data.Instance {
    struct GenericProgress : IProgress {

        public DateTime Date { get; }

        public double Value { get; private set; }

        public GenericProgress(DateTime date, double progress) {
            Date = date;
            Value = progress;
        }
    }
}
