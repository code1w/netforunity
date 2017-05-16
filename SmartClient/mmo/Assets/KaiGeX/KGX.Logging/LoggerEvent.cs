using KaiGeX.Core;
using System;
using System.Collections;
namespace KaiGeX.Logging
{
	public class LoggerEvent : BaseEvent, ICloneable
	{
		private LogLevel level;
		public LoggerEvent(LogLevel level, Hashtable parameters) : base(LoggerEvent.LogEventType(level), parameters)
		{
			this.level = level;
		}
		public static string LogEventType(LogLevel level)
		{
			return "LOG_" + level.ToString();
		}
		public override string ToString()
		{
			return string.Format("LoggerEvent " + this.type, new object[0]);
		}
		public new object Clone()
		{
			return new LoggerEvent(this.level, this.arguments);
		}
	}
}
