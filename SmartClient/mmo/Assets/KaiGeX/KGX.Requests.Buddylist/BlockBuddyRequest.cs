using KaiGeX.Entities;
using KaiGeX.Exceptions;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests.Buddylist
{
	public class BlockBuddyRequest : BaseRequest
	{
		public static readonly string KEY_BUDDY_NAME = "bn";
		public static readonly string KEY_BUDDY_BLOCK_STATE = "bs";
		private string buddyName;
		private bool blocked;
		public BlockBuddyRequest(string buddyName, bool blocked) : base(RequestType.BlockBuddy)
		{
			this.buddyName = buddyName;
			this.blocked = blocked;
		}
		public override void Validate(KaiGeNet sfs)
		{
			List<string> list = new List<string>();
			if (!sfs.BuddyManager.Inited)
			{
				list.Add("BuddyList is not inited. Please send an InitBuddyRequest first.");
			}
			if (this.buddyName == null || this.buddyName.Length < 1)
			{
				list.Add("Invalid buddy name: " + this.buddyName);
			}
			if (!sfs.BuddyManager.MyOnlineState)
			{
				list.Add("Can't block buddy while off-line");
			}
			Buddy buddyByName = sfs.BuddyManager.GetBuddyByName(this.buddyName);
			if (buddyByName == null)
			{
				list.Add("Can't block buddy, it's not in your list: " + this.buddyName);
			}
			else
			{
				if (buddyByName.IsBlocked == this.blocked)
				{
					list.Add(string.Concat(new object[]
					{
						"BuddyBlock flag is already in the requested state: ",
						this.blocked,
						", for buddy: ",
						buddyByName
					}));
				}
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("BuddyList request error", list);
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			this.sfso.PutUtfString(BlockBuddyRequest.KEY_BUDDY_NAME, this.buddyName);
			this.sfso.PutBool(BlockBuddyRequest.KEY_BUDDY_BLOCK_STATE, this.blocked);
		}
	}
}
