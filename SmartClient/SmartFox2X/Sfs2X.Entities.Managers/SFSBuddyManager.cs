using Sfs2X.Entities.Variables;
using System;
using System.Collections.Generic;
namespace Sfs2X.Entities.Managers
{
	public class SFSBuddyManager : IBuddyManager
	{
		protected Dictionary<string, Buddy> buddiesByName;
		protected Dictionary<string, BuddyVariable> myVariables;
		protected bool myOnlineState;
		protected bool inited;
		private List<string> buddyStates;
		public bool Inited
		{
			get
			{
				return this.inited;
			}
			set
			{
				this.inited = value;
			}
		}
		public List<Buddy> OfflineBuddies
		{
			get
			{
				List<Buddy> list = new List<Buddy>();
				foreach (Buddy current in this.buddiesByName.Values)
				{
					if (!current.IsOnline)
					{
						list.Add(current);
					}
				}
				return list;
			}
		}
		public List<Buddy> OnlineBuddies
		{
			get
			{
				List<Buddy> list = new List<Buddy>();
				foreach (Buddy current in this.buddiesByName.Values)
				{
					if (current.IsOnline)
					{
						list.Add(current);
					}
				}
				return list;
			}
		}
		public List<Buddy> BuddyList
		{
			get
			{
				return new List<Buddy>(this.buddiesByName.Values);
			}
		}
		public List<BuddyVariable> MyVariables
		{
			get
			{
				return new List<BuddyVariable>(this.myVariables.Values);
			}
			set
			{
				foreach (BuddyVariable current in value)
				{
					this.SetMyVariable(current);
				}
			}
		}
		public bool MyOnlineState
		{
			get
			{
				bool result;
				if (!this.inited)
				{
					result = false;
				}
				else
				{
					bool flag = true;
					BuddyVariable myVariable = this.GetMyVariable(ReservedBuddyVariables.BV_ONLINE);
					if (myVariable != null)
					{
						flag = myVariable.GetBoolValue();
					}
					result = flag;
				}
				return result;
			}
			set
			{
				this.SetMyVariable(new SFSBuddyVariable(ReservedBuddyVariables.BV_ONLINE, value));
			}
		}
		public string MyNickName
		{
			get
			{
				BuddyVariable myVariable = this.GetMyVariable(ReservedBuddyVariables.BV_NICKNAME);
				return (myVariable == null) ? null : myVariable.GetStringValue();
			}
			set
			{
				this.SetMyVariable(new SFSBuddyVariable(ReservedBuddyVariables.BV_NICKNAME, value));
			}
		}
		public string MyState
		{
			get
			{
				BuddyVariable myVariable = this.GetMyVariable(ReservedBuddyVariables.BV_STATE);
				return (myVariable == null) ? null : myVariable.GetStringValue();
			}
			set
			{
				this.SetMyVariable(new SFSBuddyVariable(ReservedBuddyVariables.BV_STATE, value));
			}
		}
		public List<string> BuddyStates
		{
			get
			{
				return this.buddyStates;
			}
			set
			{
				this.buddyStates = value;
			}
		}
		public SFSBuddyManager(SmartFox sfs)
		{
			this.buddiesByName = new Dictionary<string, Buddy>();
			this.myVariables = new Dictionary<string, BuddyVariable>();
			this.inited = false;
		}
		public void AddBuddy(Buddy buddy)
		{
			this.buddiesByName.Add(buddy.Name, buddy);
		}
		public void ClearAll()
		{
			this.buddiesByName.Clear();
		}
		public Buddy RemoveBuddyById(int id)
		{
			Buddy buddyById = this.GetBuddyById(id);
			if (buddyById != null)
			{
				this.buddiesByName.Remove(buddyById.Name);
			}
			return buddyById;
		}
		public Buddy RemoveBuddyByName(string name)
		{
			Buddy buddyByName = this.GetBuddyByName(name);
			if (buddyByName != null)
			{
				this.buddiesByName.Remove(name);
			}
			return buddyByName;
		}
		public Buddy GetBuddyById(int id)
		{
			Buddy result;
			if (id > -1)
			{
				foreach (Buddy current in this.buddiesByName.Values)
				{
					if (current.Id == id)
					{
						result = current;
						return result;
					}
				}
			}
			result = null;
			return result;
		}
		public bool ContainsBuddy(string name)
		{
			return this.buddiesByName.ContainsKey(name);
		}
		public Buddy GetBuddyByName(string name)
		{
			Buddy result;
			if (this.buddiesByName.ContainsKey(name))
			{
				result = this.buddiesByName[name];
			}
			else
			{
				result = null;
			}
			return result;
		}
		public Buddy GetBuddyByNickName(string nickName)
		{
			Buddy result;
			foreach (Buddy current in this.buddiesByName.Values)
			{
				if (current.NickName == nickName)
				{
					result = current;
					return result;
				}
			}
			result = null;
			return result;
		}
		public BuddyVariable GetMyVariable(string varName)
		{
			BuddyVariable result;
			if (this.myVariables.ContainsKey(varName))
			{
				result = this.myVariables[varName];
			}
			else
			{
				result = null;
			}
			return result;
		}
		public void SetMyVariable(BuddyVariable bVar)
		{
			this.myVariables[bVar.Name] = bVar;
		}
	}
}
