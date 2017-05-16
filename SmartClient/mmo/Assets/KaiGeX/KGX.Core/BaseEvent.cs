using System;
using System.Collections;
namespace KaiGeX.Core
{
	public class BaseEvent
	{
		protected Hashtable arguments;
		protected string type;
		protected object target;
		public string Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}
		public IDictionary Params
		{
			get
			{
				return this.arguments;
			}
			set
			{
				this.arguments = (value as Hashtable);
			}
		}
		public object Target
		{
			get
			{
				return this.target;
			}
			set
			{
				this.target = value;
			}
		}
		public override string ToString()
		{
			return this.type + " [ " + ((this.target == null) ? "null" : this.target.ToString()) + "]";
		}
		public BaseEvent Clone()
		{
			return new BaseEvent(this.type, this.arguments);
		}
		public BaseEvent(string type)
		{
			this.Type = type;
			if (this.arguments == null)
			{
				this.arguments = new Hashtable();
			}
		}
		public BaseEvent(string type, Hashtable args)
		{
			this.Type = type;
			this.arguments = args;
			if (this.arguments == null)
			{
				this.arguments = new Hashtable();
			}
		}
	}
}
