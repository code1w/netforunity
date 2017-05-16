﻿using KaiGeX.Exceptions;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests.Buddylist
{
	public class GoOnlineRequest : BaseRequest
	{
		public static readonly string KEY_ONLINE = "o";
		public static readonly string KEY_BUDDY_NAME = "bn";
		public static readonly string KEY_BUDDY_ID = "bi";
		private bool online;
		public GoOnlineRequest(bool online) : base(RequestType.GoOnline)
		{
			this.online = online;
		}
		public override void Validate(KaiGeNet sfs)
		{
			List<string> list = new List<string>();
			if (!sfs.BuddyManager.Inited)
			{
				list.Add("BuddyList is not inited. Please send an InitBuddyRequest first.");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("GoOnline request error", list);
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			sfs.BuddyManager.MyOnlineState = this.online;
			this.sfso.PutBool(GoOnlineRequest.KEY_ONLINE, this.online);
		}
	}
}
