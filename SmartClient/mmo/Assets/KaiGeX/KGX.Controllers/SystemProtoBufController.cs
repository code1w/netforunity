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
    public class SystemProtoBufController : BaseController
    {
        private Dictionary<string, RequestProtoBufDelegate> requestHandlers;
        public SystemProtoBufController(BitSwarmClient bitSwarm)
            : base(bitSwarm)
        {
            this.requestHandlers = new Dictionary<string, RequestProtoBufDelegate>();
            this.InitRequestHandlers();
        }
        private void InitRequestHandlers()
        {
            this.requestHandlers["clientmsg.LoginResponse"] = new RequestProtoBufDelegate(this.OnS2C_LoginResponse);
			this.requestHandlers["clientmsg.ResponseLevelMonster"] = new RequestProtoBufDelegate(this.OnS2C_ResponseChangeLevel);
            this.requestHandlers["clientmsg.ClientInit"] = new RequestProtoBufDelegate(this.OnS2C_ResponseClientInit);
            this.requestHandlers["clientmsg.ResponseLoginGame"] = new RequestProtoBufDelegate(this.OnS2C_ResponseLoginGame);
            this.requestHandlers["clientmsg.ResponseLoginReward"] = new RequestProtoBufDelegate(this.OnS2CResponseLoginReward);
            this.requestHandlers["clientmsg.ResponseCharPackage"] = new RequestProtoBufDelegate(this.OnS2C_ResponseCharPackage);
            this.requestHandlers["clientmsg.ResponseChangePackage"] = new RequestProtoBufDelegate(this.OnS2C_ResponseChangePackage);
            this.requestHandlers["clientmsg.ResponseConfigData"] = new RequestProtoBufDelegate(this.OnS2C_ResponseConfigData);
            this.requestHandlers["clientmsg.ResponseSkillbuff"] = new RequestProtoBufDelegate(this.OnS2C_ResponseSkillBuff);
            this.requestHandlers["clientmsg.ResponseSkill"] = new RequestProtoBufDelegate(this.OnS2C_ResponseSkill);
            this.requestHandlers["clientmsg.ResponseCumLoginRewardData"] = new RequestProtoBufDelegate(this.OnS2C_ResponseCumLoginRewardData);
            this.requestHandlers["clientmsg.ResponseStageData"] = new RequestProtoBufDelegate(this.OnS2C_ResponseStageData);
            this.requestHandlers["clientmsg.ResponseFanPaiItemData"] = new RequestProtoBufDelegate(this.OnS2C_ResponseFanPaiItemData);
            this.requestHandlers["clientmsg.MapScreenRefreshCharacter"] = new RequestProtoBufDelegate(this.OnS2C_MapScreenRefreshCharacter);
            this.requestHandlers["clientmsg.NineScreenRefreshPlayer"] = new RequestProtoBufDelegate(this.OnS2C_NineScreenRefreshPlayer);

        }

        /// <summary>
        /// 重载消息分发器
        /// </summary>
        /// <param name="message"></param>
        public override void HandleMessage(ProtoBuf.IExtensible message)
        {
            if (this.sfs.Debug)
            {
                this.log.Info(new string[]
				{
					string.Concat(new object[]
					{
						"Message: ",
						message.GetType().FullName,
						" ",
						message
					})
				});
            }
            if (!this.requestHandlers.ContainsKey(message.GetType().FullName))
            {
                this.log.Warn(new string[]
				{
					"Unknown message name: " + message.GetType().FullName
				});
            }
            else
            {
                RequestProtoBufDelegate requestDelegate = this.requestHandlers[message.GetType().FullName];
                requestDelegate(message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        private void OnS2C_LoginResponse(ProtoBuf.IExtensible msg)
        {
            clientmsg.LoginResponse loginResponse = msg as clientmsg.LoginResponse;
            UnityEngine.Debug.Log("SystemProtoBufController------>OnS2C_LoginResponse()");

            Hashtable hashtable = new Hashtable();
            if (loginResponse.gate_ip.Length != 0)
            {
                hashtable["protomsg"] = loginResponse;
                SFSEvent evt = new SFSEvent(SFSEvent.LOGIN, hashtable);
                this.sfs.DispatchEvent(evt);
            }
            else
            {

                hashtable["errorMessage"] = "Gate ip is null";
                hashtable["errorCode"] = "0";
                this.sfs.DispatchEvent(new SFSEvent(SFSEvent.LOGIN_ERROR, hashtable));
            }
        }

        private void OnS2C_ResponseChangeLevel(ProtoBuf.IExtensible msg)
        {
            UnityEngine.Debug.Log("SystemProtoBufController------>OnS2C_ResponseChangeLevel()");
			clientmsg.ResponseLevelMonster responseChangeLevel = msg as clientmsg.ResponseLevelMonster;
            Hashtable hashtable = new Hashtable();
            //if (responseChangeLevel.levelid != 0 && responseChangeLevel.landingid != 0 && responseChangeLevel.enemygroup != 0)
            //{
	        hashtable["protomsg"] = responseChangeLevel;
	        SFSEvent evt = new SFSEvent(SFSEvent.RESPONSE_CHANGELEVEL, hashtable);
	        this.sfs.DispatchEvent(evt);
            //}
        }
        private void OnS2C_ResponseClientInit(ProtoBuf.IExtensible msg)
        {
            clientmsg.ClientInit clientinit = msg as clientmsg.ClientInit;
            Hashtable hashtable = new Hashtable();
            hashtable["protomsg"] = clientinit;
            SFSEvent evt = new SFSEvent(SFSEvent.RESPONSE_CLIENTINIT, hashtable);
            this.sfs.DispatchEvent(evt);
        }

        private void OnS2C_ResponseLoginGame(ProtoBuf.IExtensible msg)
        {
            clientmsg.ResponseLoginGame logingame = msg as clientmsg.ResponseLoginGame;
            Hashtable hashtable = new Hashtable();

            hashtable["protomsg"] = logingame;
            SFSEvent evt = new SFSEvent(SFSEvent.RESPONSE_LOGINGAME, hashtable);
            this.sfs.DispatchEvent(evt);
        }

        private void OnS2CResponseLoginReward(ProtoBuf.IExtensible msg)
        {
            clientmsg.ResponseLoginReward loginreward = msg as clientmsg.ResponseLoginReward;
            Hashtable hashtable = new Hashtable();

            hashtable["protomsg"] = loginreward;
            SFSEvent evt = new SFSEvent(SFSEvent.RESPONSE_LOGINREWARD, hashtable);
            this.sfs.DispatchEvent(evt);
        }

        private void OnS2C_ResponseCharPackage(ProtoBuf.IExtensible msg)
        {
            clientmsg.ResponseCharPackage charpackage = msg as clientmsg.ResponseCharPackage;
            Hashtable hashtable = new Hashtable();

            hashtable["protomsg"] = charpackage;
            SFSEvent evt = new SFSEvent(SFSEvent.RESPONSE_CHARPACKAGE, hashtable);
            this.sfs.DispatchEvent(evt);
        }

        private void OnS2C_ResponseChangePackage(ProtoBuf.IExtensible msg)
        {
            clientmsg.ResponseChangePackage changepackage = msg as clientmsg.ResponseChangePackage;
            Hashtable hashtable = new Hashtable();

            hashtable["protomsg"] = changepackage;
            SFSEvent evt = new SFSEvent(SFSEvent.RESPONSE_CHANGEPACKAGE, hashtable);
            this.sfs.DispatchEvent(evt);
        }

        private void OnS2C_ResponseSkillBuff(ProtoBuf.IExtensible msg)
        {
            clientmsg.ResponseSkillbuff skillbuff = msg as clientmsg.ResponseSkillbuff;
            Hashtable hashtable = new Hashtable();
            UnityEngine.Debug.Log("OnS2C_ResponseSkillBuff");
            hashtable["protomsg"] = skillbuff;
            SFSEvent evt = new SFSEvent(SFSEvent.RESPONSE_SKILLBUFF, hashtable);
            this.sfs.DispatchEvent(evt);
        }

        private void OnS2C_ResponseSkill(ProtoBuf.IExtensible msg)
        {
            clientmsg.ResponseSkill skill = msg as clientmsg.ResponseSkill;
            Hashtable hashtable = new Hashtable();
            UnityEngine.Debug.Log("OnS2C_ResponseSkill");
            hashtable["protomsg"] = skill;
            SFSEvent evt = new SFSEvent(SFSEvent.RESPONSE_SKILL, hashtable);
            this.sfs.DispatchEvent(evt);
        }

        private void OnS2C_ResponseConfigData(ProtoBuf.IExtensible msg)
        {
            clientmsg.ResponseConfigData configdata = msg as clientmsg.ResponseConfigData;
            Hashtable hashtable = new Hashtable();
            hashtable["protomsg"] = configdata;
            SFSEvent evt = new SFSEvent(SFSEvent.RESPONSE_CONFIGDATA, hashtable);
            this.sfs.DispatchEvent(evt);
        }

        private void OnS2C_ResponseCumLoginRewardData(ProtoBuf.IExtensible msg)
        {
            clientmsg.ResponseCumLoginRewardData rewarddata = msg as clientmsg.ResponseCumLoginRewardData;
            Hashtable hashtable = new Hashtable();
            hashtable["protomsg"] = rewarddata;
            SFSEvent evt = new SFSEvent(SFSEvent.RESPONSE_CUMLOGINREWARDDATA, hashtable);
            this.sfs.DispatchEvent(evt);
        }

        private void OnS2C_ResponseStageData(ProtoBuf.IExtensible msg)
        {
            clientmsg.ResponseStageData stagedata = msg as clientmsg.ResponseStageData;
            Hashtable hashtable = new Hashtable();
            hashtable["protomsg"] = stagedata;
            SFSEvent evt = new SFSEvent(SFSEvent.RESPONSE_STAGEDATA, hashtable);
            this.sfs.DispatchEvent(evt);
        }

        private void OnS2C_ResponseFanPaiItemData(ProtoBuf.IExtensible msg)
        {
            clientmsg.ResponseFanPaiItemData fanpaiitemdata = msg as clientmsg.ResponseFanPaiItemData;
            Hashtable hashtable = new Hashtable();
            hashtable["protomsg"] = fanpaiitemdata;
            SFSEvent evt = new SFSEvent(SFSEvent.RESPONSE_FANPAIITEMDATA, hashtable);
            this.sfs.DispatchEvent(evt);
        }

        private void OnS2C_MapScreenRefreshCharacter(ProtoBuf.IExtensible msg)
        {
            clientmsg.MapScreenRefreshCharacter refreshcharacter = msg as clientmsg.MapScreenRefreshCharacter;
            Hashtable hashtable = new Hashtable();
            hashtable["protomsg"] = refreshcharacter;
            SFSEvent evt = new SFSEvent(SFSEvent.RESPONSE_MAPSCREENREFRESHCHARACTER, hashtable);
            this.sfs.DispatchEvent(evt);
        }

        private void OnS2C_NineScreenRefreshPlayer(ProtoBuf.IExtensible msg)
        {
            clientmsg.NineScreenRefreshPlayer ninerefreshplayer = msg as clientmsg.NineScreenRefreshPlayer;
            Hashtable hashtable = new Hashtable();
            hashtable["protomsg"] = ninerefreshplayer;
            SFSEvent evt = new SFSEvent(SFSEvent.RESPONSE_NINESCREENREFRESHPLAYER, hashtable);
            this.sfs.DispatchEvent(evt);
        }
    }
}

