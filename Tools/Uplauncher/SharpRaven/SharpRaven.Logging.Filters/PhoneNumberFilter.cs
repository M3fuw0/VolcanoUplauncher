using System.Text.RegularExpressions;

namespace SharpRaven.Logging.Filters
{
	public class PhoneNumberFilter : IFilter
	{
		private static readonly Regex phoneRegex;

		static PhoneNumberFilter()
		{
			phoneRegex = new Regex("1?\\W*([2-9][0-8]\\d)\\W*([2-9]\\d{2})\\W*(\\d{4})(\\se?x?t?(\\d*))?\\s+", RegexOptions.Compiled);
		}

		public string Filter(string input)
		{
			return phoneRegex.Replace(input, "##-PHONE-TRUNC-##");
		}
	}
}
