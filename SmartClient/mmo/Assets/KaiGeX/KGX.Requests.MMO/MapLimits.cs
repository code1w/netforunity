using KaiGeX.Entities.Data;
using System;
namespace KaiGeX.Requests.MMO
{
	public class MapLimits
	{
		private Vec3D lowerLimit;
		private Vec3D higherLimit;
		public Vec3D LowerLimit
		{
			get
			{
				return this.lowerLimit;
			}
		}
		public Vec3D HigherLimit
		{
			get
			{
				return this.higherLimit;
			}
		}
		public MapLimits(Vec3D lowerLimit, Vec3D higherLimit)
		{
			if (lowerLimit == null || higherLimit == null)
			{
				throw new ArgumentException("Map limits arguments must be both non null!");
			}
			this.lowerLimit = lowerLimit;
			this.higherLimit = higherLimit;
		}
	}
}
