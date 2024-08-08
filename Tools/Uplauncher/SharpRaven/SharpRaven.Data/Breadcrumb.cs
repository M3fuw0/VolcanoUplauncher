using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SharpRaven.Serialization;

namespace SharpRaven.Data
{
	public class Breadcrumb
	{
		private readonly DateTime timestamp;

		[JsonConverter(typeof(LowerInvariantStringEnumConverter))]
		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public BreadcrumbType? Type { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Category { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public DateTime Timestamp => timestamp;

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Message { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public IDictionary<string, string> Data { get; set; }

		[JsonConverter(typeof(LowerInvariantStringEnumConverter))]
		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public BreadcrumbLevel? Level { get; set; }

		public Breadcrumb(string category)
		{
			Category = category;
			timestamp = DateTime.UtcNow;
		}

		public Breadcrumb(string category, BreadcrumbType type)
			: this(category)
		{
			Type = type;
		}
	}
}
