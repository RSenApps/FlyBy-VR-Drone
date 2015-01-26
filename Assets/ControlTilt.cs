using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;

	public class ControlTilt : MonoBehaviour
	{
		float throttle = 0;
		float xRot;
		float UPDATE_TIME = .5f;
		float lastUpdateTime = 0;
		int MAX_TURN = 1;
		float altCommand = 0;
		internal Boolean socketReady = false;
		TcpClient mySocket;
		NetworkStream theStream;
		StreamWriter theWriter;
		StreamReader theReader;
		public String Host = "172.31.102.44";
		Int32 Port = 6000;
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
		void Update()
		{
			if (updateThrottle() != -898) {
				ControlWithScale.lastThrottle = throttle;
						}
		}
		public float updateThrottle()
		{
			try {
				xRot = Mathf.Deg2Rad * float.Parse(readSocket().Split (new char[] {','})[0]);
			if (Math.Abs (xRot) > .10) {
				throttle =  -xRot * 2 / (float) Math.PI;//negative because tilting the phone up results in a negative angle. limit to angle is 90 degrees.
			} else {
				throttle = 0;
			}
			
			return throttle;
			}
			catch (Exception e)
			{
				}
			return -898;
		}

	}


