using Sfs2X.Entities.Match;
using Sfs2X.Exceptions;
using System;
using System.Collections.Generic;
namespace Sfs2X.Requests
{
	public class FindRoomsRequest : BaseRequest
	{
		public static readonly string KEY_EXPRESSION = "e";
		public static readonly string KEY_GROUP = "g";
		public static readonly string KEY_LIMIT = "l";
		public static readonly string KEY_FILTERED_ROOMS = "fr";
		private MatchExpression matchExpr;
		private string groupId;
		private int limit;
		private void Init(MatchExpression expr, string groupId, int limit)
		{
			this.matchExpr = expr;
			this.groupId = groupId;
			this.limit = limit;
		}
		public FindRoomsRequest(MatchExpression expr, string groupId, int limit) : base(RequestType.FindRooms)
		{
			this.Init(expr, groupId, limit);
		}
		public FindRoomsRequest(MatchExpression expr) : base(RequestType.FindRooms)
		{
			this.Init(expr, null, 0);
		}
		public FindRoomsRequest(MatchExpression expr, string groupId) : base(RequestType.FindRooms)
		{
			this.Init(expr, groupId, 0);
		}
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (this.matchExpr == null)
			{
				list.Add("Missing Match Expression");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("FindRooms request error", list);
			}
		}
		public override void Execute(SmartFox sfs)
		{
			this.sfso.PutSFSArray(FindRoomsRequest.KEY_EXPRESSION, this.matchExpr.ToSFSArray());
			if (this.groupId != null)
			{
				this.sfso.PutUtfString(FindRoomsRequest.KEY_GROUP, this.groupId);
			}
			if (this.limit > 0)
			{
				this.sfso.PutShort(FindRoomsRequest.KEY_LIMIT, (short)this.limit);
			}
		}
	}
}
