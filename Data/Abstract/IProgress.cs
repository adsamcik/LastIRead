using System;

namespace LastIRead {
    public interface IProgress {
        DateTime Date { get; }

        double Value { get; }
    }
}
