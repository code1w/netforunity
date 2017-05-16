using System;
using System.Collections;
namespace KaiGeX.Core
{
	public class SFSEvent : BaseEvent
	{
		public static readonly string HANDSHAKE = "handshake";
		public static readonly string UDP_INIT = "udpInit";
		public static readonly string CONNECTION = "connection";
		public static readonly string PING_PONG = "pingPong";
		public static readonly string SOCKET_ERROR = "socketError";
		public static readonly string CONNECTION_LOST = "connectionLost";
		public static readonly string CONNECTION_RETRY = "connectionRetry";
		public static readonly string CONNECTION_RESUME = "connectionResume";
		public static readonly string CONNECTION_ATTEMPT_HTTP = "connectionAttemptHttp";
		public static readonly string CONFIG_LOAD_SUCCESS = "configLoadSuccess";
		public static readonly string CONFIG_LOAD_FAILURE = "configLoadFailure";
		public static readonly string LOGIN = "login";
		public static readonly string LOGIN_ERROR = "loginError";
		public static readonly string LOGOUT = "logout";
		public static readonly string ROOM_ADD = "roomAdd";
		public static readonly string ROOM_REMOVE = "roomRemove";
		public static readonly string ROOM_CREATION_ERROR = "roomCreationError";
		public static readonly string ROOM_JOIN = "roomJoin";
		public static readonly string ROOM_JOIN_ERROR = "roomJoinError";
		public static readonly string USER_ENTER_ROOM = "userEnterRoom";
		public static readonly string USER_EXIT_ROOM = "userExitRoom";
		public static readonly string USER_COUNT_CHANGE = "userCountChange";
		public static readonly string PUBLIC_MESSAGE = "publicMessage";
		public static readonly string PRIVATE_MESSAGE = "privateMessage";
		public static readonly string MODERATOR_MESSAGE = "moderatorMessage";
		public static readonly string ADMIN_MESSAGE = "adminMessage";
		public static readonly string OBJECT_MESSAGE = "objectMessage";
		public static readonly string EXTENSION_RESPONSE = "extensionResponse";
		public static readonly string ROOM_VARIABLES_UPDATE = "roomVariablesUpdate";
		public static readonly string USER_VARIABLES_UPDATE = "userVariablesUpdate";
		public static readonly string ROOM_GROUP_SUBSCRIBE = "roomGroupSubscribe";
		public static readonly string ROOM_GROUP_UNSUBSCRIBE = "roomGroupUnsubscribe";
		public static readonly string ROOM_GROUP_SUBSCRIBE_ERROR = "roomGroupSubscribeError";
		public static readonly string ROOM_GROUP_UNSUBSCRIBE_ERROR = "roomGroupUnsubscribeError";
		public static readonly string SPECTATOR_TO_PLAYER = "spectatorToPlayer";
		public static readonly string PLAYER_TO_SPECTATOR = "playerToSpectator";
		public static readonly string SPECTATOR_TO_PLAYER_ERROR = "spectatorToPlayerError";
		public static readonly string PLAYER_TO_SPECTATOR_ERROR = "playerToSpectatorError";
		public static readonly string ROOM_NAME_CHANGE = "roomNameChange";
		public static readonly string ROOM_NAME_CHANGE_ERROR = "roomNameChangeError";
		public static readonly string ROOM_PASSWORD_STATE_CHANGE = "roomPasswordStateChange";
		public static readonly string ROOM_PASSWORD_STATE_CHANGE_ERROR = "roomPasswordStateChangeError";
		public static readonly string ROOM_CAPACITY_CHANGE = "roomCapacityChange";
		public static readonly string ROOM_CAPACITY_CHANGE_ERROR = "roomCapacityChangeError";
		public static readonly string ROOM_FIND_RESULT = "roomFindResult";
		public static readonly string USER_FIND_RESULT = "userFindResult";
		public static readonly string INVITATION = "invitation";
		public static readonly string INVITATION_REPLY = "invitationReply";
		public static readonly string INVITATION_REPLY_ERROR = "invitationReplyError";
		public static readonly string PROXIMITY_LIST_UPDATE = "proximityListUpdate";
		public static readonly string MMOITEM_VARIABLES_UPDATE = "mmoItemVariablesUpdate";
        //zxb 2014-2-21
        public static readonly string RESPONSE_CHANGELEVEL = "responsechangelevel"; //
        public static readonly string RESPONSE_CLIENTINIT = "clientinit";

        public static readonly string RESPONSE_LOGINGAME = "logingame";

        public static readonly string RESPONSE_LOGINREWARD = "loginreward";

        public static readonly string RESPONSE_CHARPACKAGE = "charpackage";

        public static readonly string RESPONSE_CHANGEPACKAGE = "changepackage";

        public static readonly string RESPONSE_SKILLBUFF = "skillbuff";

        public static readonly string RESPONSE_SKILL = "skill";

        public static readonly string RESPONSE_CONFIGDATA = "configdata";

        public static readonly string RESPONSE_CUMLOGINREWARDDATA = "rewarddata";

        public static readonly string RESPONSE_FANPAIITEMDATA = "fanpaiitemdata";

        public static readonly string RESPONSE_STAGEDATA = "stagedata";

        public static readonly string RESPONSE_MAPSCREENREFRESHCHARACTER = "mapscreenrefreshcharacter";

        public static readonly string RESPONSE_NINESCREENREFRESHPLAYER = "ninescreenrefreshplayer";

		public SFSEvent(string type, Hashtable data) : base(type, data)
		{
		}
		public SFSEvent(string type) : base(type, null)
		{
		}
	}
}
