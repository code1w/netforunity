using KaiGeX.Entities;
using KaiGeX.Entities.Data;
using KaiGeX.Entities.Variables;
using KaiGeX.Exceptions;
using KaiGeX.Requests.MMO;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests
{
	public class CreateRoomRequest : BaseRequest
	{
		public static readonly string KEY_ROOM = "r";
		public static readonly string KEY_NAME = "n";
		public static readonly string KEY_PASSWORD = "p";
		public static readonly string KEY_GROUP_ID = "g";
		public static readonly string KEY_ISGAME = "ig";
		public static readonly string KEY_MAXUSERS = "mu";
		public static readonly string KEY_MAXSPECTATORS = "ms";
		public static readonly string KEY_MAXVARS = "mv";
		public static readonly string KEY_ROOMVARS = "rv";
		public static readonly string KEY_PERMISSIONS = "pm";
		public static readonly string KEY_EVENTS = "ev";
		public static readonly string KEY_EXTID = "xn";
		public static readonly string KEY_EXTCLASS = "xc";
		public static readonly string KEY_EXTPROP = "xp";
		public static readonly string KEY_AUTOJOIN = "aj";
		public static readonly string KEY_ROOM_TO_LEAVE = "rl";
		public static readonly string KEY_MMO_DEFAULT_AOI = "maoi";
		public static readonly string KEY_MMO_MAP_LOW_LIMIT = "mllm";
		public static readonly string KEY_MMO_MAP_HIGH_LIMIT = "mlhm";
		public static readonly string KEY_MMO_USER_MAX_LIMBO_SECONDS = "muls";
		public static readonly string KEY_MMO_PROXIMITY_UPDATE_MILLIS = "mpum";
		public static readonly string KEY_MMO_SEND_ENTRY_POINT = "msep";
		private RoomSettings settings;
		private bool autoJoin;
		private Room roomToLeave;
		private void Init(RoomSettings settings, bool autoJoin, Room roomToLeave)
		{
			this.settings = settings;
			this.autoJoin = autoJoin;
			this.roomToLeave = roomToLeave;
		}
		public CreateRoomRequest(RoomSettings settings, bool autoJoin, Room roomToLeave) : base(RequestType.CreateRoom)
		{
			this.Init(settings, autoJoin, roomToLeave);
		}
		public CreateRoomRequest(RoomSettings settings, bool autoJoin) : base(RequestType.CreateRoom)
		{
			this.Init(settings, autoJoin, null);
		}
		public CreateRoomRequest(RoomSettings settings) : base(RequestType.CreateRoom)
		{
			this.Init(settings, false, null);
		}
		public override void Validate(KaiGeNet sfs)
		{
			List<string> list = new List<string>();
			if (this.settings.Name == null || this.settings.Name.Length == 0)
			{
				list.Add("Missing room name");
			}
			if (this.settings.MaxUsers <= 0)
			{
				list.Add("maxUsers must be > 0");
			}
			if (this.settings.Extension != null)
			{
				if (this.settings.Extension.ClassName == null || this.settings.Extension.ClassName.Length == 0)
				{
					list.Add("Missing Extension class name");
				}
				if (this.settings.Extension.Id == null || this.settings.Extension.Id.Length == 0)
				{
					list.Add("Missing Extension id");
				}
			}
			if (this.settings is MMORoomSettings)
			{
				MMORoomSettings mMORoomSettings = this.settings as MMORoomSettings;
				if (mMORoomSettings.DefaultAOI == null)
				{
					list.Add("Missing default AOI (area of interest)");
				}
				if (mMORoomSettings.MapLimits != null && (mMORoomSettings.MapLimits.LowerLimit == null || mMORoomSettings.MapLimits.HigherLimit == null))
				{
					list.Add("Map limits must be both defined");
				}
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("CreateRoom request error", list);
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			this.sfso.PutUtfString(CreateRoomRequest.KEY_NAME, this.settings.Name);
			this.sfso.PutUtfString(CreateRoomRequest.KEY_GROUP_ID, this.settings.GroupId);
			this.sfso.PutUtfString(CreateRoomRequest.KEY_PASSWORD, this.settings.Password);
			this.sfso.PutBool(CreateRoomRequest.KEY_ISGAME, this.settings.IsGame);
			this.sfso.PutShort(CreateRoomRequest.KEY_MAXUSERS, this.settings.MaxUsers);
			this.sfso.PutShort(CreateRoomRequest.KEY_MAXSPECTATORS, this.settings.MaxSpectators);
			this.sfso.PutShort(CreateRoomRequest.KEY_MAXVARS, this.settings.MaxVariables);
			if (this.settings.Variables != null && this.settings.Variables.Count > 0)
			{
				ISFSArray iSFSArray = SFSArray.NewInstance();
				foreach (object current in this.settings.Variables)
				{
					if (current is RoomVariable)
					{
						RoomVariable roomVariable = current as RoomVariable;
						iSFSArray.AddSFSArray(roomVariable.ToSFSArray());
					}
				}
				this.sfso.PutSFSArray(CreateRoomRequest.KEY_ROOMVARS, iSFSArray);
			}
			if (this.settings.Permissions != null)
			{
				List<bool> list = new List<bool>();
				list.Add(this.settings.Permissions.AllowNameChange);
				list.Add(this.settings.Permissions.AllowPasswordStateChange);
				list.Add(this.settings.Permissions.AllowPublicMessages);
				list.Add(this.settings.Permissions.AllowResizing);
				this.sfso.PutBoolArray(CreateRoomRequest.KEY_PERMISSIONS, list.ToArray());
			}
			if (this.settings.Events != null)
			{
				List<bool> list2 = new List<bool>();
				list2.Add(this.settings.Events.AllowUserEnter);
				list2.Add(this.settings.Events.AllowUserExit);
				list2.Add(this.settings.Events.AllowUserCountChange);
				list2.Add(this.settings.Events.AllowUserVariablesUpdate);
				this.sfso.PutBoolArray(CreateRoomRequest.KEY_EVENTS, list2.ToArray());
			}
			if (this.settings.Extension != null)
			{
				this.sfso.PutUtfString(CreateRoomRequest.KEY_EXTID, this.settings.Extension.Id);
				this.sfso.PutUtfString(CreateRoomRequest.KEY_EXTCLASS, this.settings.Extension.ClassName);
				if (this.settings.Extension.PropertiesFile != null && this.settings.Extension.PropertiesFile.Length > 0)
				{
					this.sfso.PutUtfString(CreateRoomRequest.KEY_EXTPROP, this.settings.Extension.PropertiesFile);
				}
			}
			if (this.settings is MMORoomSettings)
			{
				MMORoomSettings mMORoomSettings = this.settings as MMORoomSettings;
				bool flag = mMORoomSettings.DefaultAOI.IsFloat();
				if (flag)
				{
					this.sfso.PutFloatArray(CreateRoomRequest.KEY_MMO_DEFAULT_AOI, mMORoomSettings.DefaultAOI.ToFloatArray());
					if (mMORoomSettings.MapLimits != null)
					{
						this.sfso.PutFloatArray(CreateRoomRequest.KEY_MMO_MAP_LOW_LIMIT, mMORoomSettings.MapLimits.LowerLimit.ToFloatArray());
						this.sfso.PutFloatArray(CreateRoomRequest.KEY_MMO_MAP_HIGH_LIMIT, mMORoomSettings.MapLimits.HigherLimit.ToFloatArray());
					}
				}
				else
				{
					this.sfso.PutIntArray(CreateRoomRequest.KEY_MMO_DEFAULT_AOI, mMORoomSettings.DefaultAOI.ToIntArray());
					if (mMORoomSettings.MapLimits != null)
					{
						this.sfso.PutIntArray(CreateRoomRequest.KEY_MMO_MAP_LOW_LIMIT, mMORoomSettings.MapLimits.LowerLimit.ToIntArray());
						this.sfso.PutIntArray(CreateRoomRequest.KEY_MMO_MAP_HIGH_LIMIT, mMORoomSettings.MapLimits.HigherLimit.ToIntArray());
					}
				}
				this.sfso.PutShort(CreateRoomRequest.KEY_MMO_USER_MAX_LIMBO_SECONDS, (short)mMORoomSettings.UserMaxLimboSeconds);
				this.sfso.PutShort(CreateRoomRequest.KEY_MMO_PROXIMITY_UPDATE_MILLIS, (short)mMORoomSettings.ProximityListUpdateMillis);
				this.sfso.PutBool(CreateRoomRequest.KEY_MMO_SEND_ENTRY_POINT, mMORoomSettings.SendAOIEntryPoint);
			}
			this.sfso.PutBool(CreateRoomRequest.KEY_AUTOJOIN, this.autoJoin);
			if (this.roomToLeave != null)
			{
				this.sfso.PutInt(CreateRoomRequest.KEY_ROOM_TO_LEAVE, this.roomToLeave.Id);
			}
		}
	}
}
