using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
//这边定义消息
using clientmsg;
using common;
//using LoginHistory;


public interface IMessageHandler
{
    void Process(ProtoBuf.IExtensible msg);
}

public class DefaultMessageHandler : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //默认处理函数, 相当于switch case 操作
        if (msg is clientmsg.LoginResponse)
        {
        }
    }
}
//玩家走出九屏
public class MapRemoveCharHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.mapremovechar = msg as clientmsg.MapRemoveChar;
        //Global.isRemove = true;
    }

}

//角色移动
public class S2CMoveHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        clientmsg.S2CMove move = msg as clientmsg.S2CMove;
       // Global.charmove.Enqueue(move);
    }
}

public class CharMoveFailedHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //clientmsg.CharMoveFailed charmovefailedmsg = msg as clientmsg.CharMoveFailed;
    }
}

public class SendChatHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
       // clientmsg.SendChat sendchatmsg = msg as clientmsg.SendChat;
    }
}

//对时消息
public class S2CProofTimeHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
       // Global.s2cproofTime = msg as clientmsg.S2CProofTime;
       // Global.isTimeTick = true;
    }
}
//批量创建九屏的人
public class NineScreenRefreshPlayerHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.ninescreenrefreshplayer = msg as clientmsg.NineScreenRefreshPlayer;
        //Global.isInst = true;
    }
}
#region 好友列表
//第二步返回玩家好友列表
public class SendUserFriendListHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.sendUserFriendList = msg as clientmsg.SendUserFriendList;
        //Global.refreshFriendList = true;
    }
}

//通知加好友消息
public class AddFriendNotifyHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.addfriendnotifymsg = msg as clientmsg.AddFriendNotify;
        //Global.addFriendNotifyHandle = true;
    }
}

////添加好友结果返回
public class AddFriendResultHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.addfriendresultmsg = msg as clientmsg.AddFriendResult;
        //Global.addFriendResultHandle = true;
    }
}

//获取好友状态
public class NotifyOnlineStateHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.notifyonlinestatemsg = msg as clientmsg.NotifyOnlineState;
        //Global.notifyOnlineStateHandle = true;
    }
}
//自动加好友--事务
public class UserFriendListInfoHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.userFriendListInfo = msg as clientmsg.UserFriendListInfo;
        //Global.userFriendListInfoHandle = true;
    }
}

#endregion
#region 事务系统
public class InformPersonPassHandle : IMessageHandler//公司通过简历通知
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.informPersonPass = msg as clientmsg.InformPersonPass;
        //Global.isInformPersonPass = true;
    }
}
public class InformPersNoPassHandle : IMessageHandler//公司拒绝了你的简历通知
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.informPersNoPass = msg as clientmsg.InformPersNoPass;
        //Global.isInformPersNoPass = true;
    }
}
public class InformPersonWhoCheckHandle : IMessageHandler//谁看了你的简历通知
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.informPersonWhoCheck = msg as clientmsg.InformPersonWhoCheck;
        //Global.isInformPersonWhoCheck = true;
    }
}
public class InformUsedPostHandle : IMessageHandler//已重复投递 通知 对个人
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.informUsedPost = msg as clientmsg.InformUsedPost;
        //Global.isInformUsedPost = true;
    }
}
public class InformCompanyWhoPostHandle : IMessageHandler//谁给你投了简历 对企业
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.informCompanyWhoPost = msg as clientmsg.InformCompanyWhoPost;
        //Global.isInformCompanyWhoPost = true;
    }
}
public class IsOnlineOrFriendHandle : IMessageHandler//人才库点击通过判断是否在线，是否为好友 对企业
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.isOnlineOrFriend = msg as clientmsg.IsOnlineOrFriend;
        //Global.isIsOnlineOrFriend = true;
    }
}
#endregion
#region 切换场景
public class ResponseChangeSceneHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.responseChangeScene = msg as clientmsg.ResponseChangeScene;
        //Global.isResponseChangeScene = true;
    }
}
#endregion
#region 第一次登陆 昵称 是否能用
public class isCreateCharSuccessHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.isCreateCharSuccess = msg as clientmsg.isCreateCharSuccess;
        //Global.isIsCreateCharSuccess = true;
    }
}
#endregion
//世界窗口消息
public class WordWindowHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        // clientmsg.WordWindow wordwindowmsg = msg as clientmsg.WordWindow;
        //Global.worldWindow = msg as clientmsg.WordWindow;
        //Global.isHaveWorldMessage = true;
    }
}
//用于服务器给用户发邮件
public class SendMailContentHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.sendMailContent = msg as clientmsg.SendMailContent;
        //Global.isHaveMail = true;
    }

}
//服务器给用户发展台状态消息
public class BoothStateHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        /*
        Global.boothState = msg as clientmsg.BoothState;
        Global.isHaveBoothState = true;
		
		Global.allBoothState.Clear();
		
        for (int i = 0; i < Global.boothState.states.Count; i++)
        {
            Global.allBoothState.Add(Global.boothState.states[i].boothid, 

Global.boothState.states[i]);
        }
         * * */
    }
         
}

//收到评分消息
public class InformAssessHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        //Global.informAssessRank = msg as clientmsg.InformAssess;

    }
}

//打招呼消息
//占展台
public class HoldBoothHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        /*
         * Global.holdBooth = msg as clientmsg.HoldBooth;
        //所有展台信息中添加刚占用的展台
        if (Global.allBoothState.ContainsKey(Global.holdBooth.boothid) != true)
        {
           Global.allBoothState.Add(Global.holdBooth.boothid, Global.holdBooth);
        }
         * */

    }
}
public class LeaveBoothHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        /*
        Global.leaveBooth = msg as clientmsg.LeaveBooth;
        //所有展台信息中删除不再占用的展台 
        for (int i = 0; i < Global.leaveBooth.boothids.Count; i++)
        {
            if (Global.allBoothState.ContainsKey(Global.leaveBooth.boothids[i]))
            {
               Global.allBoothState.Remove(Global.leaveBooth.boothids[i]);
            }

        }
         * */
    }
}
public class SendGreetHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        /*
        Global.sendGreet = msg as clientmsg.SendGreet;
        Global.isGreet = true;
         * */
    }
}
//换装消息
public class S2CChangeClothHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        /*
        Global.changeCloth = msg as clientmsg.S2CChangeCloth;
        Global.isChange = true;
         * */
    }
}
//换签名消息 类似 换装消息
public class S2CChangeSignatureHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        /*
        Global.S2CChangeSignature = msg as clientmsg.S2CChangeSignature;
        Global.isS2CChangeSignature = true;
         * */
    }
}

//刷新九屏
public class MapScreenRefreshCharacterHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
        /*
        clientmsg.MapScreenRefreshCharacter charmsg = msg as clientmsg.MapScreenRefreshCharacter;
        Global.mapscreenrefreshchar.Enqueue(charmsg);
        Global.userName = charmsg.mapinfo.charinfo.name;
        //Global.isGenderSelect = false;
        //Global.isagain = true;
         * */
    }
}

//从服务器返回消息
public class LoginResponseHandle : IMessageHandler
{
    clientmsg.LoginResponse responsemsg;
    public void Process(ProtoBuf.IExtensible msg)
    {
        responsemsg = msg as clientmsg.LoginResponse;
        
        /*
         * Debug.Log(responsemsg.chartype);
        Global.charType = responsemsg.chartype;//个人1 企业2 游客3 必须最先赋值给global
         * */
        
        LoginCheck(responsemsg.result);
    }

    public void LoginCheck(clientmsg.enumLoginResult msg)
    {
        /*
       // PanelLoginLogic.LoginButtonActive(true);
        switch (msg)
        {
            case clientmsg.enumLoginResult.enumLoginResult_Success:
                //JFSocket.GetInstance().Disconnect();
                JFSocket.GetInstance().ConnectServer(responsemsg.gate_ip, (int)responsemsg.gate_port);
                clientmsg.LoginGame gamemsg = new clientmsg.LoginGame();
                gamemsg.user_account = responsemsg.user_account;
                Global.game_msg.user_account = gamemsg.user_account;

                gamemsg.type = (common.enumCharType)Global.charType;

                JFSocket.GetInstance().SendMessage(gamemsg);
                break;
            case clientmsg.enumLoginResult.enumLoginResult_NameFail:
               // PanelLoginLogic.Instance.UserNameError();
                break;
            case clientmsg.enumLoginResult.enumLoginResult_PwdFail:
               // PanelLoginLogic.Instance.PasswordError();
                break;
            case clientmsg.enumLoginResult.enumLoginResult_HaveLogin:
                //PanelLoginLogic.Instance.HaveLogin();
                UnityEngine.Debug.Log("已经登录");
                break;
            default:
                break;
        }
         * * * */

    }
         
}

public class ClientInitHandle : IMessageHandler
{
    public void Process(ProtoBuf.IExtensible msg)
    {
    }

}

public static class MessageDispatcher
{
    private static readonly IMessageHandler s_DefaultHandler = new DefaultMessageHandler();
    private static readonly Dictionary<Type, IMessageHandler> s_Handlers = new Dictionary<Type, IMessageHandler>();
    private static readonly Dictionary<string, Type> typelist = new Dictionary<string, Type>();

    static MessageDispatcher()
    {
       
        typelist.Add(typeof(clientmsg.LoginResponse).FullName, typeof(clientmsg.LoginResponse));

        typelist.Add(typeof(clientmsg.MapScreenRefreshCharacter).FullName, typeof(clientmsg.MapScreenRefreshCharacter));

        typelist.Add(typeof(clientmsg.NineScreenRefreshPlayer).FullName, typeof(clientmsg.NineScreenRefreshPlayer));
       
        typelist.Add(typeof(clientmsg.S2CMove).FullName, typeof(clientmsg.S2CMove));

        typelist.Add(typeof(clientmsg.MapRemoveChar).FullName, typeof(clientmsg.MapRemoveChar));
 
        typelist.Add(typeof(clientmsg.CharMoveFailed).FullName, typeof(clientmsg.CharMoveFailed));

        typelist.Add(typeof(clientmsg.SendChat).FullName, typeof(clientmsg.SendChat));
  
        typelist.Add(typeof(clientmsg.SendUserFriendList).FullName, typeof(clientmsg.SendUserFriendList));
    
        typelist.Add(typeof(clientmsg.NotifyOnlineState).FullName, typeof(clientmsg.NotifyOnlineState));
     
        typelist.Add(typeof(clientmsg.SendMailContent).FullName, typeof(clientmsg.SendMailContent));
      
        typelist.Add(typeof(clientmsg.AddFriendNotify).FullName, typeof(clientmsg.AddFriendNotify));

        typelist.Add(typeof(clientmsg.AddFriendResult).FullName, typeof(clientmsg.AddFriendResult));

        typelist.Add(typeof(clientmsg.SendGreet).FullName, typeof(clientmsg.SendGreet));

        typelist.Add(typeof(clientmsg.WordWindow).FullName, typeof(clientmsg.WordWindow));

        typelist.Add(typeof(clientmsg.HoldBooth).FullName, typeof(clientmsg.HoldBooth));
    
        typelist.Add(typeof(clientmsg.BoothState).FullName, typeof(clientmsg.BoothState));
  
        typelist.Add(typeof(clientmsg.LeaveBooth).FullName, typeof(clientmsg.LeaveBooth));
  
        typelist.Add(typeof(clientmsg.S2CChangeCloth).FullName, typeof(clientmsg.S2CChangeCloth));

        typelist.Add(typeof(clientmsg.S2CChangeSignature).FullName, typeof(clientmsg.S2CChangeSignature));
   
        typelist.Add(typeof(clientmsg.InformPersonPass).FullName, typeof(clientmsg.InformPersonPass));
 
        typelist.Add(typeof(clientmsg.InformPersNoPass).FullName, typeof(clientmsg.InformPersNoPass));
   
        typelist.Add(typeof(clientmsg.InformPersonWhoCheck).FullName, typeof(clientmsg.InformPersonWhoCheck));
 
        typelist.Add(typeof(clientmsg.InformUsedPost).FullName, typeof(clientmsg.InformUsedPost));

        typelist.Add(typeof(clientmsg.InformCompanyWhoPost).FullName, typeof(clientmsg.InformCompanyWhoPost));
  
        typelist.Add(typeof(clientmsg.IsOnlineOrFriend).FullName, typeof(clientmsg.IsOnlineOrFriend));
  
        typelist.Add(typeof(clientmsg.UserFriendListInfo).FullName, typeof(clientmsg.UserFriendListInfo));
     
        typelist.Add(typeof(clientmsg.isCreateCharSuccess).FullName, typeof(clientmsg.isCreateCharSuccess));

        typelist.Add(typeof(clientmsg.ResponseChangeScene).FullName, typeof(clientmsg.ResponseChangeScene));
   
        typelist.Add(typeof(clientmsg.InformAssess).FullName, typeof(clientmsg.InformAssess));
 
        typelist.Add(typeof(clientmsg.S2CProofTime).FullName, typeof(clientmsg.S2CProofTime));


		typelist.Add(typeof(clientmsg.ResponseLevelMonster).FullName, typeof(clientmsg.ResponseLevelMonster));

        typelist.Add(typeof(clientmsg.ResponseLoginGame).FullName, typeof(clientmsg.ResponseLoginGame));

        typelist.Add(typeof(clientmsg.ClientInit).FullName, typeof(clientmsg.ClientInit));

        typelist.Add(typeof(clientmsg.ResponseLoginReward).FullName, typeof(clientmsg.ResponseLoginReward));
        typelist.Add(typeof(clientmsg.ResponseCharPackage).FullName, typeof(clientmsg.ResponseCharPackage));
        typelist.Add(typeof(clientmsg.ResponseChangePackage).FullName, typeof(clientmsg.ResponseChangePackage));
        typelist.Add(typeof(clientmsg.ResponseSkillbuff).FullName, typeof(clientmsg.ResponseSkillbuff));
        typelist.Add(typeof(clientmsg.ResponseSkill).FullName, typeof(clientmsg.ResponseSkill));
        typelist.Add(typeof(clientmsg.ResponseConfigData).FullName, typeof(clientmsg.ResponseConfigData));
        typelist.Add(typeof(clientmsg.ResponseCumLoginRewardData).FullName, typeof(clientmsg.ResponseCumLoginRewardData));
        typelist.Add(typeof(clientmsg.ResponseStageData).FullName, typeof(clientmsg.ResponseStageData));
        typelist.Add(typeof(clientmsg.ResponseFanPaiItemData).FullName, typeof(clientmsg.ResponseFanPaiItemData));
    }

    public static void Dispatch(ProtoBuf.IExtensible msg)
    {
        Type key = msg.GetType();
        if (s_Handlers.ContainsKey(key))
        {
            // We found a specific handler! :)
            s_Handlers[key].Process(msg);
        }
        else
        {
            // We will have to resort to the default handler. :(
            s_DefaultHandler.Process(msg);
        }
    }
    public static Type getTypeByStr(string name)
    {
        return typelist[name];
    }
}