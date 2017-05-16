using KaiGeX.Entities.Data;
using System;
using System.Collections.Generic;
namespace KaiGeX.Entities
{
	public class MMORoom : SFSRoom
	{
		private Vec3D defaultAOI;
		private Vec3D lowerMapLimit;
		private Vec3D higherMapLimit;
		private Dictionary<int, IMMOItem> itemsById = new Dictionary<int, IMMOItem>();
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
		public Vec3D LowerMapLimit
		{
			get
			{
				return this.lowerMapLimit;
			}
			set
			{
				this.lowerMapLimit = value;
			}
		}
		public Vec3D HigherMapLimit
		{
			get
			{
				return this.higherMapLimit;
			}
			set
			{
				this.higherMapLimit = value;
			}
		}
		public MMORoom(int id, string name, string groupId) : base(id, name, groupId)
		{
		}
		public MMORoom(int id, string name) : base(id, name)
		{
		}
		public IMMOItem GetMMOItem(int id)
		{
			return this.itemsById[id];
		}
		public List<IMMOItem> GetMMOItems()
		{
			return new List<IMMOItem>(this.itemsById.Values);
		}
		public void AddMMOItem(IMMOItem item)
		{
			this.itemsById.Add(item.Id, item);
		}
		public void RemoveItem(int id)
		{
			this.itemsById.Remove(id);
		}
	}
}
