using KaiGeX.Entities.Data;
using KaiGeX.Entities.Variables;
using System;
using System.Collections.Generic;
namespace KaiGeX.Entities
{
	public class MMOItem : IMMOItem
	{
		private int id;
		private Vec3D aoiEntryPoint;
		private Dictionary<string, IMMOItemVariable> variables = new Dictionary<string, IMMOItemVariable>();
		public int Id
		{
			get
			{
				return this.id;
			}
		}
		public Vec3D AOIEntryPoint
		{
			get
			{
				return this.aoiEntryPoint;
			}
			set
			{
				this.aoiEntryPoint = value;
			}
		}
		public static IMMOItem FromSFSArray(ISFSArray encodedItem)
		{
			IMMOItem iMMOItem = new MMOItem(encodedItem.GetInt(0));
			ISFSArray sFSArray = encodedItem.GetSFSArray(1);
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				iMMOItem.SetVariable(MMOItemVariable.FromSFSArray(sFSArray.GetSFSArray(i)));
			}
			return iMMOItem;
		}
		public MMOItem(int id)
		{
			this.id = id;
		}
		public List<IMMOItemVariable> GetVariables()
		{
			return new List<IMMOItemVariable>(this.variables.Values);
		}
		public IMMOItemVariable GetVariable(string name)
		{
			return this.variables[name];
		}
		public void SetVariable(IMMOItemVariable variable)
		{
			if (variable.IsNull())
			{
				this.variables.Remove(variable.Name);
			}
			else
			{
				this.variables[variable.Name] = variable;
			}
		}
		public void SetVariables(List<IMMOItemVariable> variables)
		{
			foreach (IMMOItemVariable current in variables)
			{
				this.SetVariable(current);
			}
		}
		public bool ContainsVariable(string name)
		{
			return this.variables.ContainsKey(name);
		}
	}
}
