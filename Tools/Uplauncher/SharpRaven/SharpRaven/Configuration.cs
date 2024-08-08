using System.Configuration;

namespace SharpRaven
{
	public class Configuration : ConfigurationSection
	{
		public class DsnElement : ConfigurationElement
		{
			[ConfigurationProperty("value")]
			public string Value
			{
				get
				{
					return (string)base["value"];
				}
				set
				{
					base["value"] = value;
				}
			}
		}

		private const string DsnKey = "dsn";

		private static readonly Configuration settings = ConfigurationManager.GetSection("sharpRaven") as Configuration;

		[ConfigurationProperty("dsn", IsKey = true)]
		public DsnElement Dsn => (DsnElement)base["dsn"];

		public static Configuration Settings => settings;
	}
}
