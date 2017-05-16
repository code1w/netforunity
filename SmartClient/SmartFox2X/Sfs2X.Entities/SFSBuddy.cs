using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using System;
using System.Collections.Generic;
namespace Sfs2X.Entities
{
	public class SFSBuddy : Buddy
	{
		protected string name;
		protected int id;
		protected bool isBlocked;
		protected Dictionary<string, BuddyVariable> variables = new Dictionary<string, BuddyVariable>();
		protected bool isTemp;
		public int Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}
		public bool IsBlocked
		{
			get
			{
				return this.isBlocked;
			}
			set
			{
				this.isBlocked = value;
			}
		}
		public bool IsTemp
		{
			get
			{
				return this.isTemp;
			}
		}
		public string Name
		{
			get
			{
				return this.name;
			}
		}
		public bool IsOnline
		{
			get
			{
				BuddyVariable variable = this.GetVariable(ReservedBuddyVariables.BV_ONLINE);
				bool flag = variable == null || variable.GetBoolValue();
				return flag && this.id > -1;
			}
		}
		public string State
		{
			get
			{
				BuddyVariable variable = this.GetVariable(ReservedBuddyVariables.BV_STATE);
				return (variable != null) ? variable.GetStringValue() : null;
			}
		}
		public string NickName
		{
			get
			{
				BuddyVariable variable = this.GetVariable(ReservedBuddyVariables.BV_NICKNAME);
				return (variable != null) ? variable.GetStringValue() : null;
			}
		}
		public List<BuddyVariable> Variables
		{
			get
			{
				return new List<BuddyVariable>(this.variables.Values);
			}
		}
		public static Buddy FromSFSArray(ISFSArray arr)
		{
			Buddy buddy = new SFSBuddy(arr.GetInt(0), arr.GetUtfString(1), arr.GetBool(2), arr.Size() > 4 && arr.GetBool(4));
			ISFSArray sFSArray = arr.GetSFSArray(3);
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				BuddyVariable variable = SFSBuddyVariable.FromSFSArray(sFSArray.GetSFSArray(i));
				buddy.SetVariable(variable);
			}
			return buddy;
		}
		public SFSBuddy(int id, string name) : this(id, name, false, false)
		{
		}
		public SFSBuddy(int id, string name, bool isBlocked) : this(id, name, isBlocked, false)
		{
		}
		public SFSBuddy(int id, string name, bool isBlocked, bool isTemp)
		{
			this.id = id;
			this.name = name;
			this.isBlocked = isBlocked;
			this.variables = new Dictionary<string, BuddyVariable>();
			this.isTemp = isTemp;
		}
		public BuddyVariable GetVariable(string varName)
		{
			BuddyVariable result;
			if (this.variables.ContainsKey(varName))
			{
				result = this.variables[varName];
			}
			else
			{
				result = null;
			}
			return result;
		}
		public List<BuddyVariable> GetOfflineVariables()
		{
			List<BuddyVariable> list = new List<BuddyVariable>();
			foreach (BuddyVariable current in this.variables.Values)
			{
				if (current.Name[0] == Convert.ToChar(SFSBuddyVariable.OFFLINE_PREFIX))
				{
					list.Add(current);
				}
			}
			return list;
		}
		public List<BuddyVariable> GetOnlineVariables()
		{
			List<BuddyVariable> list = new List<BuddyVariable>();
			foreach (BuddyVariable current in this.variables.Values)
			{
				if (current.Name[0] != Convert.ToChar(SFSBuddyVariable.OFFLINE_PREFIX))
				{
					list.Add(current);
				}
			}
			return list;
		}
		public bool ContainsVariable(string varName)
		{
			return this.variables.ContainsKey(varName);
		}
		public void SetVariable(BuddyVariable bVar)
		{
			this.variables[bVar.Name] = bVar;
		}
		public void SetVariables(ICollection<BuddyVariable> variables)
		{
			foreach (BuddyVariable current in variables)
			{
				this.SetVariable(current);
			}
		}
		public void RemoveVariable(string varName)
		{
			this.variables.Remove(varName);
		}
		public void ClearVolatileVariables()
		{
			List<string> list = new List<string>();
			foreach (BuddyVariable current in this.variables.Values)
			{
				if (current.Name[0] != Convert.ToChar(SFSBuddyVariable.OFFLINE_PREFIX))
				{
					list.Add(current.Name);
				}
			}
			foreach (string current2 in list)
			{
				this.RemoveVariable(current2);
			}
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[Buddy: ",
				this.name,
				", id: ",
				this.id,
				"]"
			});
		}
	}
}
