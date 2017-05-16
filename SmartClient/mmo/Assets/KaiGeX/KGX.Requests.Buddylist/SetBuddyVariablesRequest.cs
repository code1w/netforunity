using KaiGeX.Entities.Data;
using KaiGeX.Entities.Variables;
using KaiGeX.Exceptions;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests.Buddylist
{
	public class SetBuddyVariablesRequest : BaseRequest
	{
		public static readonly string KEY_BUDDY_NAME = "bn";
		public static readonly string KEY_BUDDY_VARS = "bv";
		private List<BuddyVariable> buddyVariables;
		public SetBuddyVariablesRequest(List<BuddyVariable> buddyVariables) : base(RequestType.SetBuddyVariables)
		{
			this.buddyVariables = buddyVariables;
		}
		public override void Validate(KaiGeNet sfs)
		{
			List<string> list = new List<string>();
			if (!sfs.BuddyManager.Inited)
			{
				list.Add("BuddyList is not inited. Please send an InitBuddyRequest first.");
			}
			if (!sfs.BuddyManager.MyOnlineState)
			{
				list.Add("Can't set buddy variables while off-line");
			}
			if (this.buddyVariables == null || this.buddyVariables.Count == 0)
			{
				list.Add("No variables were specified");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("SetBuddyVariables request error", list);
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			ISFSArray iSFSArray = SFSArray.NewInstance();
			foreach (BuddyVariable current in this.buddyVariables)
			{
				iSFSArray.AddSFSArray(current.ToSFSArray());
			}
			this.sfso.PutSFSArray(SetBuddyVariablesRequest.KEY_BUDDY_VARS, iSFSArray);
		}
	}
}
