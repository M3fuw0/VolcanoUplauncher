using System.Collections.Generic;
using System.Linq;
using SharpRaven.Logging.Filters;
using SharpRaven.Utilities;

namespace SharpRaven.Logging
{
	public class LogScrubber : IScrubber
	{
		private readonly List<IFilter> filters;

		public List<IFilter> Filters => filters;

		public LogScrubber()
		{
			filters = new List<IFilter>
			{
				new CreditCardFilter(),
				new PhoneNumberFilter()
			};
		}

		public string Scrub(string input)
		{
			if (SystemUtil.IsNullOrWhiteSpace(input))
			{
				return input;
			}
			return filters.Aggregate(input, (string current, IFilter f) => f.Filter(current));
		}
	}
}
