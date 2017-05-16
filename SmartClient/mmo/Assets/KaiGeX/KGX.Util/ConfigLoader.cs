using KaiGeX.Core;
using System;
using System.Collections;
using System.IO;
namespace KaiGeX.Util
{
	public class ConfigLoader : IDispatchable
	{
		private KaiGeNet KaiGeNet;
		private EventDispatcher dispatcher;
		private XMLParser xmlParser;
		private XMLNode rootNode;
		public EventDispatcher Dispatcher
		{
			get
			{
				return this.dispatcher;
			}
		}
		public ConfigLoader(KaiGeNet KaiGeNet)
		{
			this.KaiGeNet = KaiGeNet;
			this.dispatcher = new EventDispatcher(this);
		}
		public void LoadConfig(string filePath)
		{
			try
			{
				StreamReader streamReader = File.OpenText(filePath);
				string content = streamReader.ReadToEnd();
				this.xmlParser = new XMLParser();
				this.rootNode = this.xmlParser.Parse(content);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error loading config file: " + ex.Message);
				this.OnConfigLoadFailure("Error loading config file: " + ex.Message);
				return;
			}
			this.TryParse();
		}
		private string GetNodeText(XMLNode rootNode, string nodeName)
		{
			string result;
			if (rootNode[nodeName] == null)
			{
				result = null;
			}
			else
			{
				result = ((rootNode[nodeName] as XMLNodeList)[0] as XMLNode)["_text"].ToString();
			}
			return result;
		}
		private void TryParse()
		{
			ConfigData configData = new ConfigData();
			try
			{
				XMLNodeList xMLNodeList = this.rootNode["SmartFoxConfig"] as XMLNodeList;
				XMLNode xMLNode = xMLNodeList[0] as XMLNode;
				if (this.GetNodeText(xMLNode, "ip") == null)
				{
					this.KaiGeNet.Log.Error(new string[]
					{
						"Required config node missing: ip"
					});
				}
				if (this.GetNodeText(xMLNode, "port") == null)
				{
					this.KaiGeNet.Log.Error(new string[]
					{
						"Required config node missing: port"
					});
				}
				if (this.GetNodeText(xMLNode, "udpIp") == null)
				{
					this.KaiGeNet.Log.Error(new string[]
					{
						"Required config node missing: udpIp"
					});
				}
				if (this.GetNodeText(xMLNode, "udpPort") == null)
				{
					this.KaiGeNet.Log.Error(new string[]
					{
						"Required config node missing: udpPort"
					});
				}
				if (this.GetNodeText(xMLNode, "zone") == null)
				{
					this.KaiGeNet.Log.Error(new string[]
					{
						"Required config node missing: zone"
					});
				}
				configData.Host = this.GetNodeText(xMLNode, "ip");
				configData.Port = Convert.ToInt32(this.GetNodeText(xMLNode, "port"));
				configData.UdpHost = this.GetNodeText(xMLNode, "udpIp");
				configData.UdpPort = Convert.ToInt32(this.GetNodeText(xMLNode, "udpPort"));
				configData.Zone = this.GetNodeText(xMLNode, "zone");
				if (this.GetNodeText(xMLNode, "debug") != null)
				{
					configData.Debug = (this.GetNodeText(xMLNode, "debug").ToLower() == "true");
				}
				if (this.GetNodeText(xMLNode, "useBlueBox") != null)
				{
					configData.UseBlueBox = (this.GetNodeText(xMLNode, "useBlueBox").ToLower() == "true");
				}
				if (this.GetNodeText(xMLNode, "httpPort") != null && this.GetNodeText(xMLNode, "httpPort") != "")
				{
					configData.HttpPort = Convert.ToInt32(this.GetNodeText(xMLNode, "httpPort"));
				}
				if (this.GetNodeText(xMLNode, "blueBoxPollingRate") != null && this.GetNodeText(xMLNode, "blueBoxPollingRate") != "")
				{
					configData.BlueBoxPollingRate = Convert.ToInt32(this.GetNodeText(xMLNode, "blueBoxPollingRate"));
				}
			}
			catch (Exception ex)
			{
				this.OnConfigLoadFailure("Error parsing config file: " + ex.Message + " " + ex.StackTrace);
				return;
			}
			Hashtable hashtable = new Hashtable();
			hashtable["cfg"] = configData;
			SFSEvent evt = new SFSEvent(SFSEvent.CONFIG_LOAD_SUCCESS, hashtable);
			this.dispatcher.DispatchEvent(evt);
		}
		private void OnConfigLoadFailure(string msg)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["message"] = msg;
			SFSEvent evt = new SFSEvent(SFSEvent.CONFIG_LOAD_FAILURE, hashtable);
			this.dispatcher.DispatchEvent(evt);
		}
		public void AddEventListener(string eventType, EventListenerDelegate listener)
		{
			this.dispatcher.AddEventListener(eventType, listener);
		}
	}
}
