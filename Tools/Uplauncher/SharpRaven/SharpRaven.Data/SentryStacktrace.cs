using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SharpRaven.Data
{
	public class SentryStacktrace
	{
		[JsonProperty(PropertyName = "frames")]
		public ExceptionFrame[] Frames { get; set; }

		public SentryStacktrace(Exception exception)
		{
			if (exception == null)
			{
				return;
			}
			StackTrace stackTrace = new StackTrace(exception, fNeedFileInfo: true);
			StackFrame[] frames = stackTrace.GetFrames();
			if (frames != null)
			{
				Frames = (from f in frames.Reverse()
					select new ExceptionFrame(f)).ToArray();
			}
		}

		public override string ToString()
		{
			if (Frames == null || !Frames.Any())
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ExceptionFrame item in Frames.Reverse())
			{
				stringBuilder.Append("   at ");
				stringBuilder.Append(item);
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}
	}
}
