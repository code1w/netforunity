using System;
namespace Sfs2X.Requests
{
	public class RoomExtension
	{
		private string id;
		private string className;
		private string propertiesFile;
		public string Id
		{
			get
			{
				return this.id;
			}
		}
		public string ClassName
		{
			get
			{
				return this.className;
			}
		}
		public string PropertiesFile
		{
			get
			{
				return this.propertiesFile;
			}
			set
			{
				this.propertiesFile = value;
			}
		}
		public RoomExtension(string id, string className)
		{
			this.id = id;
			this.className = className;
			this.propertiesFile = "";
		}
	}
}
