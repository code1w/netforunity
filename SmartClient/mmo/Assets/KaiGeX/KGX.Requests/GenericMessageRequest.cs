using KaiGeX.Entities;
using KaiGeX.Entities.Data;
using KaiGeX.Exceptions;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests
{
	public class GenericMessageRequest : BaseRequest
	{
		public static readonly string KEY_ROOM_ID = "r";
		public static readonly string KEY_USER_ID = "u";
		public static readonly string KEY_MESSAGE = "m";
		public static readonly string KEY_MESSAGE_TYPE = "t";
		public static readonly string KEY_RECIPIENT = "rc";
		public static readonly string KEY_RECIPIENT_MODE = "rm";
		public static readonly string KEY_XTRA_PARAMS = "p";
		public static readonly string KEY_SENDER_DATA = "sd";
		protected int type = -1;
		protected Room room;
		protected User user;
		protected string message;
		protected ISFSObject parameters;
		protected object recipient;
		protected int sendMode;
		public GenericMessageRequest() : base(RequestType.GenericMessage)
		{
		}
		public override void Validate(KaiGeNet sfs)
		{
			if (this.type < 0)
			{
				throw new SFSValidationError("PublicMessage request error", new string[]
				{
					"Unsupported message type: " + this.type
				});
			}
			List<string> list = new List<string>();
			switch (this.type)
			{
			case 0:
				this.ValidatePublicMessage(sfs, list);
				goto IL_A9;
			case 1:
				this.ValidatePrivateMessage(sfs, list);
				goto IL_A9;
			case 4:
				this.ValidateObjectMessage(sfs, list);
				goto IL_A9;
			case 5:
				this.ValidateBuddyMessage(sfs, list);
				goto IL_A9;
			}
			this.ValidateSuperUserMessage(sfs, list);
			IL_A9:
			if (list.Count > 0)
			{
				throw new SFSValidationError("Request error - ", list);
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			this.sfso.PutByte(GenericMessageRequest.KEY_MESSAGE_TYPE, Convert.ToByte(this.type));
			switch (this.type)
			{
			case 0:
				this.ExecutePublicMessage(sfs);
				return;
			case 1:
				this.ExecutePrivateMessage(sfs);
				return;
			case 4:
				this.ExecuteObjectMessage(sfs);
				return;
			case 5:
				this.ExecuteBuddyMessage(sfs);
				return;
			}
			this.ExecuteSuperUserMessage(sfs);
		}
		private void ValidatePublicMessage(KaiGeNet sfs, List<string> errors)
		{
			if (this.message == null || this.message.Length == 0)
			{
				errors.Add("Public message is empty!");
			}
			if (this.room != null && !sfs.MySelf.IsJoinedInRoom(this.room))
			{
				errors.Add("You are not joined in the target Room: " + this.room);
			}
		}
		private void ValidatePrivateMessage(KaiGeNet sfs, List<string> errors)
		{
			if (this.message == null || this.message.Length == 0)
			{
				errors.Add("Private message is empty!");
			}
			if ((int)this.recipient < 0)
			{
				errors.Add("Invalid recipient id: " + this.recipient);
			}
		}
		private void ValidateObjectMessage(KaiGeNet sfs, List<string> errors)
		{
			if (this.parameters == null)
			{
				errors.Add("Object message is null!");
			}
		}
		private void ValidateBuddyMessage(KaiGeNet sfs, List<string> errors)
		{
			if (!sfs.BuddyManager.Inited)
			{
				errors.Add("BuddyList is not inited. Please send an InitBuddyRequest first.");
			}
			if (!sfs.BuddyManager.MyOnlineState)
			{
				errors.Add("Can't send messages while off-line");
			}
			if (this.message == null || this.message.Length == 0)
			{
				errors.Add("Buddy message is empty!");
			}
			int num = (int)this.recipient;
			if (num < 0)
			{
				errors.Add("Recipient is not online or not in your buddy list");
			}
		}
		private void ValidateSuperUserMessage(KaiGeNet sfs, List<string> errors)
		{
			if (this.message == null || this.message.Length == 0)
			{
				errors.Add("Moderator message is empty!");
			}
			switch (this.sendMode)
			{
			case 0:
				if (!(this.recipient is User))
				{
					errors.Add("TO_USER expects a User object as recipient");
				}
				break;
			case 1:
				if (!(this.recipient is Room))
				{
					errors.Add("TO_ROOM expects a Room object as recipient");
				}
				break;
			case 2:
				if (!(this.recipient is string))
				{
					errors.Add("TO_GROUP expects a String object (the groupId) as recipient");
				}
				break;
			}
		}
		private void ExecutePublicMessage(KaiGeNet sfs)
		{
			if (this.room == null)
			{
				this.room = sfs.LastJoinedRoom;
			}
			if (this.room == null)
			{
				throw new SFSError("User should be joined in a room in order to send a public message");
			}
			this.sfso.PutInt(GenericMessageRequest.KEY_ROOM_ID, this.room.Id);
			this.sfso.PutInt(GenericMessageRequest.KEY_USER_ID, sfs.MySelf.Id);
			this.sfso.PutUtfString(GenericMessageRequest.KEY_MESSAGE, this.message);
			if (this.parameters != null)
			{
				this.sfso.PutSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS, this.parameters);
			}
		}
		private void ExecutePrivateMessage(KaiGeNet sfs)
		{
			this.sfso.PutInt(GenericMessageRequest.KEY_RECIPIENT, (int)this.recipient);
			this.sfso.PutUtfString(GenericMessageRequest.KEY_MESSAGE, this.message);
			if (this.parameters != null)
			{
				this.sfso.PutSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS, this.parameters);
			}
		}
		private void ExecuteBuddyMessage(KaiGeNet sfs)
		{
			this.sfso.PutInt(GenericMessageRequest.KEY_RECIPIENT, (int)this.recipient);
			this.sfso.PutUtfString(GenericMessageRequest.KEY_MESSAGE, this.message);
			if (this.parameters != null)
			{
				this.sfso.PutSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS, this.parameters);
			}
		}
		private void ExecuteSuperUserMessage(KaiGeNet sfs)
		{
			this.sfso.PutUtfString(GenericMessageRequest.KEY_MESSAGE, this.message);
			if (this.parameters != null)
			{
				this.sfso.PutSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS, this.parameters);
			}
			this.sfso.PutInt(GenericMessageRequest.KEY_RECIPIENT_MODE, this.sendMode);
			switch (this.sendMode)
			{
			case 0:
				this.sfso.PutInt(GenericMessageRequest.KEY_RECIPIENT, ((User)this.recipient).Id);
				break;
			case 1:
				this.sfso.PutInt(GenericMessageRequest.KEY_RECIPIENT, ((Room)this.recipient).Id);
				break;
			case 2:
				this.sfso.PutUtfString(GenericMessageRequest.KEY_RECIPIENT, (string)this.recipient);
				break;
			}
		}
		private void ExecuteObjectMessage(KaiGeNet sfs)
		{
			if (this.room == null)
			{
				this.room = sfs.LastJoinedRoom;
			}
			List<int> list = new List<int>();
			ICollection<User> collection = this.recipient as ICollection<User>;
			if (collection != null)
			{
				if (collection.Count > this.room.Capacity)
				{
					throw new ArgumentException("The number of recipients is bigger than the target Room capacity: " + collection.Count);
				}
				foreach (User current in collection)
				{
					if (!list.Contains(current.Id))
					{
						list.Add(current.Id);
					}
				}
			}
			this.sfso.PutInt(GenericMessageRequest.KEY_ROOM_ID, this.room.Id);
			this.sfso.PutSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS, this.parameters);
			if (list.Count > 0)
			{
				this.sfso.PutIntArray(GenericMessageRequest.KEY_RECIPIENT, list.ToArray());
			}
		}
	}
}
