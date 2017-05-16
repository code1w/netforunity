using System;
namespace KaiGeX.Entities.Data
{
	public class Vec3D
	{
		private float fx;
		private float fy;
		private float fz;
		private int ix;
		private int iy;
		private int iz;
		private bool useFloat;
		public float FloatX
		{
			get
			{
				return this.fx;
			}
		}
		public float FloatY
		{
			get
			{
				return this.fy;
			}
		}
		public float FloatZ
		{
			get
			{
				return this.fz;
			}
		}
		public int IntX
		{
			get
			{
				return this.ix;
			}
		}
		public int IntY
		{
			get
			{
				return this.iy;
			}
		}
		public int IntZ
		{
			get
			{
				return this.iz;
			}
		}
		public static Vec3D fromArray(object array)
		{
			Vec3D result;
			if (array is int[])
			{
				result = Vec3D.fromIntArray((int[])array);
			}
			else
			{
				if (!(array is float[]))
				{
					throw new ArgumentException("Invalid Array Type, cannot convert to Vec3D!");
				}
				result = Vec3D.fromFloatArray((float[])array);
			}
			return result;
		}
		private static Vec3D fromIntArray(int[] array)
		{
			if (array.Length != 3)
			{
				throw new ArgumentException("Wrong array size. Vec3D requires an array with 3 parameters (x,y,z)");
			}
			return new Vec3D(array[0], array[1], array[2]);
		}
		private static Vec3D fromFloatArray(float[] array)
		{
			if (array.Length != 3)
			{
				throw new ArgumentException("Wrong array size. Vec3D requires an array with 3 parameters (x,y,z)");
			}
			return new Vec3D(array[0], array[1], array[2]);
		}
		public Vec3D(int px, int py, int pz)
		{
			this.ix = px;
			this.iy = py;
			this.iz = pz;
			this.useFloat = false;
		}
		public Vec3D(int px, int py) : this(px, py, 0)
		{
		}
		public Vec3D(float px, float py, float pz)
		{
			this.fx = px;
			this.fy = py;
			this.fz = pz;
			this.useFloat = true;
		}
		public Vec3D(float px, float py) : this(px, py, 0f)
		{
		}
		public bool IsFloat()
		{
			return this.useFloat;
		}
		public int[] ToIntArray()
		{
			return new int[]
			{
				this.ix,
				this.iy,
				this.iz
			};
		}
		public float[] ToFloatArray()
		{
			return new float[]
			{
				this.fx,
				this.fy,
				this.fz
			};
		}
		public override string ToString()
		{
			string result;
			if (this.IsFloat())
			{
				result = string.Format("({0},{1},{2})", this.fx, this.fy, this.fz);
			}
			else
			{
				result = string.Format("({0},{1},{2})", this.ix, this.iy, this.iz);
			}
			return result;
		}
	}
}
