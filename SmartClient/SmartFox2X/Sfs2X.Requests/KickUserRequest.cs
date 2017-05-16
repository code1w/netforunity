using Sfs2X.Exceptions;
using System;
using System.Collections.Generic;
namespace Sfs2X.Requests
{
	public class KickUserRequest : BaseRequest
	{
		public static readonly string KEY_USER_ID = "u";
		public static readonly string KEY_MESSAGE = "m";
		public static readonly string KEY_DELAY = "d";
		private int userId;
		private string message;
		private int delay;
		private void Init(int userId, string message, int delaySeconds)
		{
			this.userId = userId;
			this.message = message;
			this.delay = delaySeconds;
			if (this.delay < 0)
			{
				this.delay = 0;
			}
		}
		public KickUserRequest(int userId, string message, int delaySeconds) : base(RequestType.BanUser)
		{
			this.Init(userId, message, delaySeconds);
		}
		public KickUserRequest(int userId) : base(RequestType.KickUser)
		{
			this.Init(userId, null, 5);
		}
		public KickUserRequest(int userId, string message) : base(RequestType.KickUser)
		{
			this.Init(userId, message, 5);
		}
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (!sfs.MySelf.IsModerator() && !sfs.MySelf.IsAdmin())
			{
				list.Add("You don't have enough permissions to execute this request.");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("KickUser request error", list);
			}
		}
		public override void Execute(SmartFox sfs)
		{
			this.sfso.PutInt(KickUserRequest.KEY_USER_ID, this.userId);
			this.sfso.PutInt(KickUserRequest.KEY_DELAY, this.delay);
			if (this.message != null && this.message.Length > 0)
			{
				this.sfso.PutUtfString(KickUserRequest.KEY_MESSAGE, this.message);
			}
		}
	}
}
