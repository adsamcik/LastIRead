using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper.Configuration.Attributes;
using LiteDB;
using Newtonsoft.Json;

namespace LastIRead.Data.Instance {
	/// <summary>
	///     Generic readable implementation for most reading materials.
	/// </summary>
	internal class GenericReadable : BaseReadable {
		public override ObjectId Id { get; set; }
		public override string Title { get; set; }

		[Optional]
		public override double MaxProgress { get; set; }

		[Optional]
		public override bool Ongoing { get; set; }

		[Optional]
		public override bool Abandoned { get; set; }


		[Optional]
		[JsonProperty]
		public override IList<IProgress> History { get; protected set; } = new List<IProgress>();

		public override double ProgressIncrement { get; set; }

		protected override IProgress CreateNewProgress(double progress) {
			return new GenericProgress(DateTime.Today, progress);
		}
	}
}