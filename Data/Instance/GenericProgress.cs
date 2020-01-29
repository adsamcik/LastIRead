﻿using System;

namespace LastIRead.Data.Instance {
    /// <summary>
    /// Generic implementation of progress.
    /// Should represent most use cases.
    /// </summary>
    struct GenericProgress : IProgress {

        public DateTime Date { get; }

        public double Value { get; private set; }

        public GenericProgress(DateTime date, double progress) {
            Date = date;
            Value = progress;
        }
    }
}
