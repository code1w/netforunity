using System;
using System.Collections;
namespace KaiGeX.Core
{
	public class EventDispatcher
	{
		private object target;
		private Hashtable listeners = new Hashtable();
		public EventDispatcher(object target)
		{
			this.target = target;
		}
		public void AddEventListener(string eventType, EventListenerDelegate listener)
		{
            EventListenerDelegate eventListenerDelegate = this.listeners[eventType] as EventListenerDelegate;
            eventListenerDelegate = (EventListenerDelegate)Delegate.Combine(eventListenerDelegate, listener);
            this.listeners[eventType] = eventListenerDelegate;
		}
		public void RemoveEventListener(string eventType, EventListenerDelegate listener)
		{
			EventListenerDelegate eventListenerDelegate = this.listeners[eventType] as EventListenerDelegate;
			if (eventListenerDelegate != null)
			{
				eventListenerDelegate = (EventListenerDelegate)Delegate.Remove(eventListenerDelegate, listener);
			}
			this.listeners[eventType] = eventListenerDelegate;
		}
		public void DispatchEvent(BaseEvent evt)
		{
			EventListenerDelegate eventListenerDelegate = this.listeners[evt.Type] as EventListenerDelegate;
			if (eventListenerDelegate != null)
			{
				evt.Target = this.target;
				try
				{
					eventListenerDelegate(evt);
				}
				catch (Exception ex)
				{
					throw new Exception(string.Concat(new string[]
					{
						"Error dispatching event ",
						evt.Type,
						": ",
						ex.Message,
						" ",
						ex.StackTrace
					}), ex);
				}
			}
		}
		public void RemoveAll()
		{
			this.listeners.Clear();
		}
	}
}
