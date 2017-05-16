using System;
namespace KaiGeX.Requests
{
	public class RoomEvents
	{
		private bool allowUserEnter;
		private bool allowUserExit;
		private bool allowUserCountChange;
		private bool allowUserVariablesUpdate;
		public bool AllowUserEnter
		{
			get
			{
				return this.allowUserEnter;
			}
			set
			{
				this.allowUserEnter = value;
			}
		}
		public bool AllowUserExit
		{
			get
			{
				return this.allowUserExit;
			}
			set
			{
				this.allowUserExit = value;
			}
		}
		public bool AllowUserCountChange
		{
			get
			{
				return this.allowUserCountChange;
			}
			set
			{
				this.allowUserCountChange = value;
			}
		}
		public bool AllowUserVariablesUpdate
		{
			get
			{
				return this.allowUserVariablesUpdate;
			}
			set
			{
				this.allowUserVariablesUpdate = value;
			}
		}
		public RoomEvents()
		{
			this.allowUserEnter = false;
			this.allowUserExit = false;
			this.allowUserCountChange = false;
			this.allowUserVariablesUpdate = false;
		}
	}
}
