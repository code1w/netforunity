using UnityEngine;
using KaiGeX;

// Statics for holding the connection to the SFS server end
// Can then be queried from the entire game to get the connection

public class SmartFoxConnection : MonoBehaviour
{
	private static SmartFoxConnection mInstance; 
	private static KaiGeNet KaiGeNet;
	public static KaiGeNet Connection {
		get {
            if (mInstance == null) {
                mInstance = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
            }
            return KaiGeNet;
        }
      set {
            if (mInstance == null) {
                mInstance = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
            }
            KaiGeNet = value;
        } 
	}

	public static bool IsInitialized {
		get { 
			return (KaiGeNet != null); 
		}
	}
	
	// Handle disconnection automagically
	// ** Important for Windows users - can cause crashes otherwise
    void OnApplicationQuit() { 
        if (KaiGeNet.IsConnected) {
            KaiGeNet.Disconnect();
        }
    } 
}