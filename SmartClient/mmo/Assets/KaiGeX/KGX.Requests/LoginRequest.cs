using KaiGeX.Entities.Data;
using KaiGeX.Exceptions;
using KaiGeX.Util;
using System;
namespace KaiGeX.Requests
{
	public class LoginRequest : BaseRequest
	{
		public static readonly string KEY_ZONE_NAME = "zn";
		public static readonly string KEY_USER_NAME = "un";
		public static readonly string KEY_PASSWORD = "pw";
		public static readonly string KEY_PARAMS = "p";
		public static readonly string KEY_PRIVILEGE_ID = "pi";
		public static readonly string KEY_ID = "id";
		public static readonly string KEY_ROOMLIST = "rl";
		public static readonly string KEY_RECONNECTION_SECONDS = "rs";
		private string zoneName;
		private string userName;
		private string password;
		private ISFSObject parameters;
		private void Init(string userName, string password, string zoneName, ISFSObject parameters)
		{
			this.userName = userName;
			this.password = ((password != null) ? password : "");
			this.zoneName = zoneName;
			this.parameters = parameters;
		}
		public LoginRequest(string userName, string password, string zoneName, ISFSObject parameters) : base(RequestType.Login)
		{
			this.Init(userName, password, zoneName, parameters);
		}
		public LoginRequest(string userName, string password, string zoneName) : base(RequestType.Login)
		{
			this.Init(userName, password, zoneName, null);
		}
		public LoginRequest(string userName, string password) : base(RequestType.Login)
		{
			this.Init(userName, password, null, null);
		}
		public LoginRequest(string userName) : base(RequestType.Login)
		{
			this.Init(userName, null, null, null);
		}
		public override void Execute(KaiGeNet sfs)
		{
			this.sfso.PutUtfString(LoginRequest.KEY_ZONE_NAME, this.zoneName);
			this.sfso.PutUtfString(LoginRequest.KEY_USER_NAME, this.userName);
			if (this.password.Length > 0)
			{
				this.password = PasswordUtil.MD5Password(sfs.SessionToken + this.password);
			}
			this.sfso.PutUtfString(LoginRequest.KEY_PASSWORD, this.password);
			if (this.parameters != null)
			{
				this.sfso.PutSFSObject(LoginRequest.KEY_PARAMS, this.parameters);
			}
		}
		public override void Validate(KaiGeNet sfs)
		{
			if (sfs.MySelf != null)
			{
				throw new SFSValidationError("LoginRequest Error", new string[]
				{
					"You are already logged in. Logout first"
				});
			}
			if ((this.zoneName == null || this.zoneName.Length == 0) && sfs.Config != null)
			{
				this.zoneName = sfs.Config.Zone;
			}
			if (this.zoneName == null || this.zoneName.Length == 0)
			{
				throw new SFSValidationError("LoginRequest Error", new string[]
				{
					"Missing Zone name"
				});
			}
		}
	}
}
