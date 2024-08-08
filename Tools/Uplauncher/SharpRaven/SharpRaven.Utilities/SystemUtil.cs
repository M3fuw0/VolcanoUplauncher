using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SharpRaven.Utilities
{
	public static class SystemUtil
	{
		public static bool IsNullOrWhiteSpace(string arg)
		{
			if (!string.IsNullOrEmpty(arg))
			{
				return string.IsNullOrEmpty(arg.Trim());
			}
			return true;
		}

		public static IDictionary<string, string> GetModules()
		{
			IOrderedEnumerable<AssemblyName> orderedEnumerable = from q in AppDomain.CurrentDomain.GetAssemblies()
				where !q.IsDynamic
				select q into a
				select a.GetName() into a
				orderby a.Name
				select a;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (AssemblyName item in orderedEnumerable)
			{
				if (!dictionary.ContainsKey(item.Name))
				{
					dictionary.Add(item.Name, item.Version.ToString());
				}
			}
			return dictionary;
		}

		public static void WriteError(Exception exception)
		{
			if (exception != null)
			{
				WriteError(exception.ToString());
			}
		}

		public static void WriteError(string error)
		{
			if (error != null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("[ERROR] ");
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.WriteLine(error);
			}
		}

		public static void WriteError(string description, string multilineData)
		{
			if (multilineData == null)
			{
				return;
			}
			string[] array = multilineData.Split(new char[2] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length > 0)
			{
				WriteError(description);
				string[] array2 = array;
				foreach (string error in array2)
				{
					WriteError(error);
				}
			}
		}

		public static void CopyTo(this Stream input, Stream output)
		{
			byte[] array = new byte[16384];
			int count;
			while ((count = input.Read(array, 0, array.Length)) > 0)
			{
				output.Write(array, 0, count);
			}
		}
	}
}
