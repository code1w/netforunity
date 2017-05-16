using KaiGeX.Exceptions;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests
{
	public class BanUserRequest : BaseRequest
	{
		public static readonly string KEY_USER_ID = "u";
		public static readonly string KEY_MESSAGE = "m";
		public static readonly string KEY_DELAY = "d";
		public static readonly string KEY_BAN_MODE = "b";
		public static readonly string KEY_BAN_DURATION_HOURS = "dh";
		private int userId;
		private string message;
		private int delay;
		private BanMode banMode;
		private int durationHours;
		private void Init(int userId, string message, BanMode banMode, int delaySeconds, int durationHours)
		{
			this.userId = userId;
			this.message = message;
			this.banMode = banMode;
			this.delay = delaySeconds;
			this.durationHours = durationHours;
		}
		public BanUserRequest(int userId, string message, BanMode banMode, int delaySeconds, int durationHours) : base(RequestType.BanUser)
		{
			this.Init(userId, message, banMode, delaySeconds, durationHours);
		}
		public BanUserRequest(int userId) : base(RequestType.BanUser)
		{
			this.Init(userId, null, BanMode.BY_NAME, 5, 0);
		}
		public BanUserRequest(int userId, string message) : base(RequestType.BanUser)
		{
			this.Init(userId, message, BanMode.BY_NAME, 5, 0);
		}
		public BanUserRequest(int userId, string message, BanMode banMode) : base(RequestType.BanUser)
		{
			this.Init(userId, message, banMode, 5, 0);
		}
		public BanUserRequest(int userId, string message, BanMode banMode, int delaySeconds) : base(RequestType.BanUser)
		{
			this.Init(userId, message, banMode, delaySeconds, 0);
		}
		public override void Validate(KaiGeNet sfs)
		{
			List<string> list = new List<string>();
			if (!sfs.MySelf.IsModerator() && !sfs.MySelf.IsAdmin())
			{
				list.Add("You don't have enough permissions to execute this request.");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("BanUser request error", list);
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			this.sfso.PutInt(BanUserRequest.KEY_USER_ID, this.userId);
			this.sfso.PutInt(BanUserRequest.KEY_DELAY, this.delay);
			this.sfso.PutInt(BanUserRequest.KEY_BAN_MODE, (int)this.banMode);
			this.sfso.PutInt(BanUserRequest.KEY_BAN_DURATION_HOURS, this.durationHours);
			if (this.message != null && this.message.Length > 0)
			{
				this.sfso.PutUtfString(BanUserRequest.KEY_MESSAGE, this.message);
			}
		}
	}
}
