using System;

namespace LastIRead {
    public interface IProgress {
        DateTime Date { get; }

        int Value { get; }
    }
}
