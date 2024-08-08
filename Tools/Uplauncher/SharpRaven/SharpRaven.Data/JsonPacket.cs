using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpRaven.Serialization;
using SharpRaven.Utilities;

namespace SharpRaven.Data
{
	public class JsonPacket
	{
		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Culprit { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Environment { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string EventID { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public List<SentryException> Exceptions { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public object Extra { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string[] Fingerprint { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		[JsonConverter(typeof(LowerInvariantStringEnumConverter))]
		public ErrorLevel Level { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Logger { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Message { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public SentryMessage MessageObject { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public IDictionary<string, string> Modules { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Platform { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Project { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string Release { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public ISentryRequest Request { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public string ServerName { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public IDictionary<string, string> Tags { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public DateTime TimeStamp { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public List<Breadcrumb> Breadcrumbs { get; set; }

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		public SentryUser User { get; set; }

		public JsonPacket(string project, Exception exception)
			: this(project)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			Initialize(exception);
		}

		public JsonPacket(string project, SentryEvent @event)
			: this(project)
		{
			if (@event == null)
			{
				throw new ArgumentNullException("event");
			}
			if (@event.Exception != null)
			{
				Initialize(@event.Exception);
			}
			Message = ((@event.Message != null) ? @event.Message.ToString() : null);
			Level = @event.Level;
			Extra = Merge(@event);
			Tags = @event.Tags;
			Fingerprint = @event.Fingerprint.ToArray();
			MessageObject = @event.Message;
		}

		private static object Merge(SentryEvent @event)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			Exception exception = @event.Exception;
			object extra = @event.Extra;
			if (exception == null && extra == null)
			{
				return null;
			}
			if (extra != null && exception == null)
			{
				return extra;
			}
			ExceptionData exceptionData = new ExceptionData(exception);
			if (extra == null)
			{
				return exceptionData;
			}
			JObject val;
			if (extra.GetType().IsArray)
			{
				val = new JObject();
				JArray val2 = JArray.FromObject(extra);
				foreach (JToken item in val2)
				{
					JObject val3 = (JObject)(object)((item is JObject) ? item : null);
					JProperty[] array;
					if (val3 == null || (array = val3.Properties().ToArray()).Length != 2 || array[0].Name != "Key" || array[1].Name != "Value")
					{
						((JContainer)val).Merge((object)item);
						continue;
					}
					string text = ((object)array[0].Value).ToString();
					JToken value = array[1].Value;
					val.Add(text, value);
				}
			}
			else
			{
				val = JObject.FromObject(extra);
			}
			JObject val4 = JObject.FromObject((object)exceptionData);
			((JContainer)val).Merge((object)val4);
			return val;
		}

		public JsonPacket(string project)
			: this()
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}
			Project = project;
		}

		private JsonPacket()
		{
			Modules = SystemUtil.GetModules();
			ServerName = System.Environment.MachineName;
			TimeStamp = DateTime.UtcNow;
			Logger = "root";
			Level = ErrorLevel.Error;
			EventID = Guid.NewGuid().ToString("n");
			Project = "default";
			Platform = "csharp";
			Release = "";
			Environment = "";
		}

		public virtual string ToString(Formatting formatting)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return JsonConvert.SerializeObject((object)this, formatting);
		}

		public override string ToString()
		{
			return JsonConvert.SerializeObject((object)this);
		}

		private void Initialize(Exception exception)
		{
			Message = exception.Message;
			if (exception.TargetSite != null)
			{
				Culprit = string.Format("{0} in {1}", (exception.TargetSite.ReflectedType == null) ? "<dynamic type>" : exception.TargetSite.ReflectedType.FullName, exception.TargetSite.Name);
			}
			Exceptions = new List<SentryException>();
			for (Exception ex = exception; ex != null; ex = ex.InnerException)
			{
				SentryException ex2 = new SentryException(ex);
				ex2.Module = ex.Source;
				ex2.Type = ex.GetType().Name;
				ex2.Value = ex.Message;
				SentryException item = ex2;
				Exceptions.Add(item);
			}
			if (exception is ReflectionTypeLoadException ex3)
			{
				Exception[] loaderExceptions = ex3.LoaderExceptions;
				foreach (Exception exception2 in loaderExceptions)
				{
					SentryException item2 = new SentryException(exception2);
					Exceptions.Add(item2);
				}
			}
		}
	}
}
