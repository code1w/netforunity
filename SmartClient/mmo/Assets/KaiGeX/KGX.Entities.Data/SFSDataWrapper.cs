using System;
namespace KaiGeX.Entities.Data
{
	public class SFSDataWrapper
	{
		private int type;
		private object data;
		public int Type
		{
			get
			{
				return this.type;
			}
		}
		public object Data
		{
			get
			{
				return this.data;
			}
		}
		public SFSDataWrapper(int type, object data)
		{
			this.type = type;
			this.data = data;
		}
		public SFSDataWrapper(SFSDataType tp, object data)
		{
			this.type = (int)tp;
			this.data = data;
		}
	}
}
