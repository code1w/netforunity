using Sfs2X.Core;
using System;
using System.Collections;
namespace Sfs2X.Logging
{
	public class Logger
	{
		private SmartFox smartFox;
		private bool enableConsoleTrace = true;
		private bool enableEventDispatching = true;
		private LogLevel loggingLevel;
		public bool EnableConsoleTrace
		{
			get
			{
				return this.enableConsoleTrace;
			}
			set
			{
				this.enableConsoleTrace = value;
			}
		}
		public bool EnableEventDispatching
		{
			get
			{
				return this.enableEventDispatching;
			}
			set
			{
				this.enableEventDispatching = value;
			}
		}
		public LogLevel LoggingLevel
		{
			get
			{
				return this.loggingLevel;
			}
			set
			{
				this.loggingLevel = value;
			}
		}
		public Logger(SmartFox smartFox)
		{
			this.smartFox = smartFox;
			this.loggingLevel = LogLevel.INFO;
		}
		public void Debug(params string[] messages)
		{
			this.Log(LogLevel.DEBUG, string.Join(" ", messages));
		}
		public void Info(params string[] messages)
		{
			this.Log(LogLevel.INFO, string.Join(" ", messages));
		}
		public void Warn(params string[] messages)
		{
			this.Log(LogLevel.WARN, string.Join(" ", messages));
		}
		public void Error(params string[] messages)
		{
			this.Log(LogLevel.ERROR, string.Join(" ", messages));
		}
		private void Log(LogLevel level, string message)
		{
			if (level >= this.loggingLevel)
			{
				if (this.enableConsoleTrace)
				{
					Console.WriteLine(string.Concat(new object[]
					{
						"[SFS - ",
						level,
						"] ",
						message
					}));
				}
				if (this.enableEventDispatching && this.smartFox != null)
				{
					Hashtable hashtable = new Hashtable();
					hashtable.Add("message", message);
					LoggerEvent evt = new LoggerEvent(this.loggingLevel, hashtable);
					this.smartFox.DispatchEvent(evt);
				}
			}
		}
		public void AddEventListener(LogLevel level, EventListenerDelegate listener)
		{
			if (this.smartFox != null)
			{
				this.smartFox.AddEventListener(LoggerEvent.LogEventType(level), listener);
			}
		}
	}
}
