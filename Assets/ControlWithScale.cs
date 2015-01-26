using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
public class ControlWithScale : MonoBehaviour {
	/** The x, y and z axes positions */
	OVRDisplay ovrTracker;
	float UPDATE_TIME = .5f;
	float lastUpdateTime = 0;
	int MAX_TURN = 1;
	double[] commands = new double[3]; 
	internal Boolean socketReady = false;
	TcpClient mySocket;
	NetworkStream theStream;
	StreamWriter theWriter;
	StreamReader theReader;
	String Host = "localhost";
	public static float lastThrottle = 0;
	Int32 Port = 5000;
	void Start () {
		try {
			mySocket = new TcpClient (Host, Port);
			theStream = mySocket.GetStream ();
			theWriter = new StreamWriter (theStream);
			theReader = new StreamReader(theStream); 
			socketReady = true; 
		} catch (Exception e) { 
			Debug.Log("Socket error: " + e); 
		} 
		ovrTracker = OVRManager.display;
		for (int i=0; i<3; i++) {
			commands [i] = 0;
		}
	}
	public void writeSocket(string theLine) { 
		if (!socketReady) 
			return; 
		String foo = theLine; 
		theWriter.Write(foo); theWriter.Flush(); } 
	public String readSocket() { 
		if (!socketReady) return ""; 
		if (theStream.DataAvailable) 
			return theReader.ReadLine(); return ""; } 
	public void closeSocket() { if (!socketReady) 
		return; theWriter.Close(); theReader.Close(); mySocket.Close(); socketReady = false;
		
	}
	// Update is called once per frame
	void Update () { 
				String text = "";
				if (Input.GetKeyDown (KeyCode.UpArrow)) {
						text += "t|";
				} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
						text += "l|";
				} else if (Time.time - lastUpdateTime > UPDATE_TIME) {
						lastUpdateTime = Time.time;
						Quaternion quat = ovrTracker.GetHeadPose (0.0).orientation;
						float xRot = quat.x;
						float yRot = quat.y;
						float zRot = quat.z;
						//if less than nullzone, will not actually command
						if (Mathf.Abs (xRot) >= .12) {
								commands [0] = MAX_TURN * 4 * (xRot / 3.14);
						} else {
								commands [0] = 0;
						}
						if (Mathf.Abs (yRot) >= .16) {
								commands [1] = MAX_TURN *4 * (yRot / 3.14);
						} else {
								commands [1] = 0;
						}
						if (Mathf.Abs (zRot) >= .12) {
								commands [2] = MAX_TURN * -4 * (zRot / 3.14);
						} else {
								commands [2] = 0;
						}
						if (Input.GetKey(KeyCode.Q))
			{
				lastThrottle = .5f;
			}
			else if (Input.GetKey(KeyCode.A))
			{
				lastThrottle = -.5f;
			}
						text += "f" + commands [0] + "|" + "c" + commands [1] + "|" + "r" + commands [2] + "|" + "u" + lastThrottle + "|";
		
				}
				if (text.Length > 0) {
						writeSocket (text);
				}


	}
}

