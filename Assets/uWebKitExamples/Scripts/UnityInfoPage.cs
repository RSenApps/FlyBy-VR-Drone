/******************************************
  * uWebKit 
  * (c) 2014 THUNDERBEAST GAMES, LLC
  * http://www.uwebkit.com
  * sales@uwebkit.com
*******************************************/

using System;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Example of a web page generated in Unity, using the Javascript Bridge
/// </summary>
public class UnityInfoPage
{

	public static string GetHTML(UWKWebView view)
	{
		view.JSMessageReceived += onJSMessage;
		return HTML;
	}


	private static void onJSMessage(UWKWebView view, string message, string json, Dictionary<string, object> values)
	{
		if (message == "buttonClicked")
		{

		#if UNITY_EDITOR
			EditorUtility.DisplayDialog ("Hello!", "Button clicked, value passed:\n"+ values, "OK!" );
		#endif

		}
	}

	private static string HTML = "";

	// Generate the page HTML
	static UnityInfoPage ()
	{
		
		string[] props = new string[] { "platform", "unityVersion", "systemLanguage", "runInBackground", "isEditor", "dataPath", "persistentDataPath" };
		
		StringBuilder sb = new StringBuilder ();
		
		// Some nice CSS
		sb.Append (@"<html> <head>
		<title>Unity Info Page</title>
		<style type=""text/css"">
		body
		{
			background-color: transparent;
		}
		h1
		{
		color:black;
		text-align:left;
		}
		p
		{
			font-family:""Times New Roman"";
			font-size:20px;
		}
		</style>
		</head>
		<body>");
		
		sb.Append ("<h1> uWebKit JavaScript Bridge Info </h1>");
		
		//sb.Append ("<img src=\"file:///C:/javaBridge.png\" /><br>");
		
		sb.Append ("<input type='button' value=\"Hello\" onclick='UWK.sendMessage(\"buttonClicked\", { value1: 1, value2: \"Testing123\", value3: \"45678\"})' />");
		
		sb.Append (@"<table border=""1"">");
		
		foreach (string p in props) {
			sb.AppendFormat (@"
			<tr>
			<td>Unity.{0}</td>
			<td id = Unity_{0}></td>
			</tr>", p);
		}
		
		sb.Append ("</table>");
		
		sb.Append ("<script type='text/javascript'>");
		
		foreach (string p in props) {
			sb.AppendFormat ("document.getElementById('Unity_{0}').innerText = Unity.{0};", p);
		}
		
		sb.Append ("</script>");	
		
		sb.AppendFormat ("<h4>This page generated in Unity on {0}</h4>", DateTime.UtcNow.ToLocalTime ());
		
		sb.Append ("</body> </html>");
		
		HTML = sb.ToString ();
		
	}

	static bool props = false;

	public static void SetProperties ()
	{		
		if (props)
			return;
		
		props = true;
				
		// Export a bunch of unity variables to JavaScript properties which can then 
		// be accessed on pages
		
		JSObject.SetProperty ("Unity", "unityVersion", Application.unityVersion);
		
		JSObject.SetProperty ("Unity", "loadedLevel", Application.loadedLevel);
		JSObject.SetProperty ("Unity", "loadedLevelName", Application.loadedLevelName);
		JSObject.SetProperty ("Unity", "isLoadingLevel", Application.isLoadingLevel);
		JSObject.SetProperty ("Unity", "levelCount", Application.levelCount);
		JSObject.SetProperty ("Unity", "streamedBytes", Application.streamedBytes);
		
		JSObject.SetProperty ("Unity", "isPlaying", Application.isPlaying);
		JSObject.SetProperty ("Unity", "isEditor", Application.isEditor);
		JSObject.SetProperty ("Unity", "isWebPlayer", Application.isWebPlayer);
		
		JSObject.SetProperty ("Unity", "platform", Application.platform.ToString ());
		
		JSObject.SetProperty ("Unity", "runInBackground", Application.runInBackground);
		
		JSObject.SetProperty ("Unity", "dataPath", Application.dataPath);
		JSObject.SetProperty ("Unity", "persistentDataPath", Application.persistentDataPath);
		JSObject.SetProperty ("Unity", "temporaryCachePath", Application.temporaryCachePath);
		
		JSObject.SetProperty ("Unity", "srcValue", Application.srcValue);
		JSObject.SetProperty ("Unity", "absoluteURL", Application.absoluteURL);
		
		JSObject.SetProperty ("Unity", "webSecurityEnabled", Application.webSecurityEnabled);
		
		JSObject.SetProperty ("Unity", "targetFrameRate", Application.targetFrameRate);
		
		JSObject.SetProperty ("Unity", "systemLanguage", Application.systemLanguage.ToString ());
		
		JSObject.SetProperty ("Unity", "backgroundLoadingPriority", Application.backgroundLoadingPriority.ToString ());
		
		JSObject.SetProperty ("Unity", "internetReachability", Application.internetReachability.ToString ());
		
	}
}

