using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;

public class ParrotInterface : MonoBehaviour {
	internal Boolean socketReady = false;
	TcpClient mySocket;
	NetworkStream theStream;
	StreamWriter theWriter;
	StreamReader theReader;
	String Host = "localhost";
	Int32 Port = 5000;
	// Use this for initialization
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
	// Update is called once per frame
	void Update () {
				if (Input.GetKeyDown (KeyCode.UpArrow)) {
						writeSocket ("t|");
				}
				if (Input.GetKeyDown (KeyCode.DownArrow)) {
						writeSocket ("l|");
				}
	
				if (Input.GetKeyDown (KeyCode.C)) {
						writeSocket ("c.5|");
				}
				if (Input.GetKeyDown (KeyCode.R)) {
					writeSocket ("r.5|");
				}
				if (Input.GetKeyDown (KeyCode.U)) {
					writeSocket ("u.5|");
				}
				if (Input.GetKeyDown (KeyCode.D)) {
					writeSocket ("d.5|");
				}
	}
}
