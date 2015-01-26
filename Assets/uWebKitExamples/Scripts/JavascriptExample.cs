/******************************************
  * uWebKit 
  * (c) 2014 THUNDERBEAST GAMES, LLC
  * http://www.uwebkit.com
  * sales@uwebkit.com
*******************************************/

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Simple menu for WebGUI Example
/// </summary>
public class JavascriptExample : MonoBehaviour
{
    WebGUI webGUI;
    UWKWebView view;
    int messageCount = 1;

    string messageReceived = "";

    // Use this for initialization
    void Start ()
    {       

        JSObject.SetProperty ("MyJSObject", "unityVersion", Application.unityVersion);

        webGUI = gameObject.GetComponent<WebGUI>();
        view = gameObject.GetComponent<UWKWebView>();
        view.JSMessageReceived += onJSMessage;
       
        view.LoadURL(UWKWebView.GetApplicationDataURL() + "/StreamingAssets/uWebKit/Examples/JavascriptExample.html");
        
        webGUI.Position.x = Screen.width / 2 - view.MaxWidth / 2;
        webGUI.Position.y = 0;

    }

    void onJSMessage(UWKWebView view, string message, string json, Dictionary<string, object> values)
    {
        if (message == "UnityMessage")
        {
            messageReceived  = "Message Received:\n" + json;
        }

    }

    void OnGUI ()
    {
        Rect brect = new Rect (0, 0, 120, 40);

        if (UWKCore.BetaVersion)
        {
            GUI.Label(new Rect (0, 0, 200, 60), "UWEBKIT BETA VERSION\nCheck http://www.uwebkit.com\nfor updates");
            brect.y += 50;
        }
        
        if (GUI.Button (brect, "Back")) 
        {           
            Application.LoadLevel ("ExampleLoader");
        }

        brect.y += 50;

        if (GUI.Button (brect, "Send JS Message")) 
        {           
            view.SendJSMessage("ExampleMessage", "Javascipt Message " + messageCount++);
        }

        if (messageReceived.Length != 0)
        {
            brect.y += 50;
            Rect trect = new Rect(brect);
            trect.width += 32;
            trect.height += 32;
            GUI.TextArea(trect, messageReceived);
        }

        




    }
    
}