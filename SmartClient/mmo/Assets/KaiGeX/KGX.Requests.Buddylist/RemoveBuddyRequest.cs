using KaiGeX.Exceptions;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests.Buddylist
{
	public class RemoveBuddyRequest : BaseRequest
	{
		public static readonly string KEY_BUDDY_NAME = "bn";
		private string name;
		public RemoveBuddyRequest(string buddyName) : base(RequestType.RemoveBuddy)
		{
			this.name = buddyName;
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
				list.Add("Can't remove buddy while off-line");
			}
			if (!sfs.BuddyManager.ContainsBuddy(this.name))
			{
				list.Add("Can't remove buddy, it's not in your list: " + this.name);
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("BuddyList request error", list);
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			this.sfso.PutUtfString(RemoveBuddyRequest.KEY_BUDDY_NAME, this.name);
		}
	}
}
