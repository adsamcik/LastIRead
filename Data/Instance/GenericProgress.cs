using System;

namespace LastIRead.Data.Instance {
    struct GenericProgress : IProgress {

        public DateTime Date { get; }

        public int Value { get; private set; }

        public GenericProgress(DateTime date, int progress) {
            Date = date;
            Value = progress;
        }
    }
}
