using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
//using System.Data;
//using System.Linq;
using System.Net;
using System.Reflection;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;
using clientmsg;


public class Connection 
{
	public static string ip = "";
	public static int port = 0;
	public static clientmsg.LoginResponse responsemsg;
	//public static clientmsg.CreateChar info = new clientmsg.CreateChar();
	
   // public MessageTcpClient MessageServer { get; private set; }
    //public MessageDispatcher MessageDispatcher;

    public string ContentPath { get; private set; }

	
    private static Connection _instance;
    public static Connection Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Connection();
            }

            return _instance;
        }
    }

    public void ConnectToServer(string connect_ip, int connect_port)
    {
        bool bre = false;
        bre = Security.PrefetchSocketPolicy("192.168.1.17", 843);
        if (bre)
        {
            UnityEngine.Debug.Log("sucess Security.PrefetchSocketPolicy");
        }
		//JFSocket.GetInstance().ConnectServer(connect_ip, connect_port);
    }

    public void Send(string name, string password)
    {
        clientmsg.LoginRequest msg = new clientmsg.LoginRequest();
        msg.name = name;
        msg.pwd = password;
		//JFSocket.GetInstance().SendMessage(msg);
    }
    public void GuestSend()
    {
        clientmsg.GuestLoginReq guestLoginReq = new clientmsg.GuestLoginReq();
        //JFSocket.GetInstance().SendMessage(guestLoginReq);
 
    }
    void MessageHandler(ProtoBuf.IExtensible message)
    {
        MessageDispatcher.Dispatch(message);
    }

    void OnSocketDisconnect(Exception e)
    {
        if (e != null)
        {
           // GameObject.FindWithTag("UI2D").transform.Find("PanelLogin").GetComponent<PanelLoginLogic>().ConnectionFail_1();
            
        }
        else
        {
           // UnityEngine.Debug.Log("网络正常关闭");
            //GameObject.FindWithTag("UI2D").transform.Find("PanelLogin").GetComponent<PanelLoginLogic>().ConnectionFail_2();
        }
    }

}
