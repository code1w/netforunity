using KaiGeX.Entities;
using KaiGeX.Entities.Match;
using KaiGeX.Exceptions;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests.Game
{
	public class QuickJoinGameRequest : BaseRequest
	{
		private static readonly int MAX_ROOMS = 32;
		public static readonly string KEY_ROOM_LIST = "rl";
		public static readonly string KEY_GROUP_LIST = "gl";
		public static readonly string KEY_ROOM_TO_LEAVE = "tl";
		public static readonly string KEY_MATCH_EXPRESSION = "me";
		private List<Room> whereToSearchRoom;
		private List<string> whereToSearchString;
		private MatchExpression matchExpression;
		private Room roomToLeave;
		private bool isSearchListString = false;
		private bool isSearchListRoom = false;
		private void Init(MatchExpression matchExpression, List<string> whereToSearch, Room roomToLeave)
		{
			this.matchExpression = matchExpression;
			this.whereToSearchString = whereToSearch;
			this.roomToLeave = roomToLeave;
			this.isSearchListString = true;
		}
		public QuickJoinGameRequest(MatchExpression matchExpression, List<string> whereToSearch, Room roomToLeave) : base(RequestType.QuickJoinGame)
		{
			this.Init(matchExpression, whereToSearch, roomToLeave);
		}
		public QuickJoinGameRequest(MatchExpression matchExpression, List<string> whereToSearch) : base(RequestType.QuickJoinGame)
		{
			this.Init(matchExpression, whereToSearch, null);
		}
		public QuickJoinGameRequest(MatchExpression matchExpression, List<Room> whereToSearch, Room roomToLeave) : base(RequestType.QuickJoinGame)
		{
			this.Init(matchExpression, whereToSearch, roomToLeave);
		}
		public QuickJoinGameRequest(MatchExpression matchExpression, List<Room> whereToSearch) : base(RequestType.QuickJoinGame)
		{
			this.Init(matchExpression, whereToSearch, null);
		}
		private void Init(MatchExpression matchExpression, List<Room> whereToSearch, Room roomToLeave)
		{
			this.matchExpression = matchExpression;
			this.whereToSearchRoom = whereToSearch;
			this.roomToLeave = roomToLeave;
			this.isSearchListRoom = true;
		}
		public override void Validate(KaiGeNet sfs)
		{
			List<string> list = new List<string>();
			if (this.isSearchListRoom)
			{
				if (this.whereToSearchRoom == null || this.whereToSearchRoom.Count < 1)
				{
					list.Add("Missing whereToSearch parameter");
				}
				else
				{
					if (this.whereToSearchRoom.Count > QuickJoinGameRequest.MAX_ROOMS)
					{
						list.Add("Too many Rooms specified in the whereToSearch parameter. Client limit is: " + QuickJoinGameRequest.MAX_ROOMS);
					}
				}
			}
			if (this.isSearchListString)
			{
				if (this.whereToSearchString == null || this.whereToSearchString.Count < 1)
				{
					list.Add("Missing whereToSearch parameter");
				}
				else
				{
					if (this.whereToSearchString.Count > QuickJoinGameRequest.MAX_ROOMS)
					{
						list.Add("Too many Rooms specified in the whereToSearch parameter. Client limit is: " + QuickJoinGameRequest.MAX_ROOMS);
					}
				}
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("QuickJoinGame request error", list);
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			if (this.isSearchListString)
			{
				this.sfso.PutUtfStringArray(QuickJoinGameRequest.KEY_GROUP_LIST, this.whereToSearchString.ToArray());
			}
			else
			{
				if (this.isSearchListRoom)
				{
					List<int> list = new List<int>();
					foreach (Room current in this.whereToSearchRoom)
					{
						list.Add(current.Id);
					}
					this.sfso.PutIntArray(QuickJoinGameRequest.KEY_ROOM_LIST, list.ToArray());
				}
			}
			if (this.roomToLeave != null)
			{
				this.sfso.PutInt(QuickJoinGameRequest.KEY_ROOM_TO_LEAVE, this.roomToLeave.Id);
			}
			if (this.matchExpression != null)
			{
				this.sfso.PutSFSArray(QuickJoinGameRequest.KEY_MATCH_EXPRESSION, this.matchExpression.ToSFSArray());
			}
		}
	}
}
