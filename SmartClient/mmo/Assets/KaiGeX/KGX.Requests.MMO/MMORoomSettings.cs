using KaiGeX.Entities.Data;
using System;
namespace KaiGeX.Requests.MMO
{
	public class MMORoomSettings : RoomSettings
	{
		private Vec3D defaultAOI;
		private MapLimits mapLimits;
		private int userMaxLimboSeconds = 50;
		private int proximityListUpdateMillis = 250;
		private bool sendAOIEntryPoint = true;
		public Vec3D DefaultAOI
		{
			get
			{
				return this.defaultAOI;
			}
			set
			{
				this.defaultAOI = value;
			}
		}
		public MapLimits MapLimits
		{
			get
			{
				return this.mapLimits;
			}
			set
			{
				this.mapLimits = value;
			}
		}
		public int UserMaxLimboSeconds
		{
			get
			{
				return this.userMaxLimboSeconds;
			}
			set
			{
				this.userMaxLimboSeconds = value;
			}
		}
		public int ProximityListUpdateMillis
		{
			get
			{
				return this.proximityListUpdateMillis;
			}
			set
			{
				this.proximityListUpdateMillis = value;
			}
		}
		public bool SendAOIEntryPoint
		{
			get
			{
				return this.sendAOIEntryPoint;
			}
			set
			{
				this.sendAOIEntryPoint = value;
			}
		}
		public MMORoomSettings(string name) : base(name)
		{
		}
	}
}
