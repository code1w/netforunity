using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;
using KaiGeX;
using KaiGeX.Core;
using KaiGeX.Entities;
using KaiGeX.Requests;
using KaiGeX.Logging;
using KaiGeX.Requests.MMO;
using KaiGeX.Entities.Data;
using clientmsg;



public class ConnectionGUI : MonoBehaviour {
	
	//----------------------------------------------------------
	// Setup variablesF:\SmartClient\mmo\Assets\\
	//----------------------------------------------------------
	public string LoginServerIP = "192.168.1.103";
	public int LoginServerPort = 1888;
	public string zone = "BasicExamples";
	public GUISkin sfsSkin;
	public LogLevel logLevel = LogLevel.DEBUG;

	// Internal / private variables
	public static KaiGeNet smartFox;
    private KaiGeNet smartFoxGate;
    
	private string username = "";
	private string loginErrorMessage = "";
	private string serverConnectionStatusMessage = "";
	private bool isJoining = false;
	private bool isLogin = false;
	private bool isGate = false;
	
	private const string UNITY_MMO_ROOM = "UnityMMODemo";
    bool bCallSendMsg = false;

   // public UIButton btnLogin = null;
   // public UIButton btnLoginOut = null;

    public static ConnectionGUI instance;
	//----------------------------------------------------------
	// Called when program starts
	//----------------------------------------------------------
	void Start() 
    {
        instance = this;
        DontDestroyOnLoad(this);
		// In a webplayer (or editor in webplayer mode) we need to setup security policy negotiation with the server first 
		if (Application.isWebPlayer) 
        {
			if (!Security.PrefetchSocketPolicy(LoginServerIP, 843, 500))
            {
				Debug.LogError("Security Exception. Policy file load failed!");
			}
		}	
		
		// Lets connect
		smartFox = new KaiGeNet(true);
        smartFoxGate = new KaiGeNet(true);
		Debug.LogWarning("API Version: " + smartFox.Version);
					
		// Register callback delegate

		smartFox.AddEventListener(SFSEvent.CONNECTION, OnConnectionLoginServer);
		smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		smartFox.AddEventListener(SFSEvent.LOGIN, OnConnectionGateServer);
		smartFox.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
		smartFox.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
		smartFox.AddEventListener(SFSEvent.LOGOUT, OnLogout);
		//smartFox.AddLogListener(logLevel, OnDebugMessage);
		smartFox.Connect(LoginServerIP, LoginServerPort);

		//smartFoxGate.AddEventListener(SFSEvent.CONNECTION, OnConnectionGateServer);
		//smartFoxGate.AddEventListener(SFSEvent.RESPONSE_CHANGELEVEL, OnS2C_ResponseChangeLevel);
		//smartFoxGate.AddEventListener(SFSEvent.RESPONSE_CLIENTINIT, OnS2C_ResponseClientInit);



		
	}
	
	//----------------------------------------------------------
	// As Unity is not thread safe, we process the queued up callbacks every physics tick
	//----------------------------------------------------------
	void FixedUpdate() {
        
		if (smartFox != null) {
			smartFox.ProcessEvents();
		}
        
	}

	//----------------------------------------------------------
	// Handle connection response from server
	//----------------------------------------------------------
	public void OnConnectionLoginServer(BaseEvent evt) 
    {
		bool success = (bool)evt.Params["success"];
		string error = (string)evt.Params["errorMessage"];
		if (success) 
        {
           // OutLoginMsg();
			isLogin = true;
			serverConnectionStatusMessage = "Connect to server success!";
			SmartFoxConnection.Connection = smartFox;
			OutLoginGateMsg();
            
    	} else 
        {
			serverConnectionStatusMessage = "Can't connect to server!";
        }
    }
    public void OnConnectionGateServer(BaseEvent evt)
    {
        Debug.Log("----call-OnConnectionGateServer-----");
        bool success = (bool)evt.Params["success"];
        string error = (string)evt.Params["errorMessage"];
        if (success)
        {
			SmartFoxConnection.Connection = smartFox;
			isGate = true;
			serverConnectionStatusMessage = "Connected to server success!";
            OutLoginGateMsg();

        }
        else
        {
            serverConnectionStatusMessage = "Can't connect to server!";
        }

    }


	public void OnConnectionLost(BaseEvent evt) {
		// Reset all internal states so we kick back to login screen
		Debug.Log("OnConnectionLost");
		isJoining = false;
		
		serverConnectionStatusMessage = "Connection was lost, Reason: " + (string)evt.Params["reason"];
	}

    public void OnLoginGateServer(BaseEvent evt)
    {
        
    }

	public void OnLoginLoginServer(BaseEvent evt) 
	{
        //smartFox
        clientmsg.LoginResponse loginresponse = (clientmsg.LoginResponse)evt.Params["protomsg"];
        string gateServerIP = loginresponse.gate_ip;
        uint gateServerPort = loginresponse.gate_port;
        //Debug.Log("Recv GateInfo IP: "  + gateServerIP);
        //Debug.Log("Recv GateInfo Port: " + gateServerPort);
		smartFox.Disconnect();
		smartFox = null;
        smartFoxGate.Connect(gateServerIP, (int)gateServerPort); 


	}
    public void OnLoginGate(BaseEvent evtt)
    {
		isJoining = true;
        OutLoginGateMsg();
		Application.LoadLevel("game");
    }

	public void OnLoginError(BaseEvent evt) 
	{
		Debug.Log("Login error: "+(string)evt.Params["errorMessage"]);
	}
	
	public void OnRoomJoin(BaseEvent evt) 
	{
		Debug.Log("Joined room successfully");

		// Room was joined - lets load the game and remove all the listeners from this component
		smartFox.RemoveAllEventListeners();
		Application.LoadLevel("Game");
        
	}
	
	void OnLogout(BaseEvent evt) {
		Debug.Log("OnLogout");
		isJoining = false;
	}
	
	public void OnDebugMessage(BaseEvent evt) {
		string message = (string)evt.Params["message"];
		Debug.Log("[SFS DEBUG] " + message);
	}


    public  void SendProtoBufMsg(ProtoBuf.IExtensible msg, KaiGeNet fox)
    {
        fox.SendProtobufMsg(msg);
    }

    void OutLoginMsg()
    {
        int messageContentLen = 0;
        clientmsg.LoginRequest msg = new clientmsg.LoginRequest();
        msg.name = "test10";
        msg.pwd = "123456";
        messageContentLen += msg.name.Length;
        messageContentLen += msg.pwd.Length;
        SendProtoBufMsg(msg, smartFox);
        
    }
    void OutLoginGateMsg()
    {
        if (smartFox.IsConnected)
        {
            //clientmsg.LoginGame msg = new clientmsg.LoginGame();
            //SendProtoBufMsg(msg, smartFoxGate);
			Application.LoadLevel("Game");
        }

    }
    void ChangeData()
    {

    }

    public void Login_Server()
    {
		//connect LoginServer
		smartFox.Connect(LoginServerIP, LoginServerPort);
    }

	public void btn_LoginOut()
	{
		smartFoxGate.Disconnect();
	}

	void OnGUI() {
		if (smartFoxGate == null) return;
		GUI.skin = sfsSkin;
		
		// Determine which state we are in and show the GUI accordingly
		if(SmartFoxConnection.Connection != null)
		{
			if(!smartFoxGate.IsConnected)
			{
				DrawMessagePanelGUI("GateServer not connected");
			}
		}
		else if (isJoining) {
			DrawMessagePanelGUI("Joining.....");
		}
		else {
			DrawLoginGUI();
		}
	}
	
	
	// Generic single message panel
	void DrawMessagePanelGUI(string message) {
		// Lets just quickly set up some GUI layout variables
		float panelWidth = 400;
		float panelHeight = 300;
		float panelPosX = Screen.width/2 - panelWidth/2;
		float panelPosY = Screen.height/2 - panelHeight/2;
		
		// Draw the box
		GUILayout.BeginArea(new Rect(panelPosX, panelPosY, panelWidth, panelHeight));
		GUILayout.Box ("Object Movement Example", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		GUILayout.BeginVertical();
		GUILayout.BeginArea(new Rect(20, 25, panelWidth-40, panelHeight-60), GUI.skin.customStyles[0]);
		
		// Center label
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		
		GUILayout.Label(message);
		
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea ();		
		
		GUILayout.BeginArea(new Rect(20, panelHeight-30, panelWidth-40, 80));
		// Display client status
		GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.label);
		centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
		
		GUILayout.Label("Client Status: " + serverConnectionStatusMessage, centeredLabelStyle);
		
		GUILayout.EndArea ();		
		GUILayout.EndVertical();
		GUILayout.EndArea ();		
	}
	
	// Login GUI allowing for username, password and zone selection
	private void DrawLoginGUI() {
		// Lets just quickly set up some GUI layout variables
		float panelWidth = 400;
		float panelHeight = 300;
		float panelPosX = Screen.width/2 - panelWidth/2;
		float panelPosY = Screen.height/2 - panelHeight/2;
		
		// Draw the box
		GUILayout.BeginArea(new Rect(panelPosX, panelPosY, panelWidth, panelHeight));
		GUILayout.Box ("Object Movement Login", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		GUILayout.BeginVertical();
		GUILayout.BeginArea(new Rect(20, 25, panelWidth-40, panelHeight-60), GUI.skin.customStyles[0]);
		
		// Lets show login box!
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Username: ");
		username = GUILayout.TextField(username, 25, GUILayout.MinWidth(200));
		GUILayout.EndHorizontal();
		
		GUILayout.Label(loginErrorMessage);
		
		// Center login button
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();		
		if (GUILayout.Button("Login")  || (Event.current.type == EventType.keyDown && Event.current.character == '\n')) {
			Debug.Log("Sending login request");
			OutLoginMsg();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		
		GUILayout.EndArea ();		
		
		GUILayout.BeginArea(new Rect(20, panelHeight-30, panelWidth-40, 80));
		// Display client status
		GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.label);
		centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
		
		GUILayout.Label("Client Status: " + serverConnectionStatusMessage, centeredLabelStyle);
		
		GUILayout.EndArea ();		
		GUILayout.EndVertical();
		GUILayout.EndArea ();		
	}

    void OnS2C_ResponseChangeLevel(BaseEvent evt)
    {
       // clientmsg.ResponseChangeLevel responsechangelevel = (clientmsg.ResponseChangeLevel)evt.Params["protomsg"];
    
//         JsonData jdpos = JsonMapper.ToObject(responsechangelevel.monsterpos);
//         JsonData jdgroup = JsonMapper.ToObject(responsechangelevel.monstergroup);
// 
//         Debug.Log("jdpos" + jdpos[0]["originx"] + " ..... " + jdpos[0]["originy"]);
//         int x = int.Parse(jdpos[0]["originx"].ToString());
//         int y = int.Parse(jdpos[0]["originy"].ToString());
//         int count = int.Parse(jdpos[0]["quantity"].ToString());
// 
//         for (int i = 0; i < count; i++ )
//         {
//             int posx = int.Parse(jdgroup[i]["posX"].ToString());
//             int posy = int.Parse(jdgroup[i]["posY"].ToString());
//             string name = jdgroup[i]["type"].ToString();
//         } 
    }

    void OnS2C_ResponseClientInit(BaseEvent evt)
    {
        clientmsg.ClientInit clientinit = (clientmsg.ClientInit)evt.Params["protomsg"];
        Debug.Log(clientinit.GetType().FullName.ToString());
    }
    
}