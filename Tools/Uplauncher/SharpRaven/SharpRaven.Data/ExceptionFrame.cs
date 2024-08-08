using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SharpRaven.Data
{
	public class ExceptionFrame
	{
		[JsonProperty(PropertyName = "abs_path")]
		public string AbsolutePath { get; set; }

		[JsonProperty(PropertyName = "colno")]
		public int ColumnNumber { get; set; }

		[JsonProperty(PropertyName = "filename")]
		public string Filename { get; set; }

		[JsonProperty(PropertyName = "function")]
		public string Function { get; private set; }

		[JsonProperty(PropertyName = "in_app")]
		public bool InApp { get; set; }

		[JsonProperty(PropertyName = "lineno")]
		public int LineNumber { get; set; }

		[JsonProperty(PropertyName = "module")]
		public string Module { get; private set; }

		[JsonProperty(PropertyName = "post_context")]
		public List<string> PostContext { get; set; }

		[JsonProperty(PropertyName = "pre_context")]
		public List<string> PreContext { get; set; }

		[JsonProperty(PropertyName = "context_line")]
		public string Source { get; set; }

		[JsonProperty(PropertyName = "vars")]
		public Dictionary<string, string> Vars { get; set; }

		public ExceptionFrame(StackFrame frame)
		{
			if (frame != null)
			{
				int num = frame.GetFileLineNumber();
				if (num == 0)
				{
					num = frame.GetILOffset();
				}
				MethodBase method = frame.GetMethod();
				if (method != null)
				{
					Module = ((method.DeclaringType != null) ? method.DeclaringType.FullName : null);
					Function = method.Name;
					Source = method.ToString();
				}
				else
				{
					Module = "(unknown)";
					Function = "(unknown)";
					Source = "(unknown)";
				}
				Filename = frame.GetFileName();
				LineNumber = num;
				ColumnNumber = frame.GetFileColumnNumber();
				InApp = !IsSystemModuleName(Module);
				DemangleAsyncFunctionName();
				DemangleAnonymousFunction();
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (Module != null)
			{
				stringBuilder.Append(Module);
				stringBuilder.Append('.');
			}
			if (Function != null)
			{
				stringBuilder.Append(Function);
				stringBuilder.Append("()");
			}
			if (Filename != null)
			{
				stringBuilder.Append(" in ");
				stringBuilder.Append(Filename);
			}
			if (LineNumber > -1)
			{
				stringBuilder.Append(":line ");
				stringBuilder.Append(LineNumber);
			}
			return stringBuilder.ToString();
		}

		private static bool IsSystemModuleName(string moduleName)
		{
			if (!string.IsNullOrEmpty(moduleName))
			{
				if (!moduleName.StartsWith("System.", StringComparison.Ordinal))
				{
					return moduleName.StartsWith("Microsoft.", StringComparison.Ordinal);
				}
				return true;
			}
			return false;
		}

		private void DemangleAsyncFunctionName()
		{
			if (Module != null && !(Function != "MoveNext"))
			{
				string pattern = "^(.*)\\+<(\\w*)>d__\\d*$";
				Match match = Regex.Match(Module, pattern);
				if (match.Success && match.Groups.Count == 3)
				{
					Module = match.Groups[1].Value;
					Function = match.Groups[2].Value;
				}
			}
		}

		private void DemangleAnonymousFunction()
		{
			if (Function != null)
			{
				string pattern = "^<(\\w*)>b__\\w+$";
				Match match = Regex.Match(Function, pattern);
				if (match.Success && match.Groups.Count == 2)
				{
					Function = match.Groups[1].Value + " { <lambda> }";
				}
			}
		}
	}
}
