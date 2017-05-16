using KaiGeX.Bitswarm;
using KaiGeX.Core;
using KaiGeX.Entities;
using KaiGeX.Entities.Data;
using KaiGeX.Entities.Invitation;
using KaiGeX.Entities.Managers;
using KaiGeX.Entities.Variables;
using KaiGeX.Requests;
using KaiGeX.Requests.Buddylist;
using KaiGeX.Requests.Game;
using KaiGeX.Requests.MMO;
using KaiGeX.Util;
using System;
using System.Collections;
using System.Collections.Generic;
namespace KaiGeX.Controllers
{
    public class SystemController : BaseController
    {
        private Dictionary<int, RequestDelegate> requestHandlers;
        public SystemController(BitSwarmClient bitSwarm)
            : base(bitSwarm)
        {
            this.requestHandlers = new Dictionary<int, RequestDelegate>();
            this.InitRequestHandlers();
        }
        private void InitRequestHandlers()
        {
            this.requestHandlers[0] = new RequestDelegate(this.FnHandshake);
            this.requestHandlers[1] = new RequestDelegate(this.FnLogin);
            this.requestHandlers[2] = new RequestDelegate(this.FnLogout);
            this.requestHandlers[4] = new RequestDelegate(this.FnJoinRoom);
            this.requestHandlers[6] = new RequestDelegate(this.FnCreateRoom);
            this.requestHandlers[7] = new RequestDelegate(this.FnGenericMessage);
            this.requestHandlers[8] = new RequestDelegate(this.FnChangeRoomName);
            this.requestHandlers[9] = new RequestDelegate(this.FnChangeRoomPassword);
            this.requestHandlers[19] = new RequestDelegate(this.FnChangeRoomCapacity);
            this.requestHandlers[11] = new RequestDelegate(this.FnSetRoomVariables);
            this.requestHandlers[12] = new RequestDelegate(this.FnSetUserVariables);
            this.requestHandlers[15] = new RequestDelegate(this.FnSubscribeRoomGroup);
            this.requestHandlers[16] = new RequestDelegate(this.FnUnsubscribeRoomGroup);
            this.requestHandlers[17] = new RequestDelegate(this.FnSpectatorToPlayer);
            this.requestHandlers[18] = new RequestDelegate(this.FnPlayerToSpectator);
            this.requestHandlers[200] = new RequestDelegate(this.FnInitBuddyList);
            this.requestHandlers[201] = new RequestDelegate(this.FnAddBuddy);
            this.requestHandlers[203] = new RequestDelegate(this.FnRemoveBuddy);
            this.requestHandlers[202] = new RequestDelegate(this.FnBlockBuddy);
            this.requestHandlers[205] = new RequestDelegate(this.FnGoOnline);
            this.requestHandlers[204] = new RequestDelegate(this.FnSetBuddyVariables);
            this.requestHandlers[27] = new RequestDelegate(this.FnFindRooms);
            this.requestHandlers[28] = new RequestDelegate(this.FnFindUsers);
            this.requestHandlers[300] = new RequestDelegate(this.FnInviteUsers);
            this.requestHandlers[301] = new RequestDelegate(this.FnInvitationReply);
            this.requestHandlers[303] = new RequestDelegate(this.FnQuickJoinGame);
            this.requestHandlers[29] = new RequestDelegate(this.FnPingPong);
            this.requestHandlers[30] = new RequestDelegate(this.FnSetUserPosition);
            this.requestHandlers[1000] = new RequestDelegate(this.FnUserEnterRoom);
            this.requestHandlers[1001] = new RequestDelegate(this.FnUserCountChange);
            this.requestHandlers[1002] = new RequestDelegate(this.FnUserLost);
            this.requestHandlers[1003] = new RequestDelegate(this.FnRoomLost);
            this.requestHandlers[1004] = new RequestDelegate(this.FnUserExitRoom);
            this.requestHandlers[1005] = new RequestDelegate(this.FnClientDisconnection);
            this.requestHandlers[1006] = new RequestDelegate(this.FnReconnectionFailure);
            this.requestHandlers[1007] = new RequestDelegate(this.FnSetMMOItemVariables);
        }

        public override void HandleMessage(IMessage message)
        {
            if (this.sfs.Debug)
            {
                this.log.Info(new string[]
				{
					string.Concat(new object[]
					{
						"Message: ",
						(RequestType)message.Id,
						" ",
						message
					})
				});
            }
            if (!this.requestHandlers.ContainsKey(message.Id))
            {
                this.log.Warn(new string[]
				{
					"Unknown message id: " + message.Id
				});
            }
            else
            {
                RequestDelegate requestDelegate = this.requestHandlers[message.Id];
                requestDelegate(message);
            }
        }
        private void FnHandshake(IMessage msg)
        {
            Hashtable hashtable = new Hashtable();
            hashtable["message"] = msg.Content;
            SFSEvent evt = new SFSEvent(SFSEvent.HANDSHAKE, hashtable);
            this.sfs.HandleHandShake(evt);
            this.sfs.DispatchEvent(evt);
        }
        private void FnLogin(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                this.PopulateRoomList(content.GetSFSArray(LoginRequest.KEY_ROOMLIST));
                this.sfs.MySelf = new SFSUser(content.GetInt(LoginRequest.KEY_ID), content.GetUtfString(LoginRequest.KEY_USER_NAME), true);
                this.sfs.MySelf.UserManager = this.sfs.UserManager;
                this.sfs.MySelf.PrivilegeId = (int)content.GetShort(LoginRequest.KEY_PRIVILEGE_ID);
                this.sfs.UserManager.AddUser(this.sfs.MySelf);
                this.sfs.SetReconnectionSeconds((int)content.GetShort(LoginRequest.KEY_RECONNECTION_SECONDS));
                this.sfs.MySelf.PrivilegeId = (int)content.GetShort(LoginRequest.KEY_PRIVILEGE_ID);
                hashtable["zone"] = content.GetUtfString(LoginRequest.KEY_ZONE_NAME);
                hashtable["user"] = this.sfs.MySelf;
                hashtable["data"] = content.GetSFSObject(LoginRequest.KEY_PARAMS);
                SFSEvent evt = new SFSEvent(SFSEvent.LOGIN, hashtable);
                this.sfs.HandleLogin(evt);
                this.sfs.DispatchEvent(evt);
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.LOGIN_ERROR, hashtable));
            }
        }
        private void FnCreateRoom(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                IRoomManager roomManager = this.sfs.RoomManager;
                Room room = SFSRoom.FromSFSArray(content.GetSFSArray(CreateRoomRequest.KEY_ROOM));
                room.RoomManager = this.sfs.RoomManager;
                roomManager.AddRoom(room);
                hashtable["room"] = room;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_ADD, hashtable));
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_CREATION_ERROR, hashtable));
            }
        }
        private void FnJoinRoom(IMessage msg)
        {
            IRoomManager roomManager = this.sfs.RoomManager;
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            this.sfs.IsJoining = false;
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                ISFSArray sFSArray = content.GetSFSArray(JoinRoomRequest.KEY_ROOM);
                ISFSArray sFSArray2 = content.GetSFSArray(JoinRoomRequest.KEY_USER_LIST);
                Room room = SFSRoom.FromSFSArray(sFSArray);
                room.RoomManager = this.sfs.RoomManager;
                room = roomManager.ReplaceRoom(room, roomManager.ContainsGroup(room.GroupId));
                for (int i = 0; i < sFSArray2.Size(); i++)
                {
                    ISFSArray sFSArray3 = sFSArray2.GetSFSArray(i);
                    User orCreateUser = this.GetOrCreateUser(sFSArray3, true, room);
                    orCreateUser.SetPlayerId((int)sFSArray3.GetShort(3), room);
                    room.AddUser(orCreateUser);
                }
                room.IsJoined = true;
                this.sfs.LastJoinedRoom = room;
                hashtable["room"] = room;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_JOIN, hashtable));
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_JOIN_ERROR, hashtable));
            }
        }
        private void FnUserEnterRoom(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            Room roomById = this.sfs.RoomManager.GetRoomById(content.GetInt("r"));
            if (roomById != null)
            {
                ISFSArray sFSArray = content.GetSFSArray("u");
                User orCreateUser = this.GetOrCreateUser(sFSArray, true, roomById);
                roomById.AddUser(orCreateUser);
                hashtable["user"] = orCreateUser;
                hashtable["room"] = roomById;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.USER_ENTER_ROOM, hashtable));
            }
        }
        private void FnUserCountChange(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            Room roomById = this.sfs.RoomManager.GetRoomById(content.GetInt("r"));
            if (roomById != null)
            {
                int @short = (int)content.GetShort("uc");
                int num = 0;
                if (content.ContainsKey("sc"))
                {
                    num = (int)content.GetShort("sc");
                }
                roomById.UserCount = @short;
                roomById.SpectatorCount = num;
                hashtable["room"] = roomById;
                hashtable["uCount"] = @short;
                hashtable["sCount"] = num;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.USER_COUNT_CHANGE, hashtable));
            }
        }
        private void FnUserLost(IMessage msg)
        {
            ISFSObject content = msg.Content;
            int @int = content.GetInt("u");
            User userById = this.sfs.UserManager.GetUserById(@int);
            if (userById != null)
            {
                List<Room> userRooms = this.sfs.RoomManager.GetUserRooms(userById);
                this.sfs.RoomManager.RemoveUser(userById);
                this.sfs.UserManager.RemoveUser(userById);
                foreach (Room current in userRooms)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable["user"] = userById;
                    hashtable["room"] = current;
                    this.sfs.DispatchEvent(new SFSEvent(SFSEvent.USER_EXIT_ROOM, hashtable));
                }
            }
        }
        private void FnRoomLost(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            int @int = content.GetInt("r");
            Room roomById = this.sfs.RoomManager.GetRoomById(@int);
            IUserManager userManager = this.sfs.UserManager;
            if (roomById != null)
            {
                this.sfs.RoomManager.RemoveRoom(roomById);
                foreach (User current in roomById.UserList)
                {
                    userManager.RemoveUser(current);
                }
                hashtable["room"] = roomById;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_REMOVE, hashtable));
            }
        }
        private void FnUserExitRoom(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            int @int = content.GetInt("r");
            int int2 = content.GetInt("u");
            Room roomById = this.sfs.RoomManager.GetRoomById(@int);
            User userById = this.sfs.UserManager.GetUserById(int2);
            if (roomById != null && userById != null)
            {
                roomById.RemoveUser(userById);
                this.sfs.UserManager.RemoveUser(userById);
                if (userById.IsItMe && roomById.IsJoined)
                {
                    roomById.IsJoined = false;
                    if (this.sfs.JoinedRooms.Count == 0)
                    {
                        this.sfs.LastJoinedRoom = null;
                    }
                    if (!roomById.IsManaged)
                    {
                        this.sfs.RoomManager.RemoveRoom(roomById);
                    }
                }
                hashtable["user"] = userById;
                hashtable["room"] = roomById;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.USER_EXIT_ROOM, hashtable));
            }
            else
            {
                this.log.Debug(new string[]
				{
					string.Concat(new object[]
					{
						"Failed to handle UserExit event. Room: ",
						roomById,
						", User: ",
						userById
					})
				});
            }
        }
        private void FnClientDisconnection(IMessage msg)
        {
            ISFSObject content = msg.Content;
            int @byte = (int)content.GetByte("dr");
            this.sfs.HandleClientDisconnection(ClientDisconnectionReason.GetReason(@byte));
        }
        private void FnSetRoomVariables(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            int @int = content.GetInt(SetRoomVariablesRequest.KEY_VAR_ROOM);
            ISFSArray sFSArray = content.GetSFSArray(SetRoomVariablesRequest.KEY_VAR_LIST);
            Room roomById = this.sfs.RoomManager.GetRoomById(@int);
            ArrayList arrayList = new ArrayList();
            if (roomById != null)
            {
                for (int i = 0; i < sFSArray.Size(); i++)
                {
                    RoomVariable roomVariable = SFSRoomVariable.FromSFSArray(sFSArray.GetSFSArray(i));
                    roomById.SetVariable(roomVariable);
                    arrayList.Add(roomVariable.Name);
                }
                hashtable["changedVars"] = arrayList;
                hashtable["room"] = roomById;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_VARIABLES_UPDATE, hashtable));
            }
            else
            {
                this.log.Warn(new string[]
				{
					"RoomVariablesUpdate, unknown Room id = " + @int
				});
            }
        }
        private void FnSetUserVariables(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            int @int = content.GetInt(SetUserVariablesRequest.KEY_USER);
            ISFSArray sFSArray = content.GetSFSArray(SetUserVariablesRequest.KEY_VAR_LIST);
            User userById = this.sfs.UserManager.GetUserById(@int);
            ArrayList arrayList = new ArrayList();
            if (userById != null)
            {
                for (int i = 0; i < sFSArray.Size(); i++)
                {
                    UserVariable userVariable = SFSUserVariable.FromSFSArray(sFSArray.GetSFSArray(i));
                    userById.SetVariable(userVariable);
                    arrayList.Add(userVariable.Name);
                }
                hashtable["changedVars"] = arrayList;
                hashtable["user"] = userById;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.USER_VARIABLES_UPDATE, hashtable));
            }
            else
            {
                this.log.Warn(new string[]
				{
					"UserVariablesUpdate: unknown user id = " + @int
				});
            }
        }
        private void FnSubscribeRoomGroup(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                string utfString = content.GetUtfString(SubscribeRoomGroupRequest.KEY_GROUP_ID);
                ISFSArray sFSArray = content.GetSFSArray(SubscribeRoomGroupRequest.KEY_ROOM_LIST);
                if (this.sfs.RoomManager.ContainsGroup(utfString))
                {
                    this.log.Warn(new string[]
					{
						"SubscribeGroup Error. Group:" + utfString + "already subscribed!"
					});
                }
                this.PopulateRoomList(sFSArray);
                hashtable["groupId"] = utfString;
                hashtable["newRooms"] = this.sfs.RoomManager.GetRoomListFromGroup(utfString);
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_GROUP_SUBSCRIBE, hashtable));
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_GROUP_SUBSCRIBE_ERROR, hashtable));
            }
        }
        private void FnUnsubscribeRoomGroup(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                string utfString = content.GetUtfString(UnsubscribeRoomGroupRequest.KEY_GROUP_ID);
                if (!this.sfs.RoomManager.ContainsGroup(utfString))
                {
                    this.log.Warn(new string[]
					{
						"UnsubscribeGroup Error. Group:" + utfString + "is not subscribed!"
					});
                }
                this.sfs.RoomManager.RemoveGroup(utfString);
                hashtable["groupId"] = utfString;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_GROUP_UNSUBSCRIBE, hashtable));
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_GROUP_UNSUBSCRIBE_ERROR, hashtable));
            }
        }
        private void FnChangeRoomName(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                int @int = content.GetInt(ChangeRoomNameRequest.KEY_ROOM);
                Room roomById = this.sfs.RoomManager.GetRoomById(@int);
                if (roomById != null)
                {
                    hashtable["oldName"] = roomById.Name;
                    this.sfs.RoomManager.ChangeRoomName(roomById, content.GetUtfString(ChangeRoomNameRequest.KEY_NAME));
                    hashtable["room"] = roomById;
                    this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_NAME_CHANGE, hashtable));
                }
                else
                {
                    this.log.Warn(new string[]
					{
						"Room not found, ID:" + @int + ", Room name change failed."
					});
                }
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_NAME_CHANGE_ERROR, hashtable));
            }
        }
        private void FnChangeRoomPassword(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                int @int = content.GetInt(ChangeRoomPasswordStateRequest.KEY_ROOM);
                Room roomById = this.sfs.RoomManager.GetRoomById(@int);
                if (roomById != null)
                {
                    this.sfs.RoomManager.ChangeRoomPasswordState(roomById, content.GetBool(ChangeRoomPasswordStateRequest.KEY_PASS));
                    hashtable["room"] = roomById;
                    this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_PASSWORD_STATE_CHANGE, hashtable));
                }
                else
                {
                    this.log.Warn(new string[]
					{
						"Room not found, ID:" + @int + ", Room password change failed."
					});
                }
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_PASSWORD_STATE_CHANGE_ERROR, hashtable));
            }
        }
        private void FnChangeRoomCapacity(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                int @int = content.GetInt(ChangeRoomCapacityRequest.KEY_ROOM);
                Room roomById = this.sfs.RoomManager.GetRoomById(@int);
                if (roomById != null)
                {
                    this.sfs.RoomManager.ChangeRoomCapacity(roomById, content.GetInt(ChangeRoomCapacityRequest.KEY_USER_SIZE), content.GetInt(ChangeRoomCapacityRequest.KEY_SPEC_SIZE));
                    hashtable["room"] = roomById;
                    this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_CAPACITY_CHANGE, hashtable));
                }
                else
                {
                    this.log.Warn(new string[]
					{
						"Room not found, ID:" + @int + ", Room capacity change failed."
					});
                }
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_CAPACITY_CHANGE_ERROR, hashtable));
            }
        }
        private void FnLogout(IMessage msg)
        {
            this.sfs.HandleLogout();
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            hashtable["zoneName"] = content.GetUtfString(LogoutRequest.KEY_ZONE_NAME);
            this.sfs.DispatchEvent(new SFSEvent(SFSEvent.LOGOUT, hashtable));
        }
        private User GetOrCreateUser(ISFSArray userObj)
        {
            return this.GetOrCreateUser(userObj, false, null);
        }
        private User GetOrCreateUser(ISFSArray userObj, bool addToGlobalManager)
        {
            return this.GetOrCreateUser(userObj, addToGlobalManager, null);
        }
        private User GetOrCreateUser(ISFSArray userObj, bool addToGlobalManager, Room room)
        {
            int @int = userObj.GetInt(0);
            User user = this.sfs.UserManager.GetUserById(@int);
            if (user == null)
            {
                user = SFSUser.FromSFSArray(userObj, room);
                user.UserManager = this.sfs.UserManager;
            }
            else
            {
                if (room != null)
                {
                    user.SetPlayerId((int)userObj.GetShort(3), room);
                    ISFSArray sFSArray = userObj.GetSFSArray(4);
                    for (int i = 0; i < sFSArray.Size(); i++)
                    {
                        user.SetVariable(SFSUserVariable.FromSFSArray(sFSArray.GetSFSArray(i)));
                    }
                }
            }
            if (addToGlobalManager)
            {
                this.sfs.UserManager.AddUser(user);
            }
            return user;
        }
        private void FnSpectatorToPlayer(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                int @int = content.GetInt(SpectatorToPlayerRequest.KEY_ROOM_ID);
                int int2 = content.GetInt(SpectatorToPlayerRequest.KEY_USER_ID);
                int @short = (int)content.GetShort(SpectatorToPlayerRequest.KEY_PLAYER_ID);
                User userById = this.sfs.UserManager.GetUserById(int2);
                Room roomById = this.sfs.RoomManager.GetRoomById(@int);
                if (roomById != null)
                {
                    if (userById != null)
                    {
                        if (userById.IsJoinedInRoom(roomById))
                        {
                            userById.SetPlayerId(@short, roomById);
                            hashtable["room"] = roomById;
                            hashtable["user"] = userById;
                            hashtable["playerId"] = @short;
                            this.sfs.DispatchEvent(new SFSEvent(SFSEvent.SPECTATOR_TO_PLAYER, hashtable));
                        }
                        else
                        {
                            this.log.Warn(new string[]
							{
								string.Concat(new object[]
								{
									"User: ",
									userById,
									" not joined in Room: ",
									roomById,
									", SpectatorToPlayer failed."
								})
							});
                        }
                    }
                    else
                    {
                        this.log.Warn(new string[]
						{
							"User not found, ID:" + int2 + ", SpectatorToPlayer failed."
						});
                    }
                }
                else
                {
                    this.log.Warn(new string[]
					{
						"Room not found, ID:" + @int + ", SpectatorToPlayer failed."
					});
                }
            }
            else
            {
                short short2 = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)short2, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = short2;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.SPECTATOR_TO_PLAYER_ERROR, hashtable));
            }
        }
        private void FnPlayerToSpectator(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                int @int = content.GetInt(PlayerToSpectatorRequest.KEY_ROOM_ID);
                int int2 = content.GetInt(PlayerToSpectatorRequest.KEY_USER_ID);
                User userById = this.sfs.UserManager.GetUserById(int2);
                Room roomById = this.sfs.RoomManager.GetRoomById(@int);
                if (roomById != null)
                {
                    if (userById != null)
                    {
                        if (userById.IsJoinedInRoom(roomById))
                        {
                            userById.SetPlayerId(-1, roomById);
                            hashtable["room"] = roomById;
                            hashtable["user"] = userById;
                            this.sfs.DispatchEvent(new SFSEvent(SFSEvent.PLAYER_TO_SPECTATOR, hashtable));
                        }
                        else
                        {
                            this.log.Warn(new string[]
							{
								string.Concat(new object[]
								{
									"User: ",
									userById,
									" not joined in Room: ",
									roomById,
									", PlayerToSpectator failed."
								})
							});
                        }
                    }
                    else
                    {
                        this.log.Warn(new string[]
						{
							"User not found, ID:" + int2 + ", PlayerToSpectator failed."
						});
                    }
                }
                else
                {
                    this.log.Warn(new string[]
					{
						"Room not found, ID:" + @int + ", PlayerToSpectator failed."
					});
                }
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.PLAYER_TO_SPECTATOR_ERROR, hashtable));
            }
        }
        private void FnInitBuddyList(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                ISFSArray sFSArray = content.GetSFSArray(InitBuddyListRequest.KEY_BLIST);
                ISFSArray sFSArray2 = content.GetSFSArray(InitBuddyListRequest.KEY_MY_VARS);
                string[] utfStringArray = content.GetUtfStringArray(InitBuddyListRequest.KEY_BUDDY_STATES);
                this.sfs.BuddyManager.ClearAll();
                for (int i = 0; i < sFSArray.Size(); i++)
                {
                    Buddy buddy = SFSBuddy.FromSFSArray(sFSArray.GetSFSArray(i));
                    this.sfs.BuddyManager.AddBuddy(buddy);
                }
                if (utfStringArray != null)
                {
                    this.sfs.BuddyManager.BuddyStates = new List<string>(utfStringArray);
                }
                List<BuddyVariable> list = new List<BuddyVariable>();
                for (int j = 0; j < sFSArray2.Size(); j++)
                {
                    list.Add(SFSBuddyVariable.FromSFSArray(sFSArray2.GetSFSArray(j)));
                }
                this.sfs.BuddyManager.MyVariables = list;
                this.sfs.BuddyManager.Inited = true;
                hashtable["buddyList"] = this.sfs.BuddyManager.BuddyList;
                hashtable["myVariables"] = this.sfs.BuddyManager.MyVariables;
                this.sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_LIST_INIT, hashtable));
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ERROR, hashtable));
            }
        }
        private void FnAddBuddy(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                Buddy buddy = SFSBuddy.FromSFSArray(content.GetSFSArray(AddBuddyRequest.KEY_BUDDY_NAME));
                this.sfs.BuddyManager.AddBuddy(buddy);
                hashtable["buddy"] = buddy;
                this.sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ADD, hashtable));
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ERROR, hashtable));
            }
        }
        private void FnRemoveBuddy(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                string utfString = content.GetUtfString(RemoveBuddyRequest.KEY_BUDDY_NAME);
                Buddy buddy = this.sfs.BuddyManager.RemoveBuddyByName(utfString);
                if (buddy != null)
                {
                    hashtable["buddy"] = buddy;
                    this.sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_REMOVE, hashtable));
                }
                else
                {
                    this.log.Warn(new string[]
					{
						"RemoveBuddy failed, buddy not found: " + utfString
					});
                }
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ERROR, hashtable));
            }
        }
        private void FnBlockBuddy(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                string utfString = content.GetUtfString(BlockBuddyRequest.KEY_BUDDY_NAME);
                Buddy buddyByName = this.sfs.BuddyManager.GetBuddyByName(utfString);
                if (buddyByName != null)
                {
                    buddyByName.IsBlocked = content.GetBool(BlockBuddyRequest.KEY_BUDDY_BLOCK_STATE);
                    hashtable["buddy"] = buddyByName;
                    this.sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_BLOCK, hashtable));
                }
                else
                {
                    this.log.Warn(new string[]
					{
						"BlockBuddy failed, buddy not found: " + utfString + ", in local BuddyList"
					});
                }
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ERROR, hashtable));
            }
        }
        private void FnGoOnline(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                string utfString = content.GetUtfString(GoOnlineRequest.KEY_BUDDY_NAME);
                Buddy buddyByName = this.sfs.BuddyManager.GetBuddyByName(utfString);
                bool flag = utfString == this.sfs.MySelf.Name;
                int @byte = (int)content.GetByte(GoOnlineRequest.KEY_ONLINE);
                bool flag2 = @byte == 0;
                bool flag3 = true;
                if (flag)
                {
                    if (this.sfs.BuddyManager.MyOnlineState != flag2)
                    {
                        this.log.Warn(new string[]
						{
							"Unexpected: MyOnlineState is not in synch with the server. Resynching: " + flag2
						});
                        this.sfs.BuddyManager.MyOnlineState = flag2;
                    }
                }
                else
                {
                    if (buddyByName == null)
                    {
                        this.log.Warn(new string[]
						{
							"GoOnline error, buddy not found: " + utfString + ", in local BuddyList."
						});
                        return;
                    }
                    buddyByName.Id = content.GetInt(GoOnlineRequest.KEY_BUDDY_ID);
                    BuddyVariable variable = new SFSBuddyVariable(ReservedBuddyVariables.BV_ONLINE, flag2);
                    buddyByName.SetVariable(variable);
                    if (@byte == 2)
                    {
                        buddyByName.ClearVolatileVariables();
                    }
                    flag3 = this.sfs.BuddyManager.MyOnlineState;
                }
                if (flag3)
                {
                    hashtable["buddy"] = buddyByName;
                    hashtable["isItMe"] = flag;
                    this.sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ONLINE_STATE_UPDATE, hashtable));
                }
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ERROR, hashtable));
            }
        }
        private void FnReconnectionFailure(IMessage msg)
        {
            this.sfs.HandleReconnectionFailure();
        }
        private void FnSetBuddyVariables(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                string utfString = content.GetUtfString(SetBuddyVariablesRequest.KEY_BUDDY_NAME);
                ISFSArray sFSArray = content.GetSFSArray(SetBuddyVariablesRequest.KEY_BUDDY_VARS);
                Buddy buddyByName = this.sfs.BuddyManager.GetBuddyByName(utfString);
                bool flag = utfString == this.sfs.MySelf.Name;
                List<string> list = new List<string>();
                List<BuddyVariable> list2 = new List<BuddyVariable>();
                bool flag2 = true;
                for (int i = 0; i < sFSArray.Size(); i++)
                {
                    BuddyVariable buddyVariable = SFSBuddyVariable.FromSFSArray(sFSArray.GetSFSArray(i));
                    list2.Add(buddyVariable);
                    list.Add(buddyVariable.Name);
                }
                if (flag)
                {
                    this.sfs.BuddyManager.MyVariables = list2;
                }
                else
                {
                    if (buddyByName == null)
                    {
                        this.log.Warn(new string[]
						{
							"Unexpected. Target of BuddyVariables update not found: " + utfString
						});
                        return;
                    }
                    buddyByName.SetVariables(list2);
                    flag2 = this.sfs.BuddyManager.MyOnlineState;
                }
                if (flag2)
                {
                    hashtable["isItMe"] = flag;
                    hashtable["changedVars"] = list;
                    hashtable["buddy"] = buddyByName;
                    this.sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_VARIABLES_UPDATE, hashtable));
                }
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ERROR, hashtable));
            }
        }
        private void FnFindRooms(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            ISFSArray sFSArray = content.GetSFSArray(FindRoomsRequest.KEY_FILTERED_ROOMS);
            List<Room> list = new List<Room>();
            for (int i = 0; i < sFSArray.Size(); i++)
            {
                Room room = SFSRoom.FromSFSArray(sFSArray.GetSFSArray(i));
                Room roomById = this.sfs.RoomManager.GetRoomById(room.Id);
                if (roomById != null)
                {
                    room.IsJoined = roomById.IsJoined;
                }
                list.Add(room);
            }
            hashtable["rooms"] = list;
            this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_FIND_RESULT, hashtable));
        }
        private void FnFindUsers(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            ISFSArray sFSArray = content.GetSFSArray(FindUsersRequest.KEY_FILTERED_USERS);
            List<User> list = new List<User>();
            User mySelf = this.sfs.MySelf;
            for (int i = 0; i < sFSArray.Size(); i++)
            {
                User user = SFSUser.FromSFSArray(sFSArray.GetSFSArray(i));
                if (user.Id == mySelf.Id)
                {
                    user = mySelf;
                }
                list.Add(user);
            }
            hashtable["users"] = list;
            this.sfs.DispatchEvent(new SFSEvent(SFSEvent.USER_FIND_RESULT, hashtable));
        }
        private void FnInviteUsers(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            User inviter;
            if (content.ContainsKey(InviteUsersRequest.KEY_USER_ID))
            {
                inviter = this.sfs.UserManager.GetUserById(content.GetInt(InviteUsersRequest.KEY_USER_ID));
            }
            else
            {
                inviter = SFSUser.FromSFSArray(content.GetSFSArray(InviteUsersRequest.KEY_USER));
            }
            int @short = (int)content.GetShort(InviteUsersRequest.KEY_TIME);
            int @int = content.GetInt(InviteUsersRequest.KEY_INVITATION_ID);
            ISFSObject sFSObject = content.GetSFSObject(InviteUsersRequest.KEY_PARAMS);
            hashtable["invitation"] = new SFSInvitation(inviter, this.sfs.MySelf, @short, sFSObject)
            {
                Id = @int
            };
            this.sfs.DispatchEvent(new SFSEvent(SFSEvent.INVITATION, hashtable));
        }
        private void FnInvitationReply(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                User value;
                if (content.ContainsKey(InviteUsersRequest.KEY_USER_ID))
                {
                    value = this.sfs.UserManager.GetUserById(content.GetInt(InviteUsersRequest.KEY_USER_ID));
                }
                else
                {
                    value = SFSUser.FromSFSArray(content.GetSFSArray(InviteUsersRequest.KEY_USER));
                }
                int @byte = (int)content.GetByte(InviteUsersRequest.KEY_REPLY_ID);
                ISFSObject sFSObject = content.GetSFSObject(InviteUsersRequest.KEY_PARAMS);
                hashtable["invitee"] = value;
                hashtable["reply"] = @byte;
                hashtable["data"] = sFSObject;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.INVITATION_REPLY, hashtable));
            }
            else
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.INVITATION_REPLY_ERROR, hashtable));
            }
        }
        private void FnQuickJoinGame(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            if (content.ContainsKey(BaseRequest.KEY_ERROR_CODE))
            {
                short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                string errorMessage = SFSErrorCodes.GetErrorMessage((int)@short, this.sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
                hashtable["errorMessage"] = errorMessage;
                hashtable["errorCode"] = @short;
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_JOIN_ERROR, hashtable));
            }
        }
        private void FnPingPong(IMessage msg)
        {
            int num = this.sfs.LagMonitor.OnPingPong();
            Hashtable hashtable = new Hashtable();
            hashtable["lagValue"] = num;
            this.sfs.DispatchEvent(new SFSEvent(SFSEvent.PING_PONG, hashtable));
        }
        private void FnSetUserPosition(IMessage msg)
        {
            Hashtable hashtable = new Hashtable();
            ISFSObject content = msg.Content;
            int @int = content.GetInt(SetUserPositionRequest.KEY_ROOM);
            int[] intArray = content.GetIntArray(SetUserPositionRequest.KEY_MINUS_USER_LIST);
            ISFSArray sFSArray = content.GetSFSArray(SetUserPositionRequest.KEY_PLUS_USER_LIST);
            int[] intArray2 = content.GetIntArray(SetUserPositionRequest.KEY_MINUS_ITEM_LIST);
            ISFSArray sFSArray2 = content.GetSFSArray(SetUserPositionRequest.KEY_PLUS_ITEM_LIST);
            Room roomById = this.sfs.RoomManager.GetRoomById(@int);
            List<User> list = new List<User>();
            List<User> list2 = new List<User>();
            List<IMMOItem> list3 = new List<IMMOItem>();
            List<IMMOItem> list4 = new List<IMMOItem>();
            if (intArray != null && intArray.Length > 0)
            {
                int[] array = intArray;
                for (int i = 0; i < array.Length; i++)
                {
                    int num = array[i];
                    User userById = roomById.GetUserById(num);
                    this.log.Warn(new string[]
					{
						string.Concat(new object[]
						{
							"--------------------> User with id ",
							num,
							" returned null? ",
							userById == null,
							" RoomSize: ",
							roomById.UserList.Count,
							", Room: ",
							roomById.Name
						})
					});
                    if (userById != null)
                    {
                        roomById.RemoveUser(userById);
                        list2.Add(userById);
                    }
                }
            }
            if (sFSArray != null)
            {
                for (int j = 0; j < sFSArray.Size(); j++)
                {
                    ISFSArray sFSArray3 = sFSArray.GetSFSArray(j);
                    User orCreateUser = this.GetOrCreateUser(sFSArray3, true, roomById);
                    list.Add(orCreateUser);
                    roomById.AddUser(orCreateUser);
                    this.log.Warn(new string[]
					{
						"-------------------> ADDED USER: " + orCreateUser
					});
                    this.log.Warn(new string[]
					{
						string.Concat(new object[]
						{
							"-------------------> ROOM: ",
							roomById.Name,
							", SIZE: ",
							roomById.UserList.Count
						})
					});
                    object elementAt = sFSArray3.GetElementAt(5);
                    if (elementAt != null)
                    {
                        (orCreateUser as SFSUser).AOIEntryPoint = Vec3D.fromArray(elementAt);
                    }
                }
            }
            MMORoom mMORoom = roomById as MMORoom;
            if (intArray2 != null)
            {
                int[] array2 = intArray2;
                for (int k = 0; k < array2.Length; k++)
                {
                    int id = array2[k];
                    IMMOItem mMOItem = mMORoom.GetMMOItem(id);
                    if (mMOItem != null)
                    {
                        mMORoom.RemoveItem(id);
                        list4.Add(mMOItem);
                    }
                }
            }
            if (sFSArray2 != null)
            {
                for (int l = 0; l < sFSArray2.Size(); l++)
                {
                    ISFSArray sFSArray4 = sFSArray2.GetSFSArray(l);
                    IMMOItem iMMOItem = MMOItem.FromSFSArray(sFSArray4);
                    list3.Add(iMMOItem);
                    mMORoom.AddMMOItem(iMMOItem);
                    object elementAt2 = sFSArray4.GetElementAt(2);
                    if (elementAt2 != null)
                    {
                        (iMMOItem as MMOItem).AOIEntryPoint = Vec3D.fromArray(elementAt2);
                    }
                }
            }
            hashtable["addedItems"] = list3;
            hashtable["removedItems"] = list4;
            hashtable["removedUsers"] = list2;
            hashtable["addedUsers"] = list;
            hashtable["room"] = (roomById as MMORoom);
            this.sfs.DispatchEvent(new SFSEvent(SFSEvent.PROXIMITY_LIST_UPDATE, hashtable));
        }
        private void FnSetMMOItemVariables(IMessage msg)
        {
            ISFSObject content = msg.Content;
            Hashtable hashtable = new Hashtable();
            int @int = content.GetInt(SetMMOItemVariables.KEY_ROOM_ID);
            int int2 = content.GetInt(SetMMOItemVariables.KEY_ITEM_ID);
            ISFSArray sFSArray = content.GetSFSArray(SetMMOItemVariables.KEY_VAR_LIST);
            MMORoom mMORoom = this.sfs.GetRoomById(@int) as MMORoom;
            List<string> list = new List<string>();
            if (mMORoom != null)
            {
                IMMOItem mMOItem = mMORoom.GetMMOItem(int2);
                if (mMOItem != null)
                {
                    for (int i = 0; i < sFSArray.Size(); i++)
                    {
                        IMMOItemVariable iMMOItemVariable = MMOItemVariable.FromSFSArray(sFSArray.GetSFSArray(i));
                        mMOItem.SetVariable(iMMOItemVariable);
                        list.Add(iMMOItemVariable.Name);
                    }
                    hashtable["changedVars"] = list;
                    hashtable["mmoItem"] = mMOItem;
                    hashtable["room"] = mMORoom;
                    this.sfs.DispatchEvent(new SFSEvent(SFSEvent.MMOITEM_VARIABLES_UPDATE, hashtable));
                }
            }
        }
        private void PopulateRoomList(ISFSArray roomList)
        {
            IRoomManager roomManager = this.sfs.RoomManager;
            for (int i = 0; i < roomList.Size(); i++)
            {
                ISFSArray sFSArray = roomList.GetSFSArray(i);
                Room room = SFSRoom.FromSFSArray(sFSArray);
                roomManager.ReplaceRoom(room);
            }
        }
        private void FnGenericMessage(IMessage msg)
        {
            ISFSObject content = msg.Content;
            switch (content.GetByte(GenericMessageRequest.KEY_MESSAGE_TYPE))
            {
                case 0:
                    this.HandlePublicMessage(content);
                    break;
                case 1:
                    this.HandlePrivateMessage(content);
                    break;
                case 2:
                    this.HandleModMessage(content);
                    break;
                case 3:
                    this.HandleAdminMessage(content);
                    break;
                case 4:
                    this.HandleObjectMessage(content);
                    break;
                case 5:
                    this.HandleBuddyMessage(content);
                    break;
            }
        }
        private void HandlePublicMessage(ISFSObject sfso)
        {
            Hashtable hashtable = new Hashtable();
            int @int = sfso.GetInt(GenericMessageRequest.KEY_ROOM_ID);
            Room roomById = this.sfs.RoomManager.GetRoomById(@int);
            if (roomById != null)
            {
                hashtable["room"] = roomById;
                hashtable["sender"] = this.sfs.UserManager.GetUserById(sfso.GetInt(GenericMessageRequest.KEY_USER_ID));
                hashtable["message"] = sfso.GetUtfString(GenericMessageRequest.KEY_MESSAGE);
                hashtable["data"] = sfso.GetSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS);
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.PUBLIC_MESSAGE, hashtable));
            }
            else
            {
                this.log.Warn(new string[]
				{
					"Unexpected, PublicMessage target room doesn't exist. RoomId: " + @int
				});
            }
        }
        public void HandlePrivateMessage(ISFSObject sfso)
        {
            Hashtable hashtable = new Hashtable();
            int @int = sfso.GetInt(GenericMessageRequest.KEY_USER_ID);
            User user = this.sfs.UserManager.GetUserById(@int);
            if (user == null)
            {
                if (!sfso.ContainsKey(GenericMessageRequest.KEY_SENDER_DATA))
                {
                    this.log.Warn(new string[]
					{
						"Unexpected. Private message has no Sender details!"
					});
                    return;
                }
                user = SFSUser.FromSFSArray(sfso.GetSFSArray(GenericMessageRequest.KEY_SENDER_DATA));
            }
            hashtable["sender"] = user;
            hashtable["message"] = sfso.GetUtfString(GenericMessageRequest.KEY_MESSAGE);
            hashtable["data"] = sfso.GetSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS);
            this.sfs.DispatchEvent(new SFSEvent(SFSEvent.PRIVATE_MESSAGE, hashtable));
        }
        public void HandleBuddyMessage(ISFSObject sfso)
        {
            Hashtable hashtable = new Hashtable();
            int @int = sfso.GetInt(GenericMessageRequest.KEY_USER_ID);
            Buddy buddyById = this.sfs.BuddyManager.GetBuddyById(@int);
            hashtable["isItMe"] = (this.sfs.MySelf.Id == @int);
            hashtable["buddy"] = buddyById;
            hashtable["message"] = sfso.GetUtfString(GenericMessageRequest.KEY_MESSAGE);
            hashtable["data"] = sfso.GetSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS);
            this.sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_MESSAGE, hashtable));
        }
        public void HandleModMessage(ISFSObject sfso)
        {
            this.HandleModMessage(sfso, SFSEvent.MODERATOR_MESSAGE);
        }
        public void HandleAdminMessage(ISFSObject sfso)
        {
            this.HandleModMessage(sfso, SFSEvent.ADMIN_MESSAGE);
        }
        private void HandleModMessage(ISFSObject sfso, string evt)
        {
            Hashtable hashtable = new Hashtable();
            hashtable["sender"] = SFSUser.FromSFSArray(sfso.GetSFSArray(GenericMessageRequest.KEY_SENDER_DATA));
            hashtable["message"] = sfso.GetUtfString(GenericMessageRequest.KEY_MESSAGE);
            hashtable["data"] = sfso.GetSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS);
            this.sfs.DispatchEvent(new SFSEvent(evt, hashtable));
        }
        public void HandleObjectMessage(ISFSObject sfso)
        {
            Hashtable hashtable = new Hashtable();
            int @int = sfso.GetInt(GenericMessageRequest.KEY_USER_ID);
            hashtable["sender"] = this.sfs.UserManager.GetUserById(@int);
            hashtable["message"] = sfso.GetSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS);
            this.sfs.DispatchEvent(new SFSEvent(SFSEvent.OBJECT_MESSAGE, hashtable));
        }
    }
}
