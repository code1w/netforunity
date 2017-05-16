using KaiGeX.Entities;
using KaiGeX.Entities.Match;
using KaiGeX.Exceptions;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests
{
	public class FindUsersRequest : BaseRequest
	{
		public static readonly string KEY_EXPRESSION = "e";
		public static readonly string KEY_GROUP = "g";
		public static readonly string KEY_ROOM = "r";
		public static readonly string KEY_LIMIT = "l";
		public static readonly string KEY_FILTERED_USERS = "fu";
		private MatchExpression matchExpr;
		private object target;
		private int limit;
		private void Init(MatchExpression expr, object target, int limit)
		{
			this.matchExpr = expr;
			this.target = target;
			this.limit = limit;
		}
		public FindUsersRequest(MatchExpression expr, string target, int limit) : base(RequestType.FindUsers)
		{
			this.Init(expr, target, limit);
		}
		public FindUsersRequest(MatchExpression expr) : base(RequestType.FindUsers)
		{
			this.Init(expr, null, 0);
		}
		public FindUsersRequest(MatchExpression expr, Room target) : base(RequestType.FindUsers)
		{
			this.Init(expr, target, 0);
		}
		public FindUsersRequest(MatchExpression expr, Room target, int limit) : base(RequestType.FindUsers)
		{
			this.Init(expr, target, limit);
		}
		public FindUsersRequest(MatchExpression expr, string target) : base(RequestType.FindUsers)
		{
			this.Init(expr, target, 0);
		}
		public override void Validate(KaiGeNet sfs)
		{
			List<string> list = new List<string>();
			if (this.matchExpr == null)
			{
				list.Add("Missing Match Expression");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("FindUsers request error", list);
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			this.sfso.PutSFSArray(FindUsersRequest.KEY_EXPRESSION, this.matchExpr.ToSFSArray());
			if (this.target != null)
			{
				if (this.target is Room)
				{
					this.sfso.PutInt(FindUsersRequest.KEY_ROOM, (this.target as Room).Id);
				}
				else
				{
					if (this.target is string)
					{
						this.sfso.PutUtfString(FindUsersRequest.KEY_GROUP, this.target as string);
					}
					else
					{
						sfs.Log.Warn(new string[]
						{
							"Unsupport target type for FindUsersRequest: " + this.target
						});
					}
				}
			}
			if (this.limit > 0)
			{
				this.sfso.PutShort(FindUsersRequest.KEY_LIMIT, (short)this.limit);
			}
		}
	}
}
