using KaiGeX.Exceptions;
using System;
namespace KaiGeX.Requests
{
	public class LogoutRequest : BaseRequest
	{
		public static readonly string KEY_ZONE_NAME = "zn";
		public LogoutRequest() : base(RequestType.Logout)
		{
		}
		public override void Validate(KaiGeNet sfs)
		{
			if (sfs.MySelf == null)
			{
				throw new SFSValidationError("LogoutRequest Error", new string[]
				{
					"You are not logged in at the moment!"
				});
			}
		}
	}
}
