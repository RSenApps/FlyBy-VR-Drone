
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FacebookLoginInfo
{
    public UWKWebView WebView;
    public string AccessToken;
}

public class FacebookLogin : MonoBehaviour
{
    enum State { Start, Login, LoggedIn};

    public Texture2D LoginTexture;

    public string FacebookLoginSite = "http://uwk.uwebkit.com/examples/FacebookSiteExample.html";

    public int X = 0;
    public int Y = 0;
    public int Width = 800;
    public int Height = 600;

    Rect windowRect;
    Rect browserRect;

    Vector2 scale;

    State state;

    int unityWindowId = 1;

    UWKWebView view;

    string accessToken = "";

    void Start()
    {
        windowRect = new Rect (X, Y, Width + 16, Height + 16);        

        view = UWKWebView.AddToGameObject(gameObject, FacebookLoginSite, Width, Height);
        view.JSMessageReceived += onJSMessage;
        view.Hide();

        Center();
    }

    void handleLoggedIn()
    {
        view.JSMessageReceived -= onJSMessage;
        view.Hide();
        state = State.LoggedIn;

        FacebookLoginInfo info = new FacebookLoginInfo();
        info.AccessToken = accessToken;
        info.WebView = view;

        SendMessage("OnLoggedIntoFacebook", info, SendMessageOptions.DontRequireReceiver);        

        enabled = false;
    }

    void windowFunction (int windowID)
    {
        int titleHeight = 24;
        Rect titleRect = new Rect (0, 0, Width + 16, titleHeight);
        
        GUI.DragWindow (titleRect);

        GUILayout.BeginVertical();

        GUIStyle style = new GUIStyle(GUI.skin.textArea);
        style.fontSize = 24;

        string text = @"This example shows everything needed to implement a login to Facebook dialog using uWebKit2";

        GUILayout.TextArea(text, style);

        if (state == State.Start)
        {
            if (GUILayout.Button(LoginTexture))
            {
                if (accessToken == "")
                {
                    view.Show();
                    state = State.Login;
                }
                else
                {                   
                    handleLoggedIn();
                }               
            }

            GUILayout.Space(64);

            if (GUILayout.Button("Clear\nCookies",  GUILayout.MaxWidth(100)))
            {
                UWKCore.ClearCookies();
            }
        }
        else if (state == State.Login)
        {           
            GUILayout.Box(view.WebTexture);

            if (Event.current.type == EventType.Repaint)
            {
                browserRect = GUILayoutUtility.GetLastRect ();

                browserRect.x += windowRect.x;
                browserRect.y += windowRect.y;

                scale.x = (float)Width / (float)browserRect.width;
                scale.y = (float)Height / (float)browserRect.height;
            }

            
        }       
        else if (state == State.LoggedIn)
        {

        }

        GUILayout.EndVertical();

    }

    void OnGUI ()
    {
        if (state == State.LoggedIn)
            return;

        windowRect = GUILayout.Window (unityWindowId, windowRect, windowFunction, "");

        if (state == State.Login)
        {
            if (Event.current.type == EventType.Layout)
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.y = Screen.height - mousePos.y;   

                if (browserRect.Contains(mousePos))
                {
                    mousePos.x -= browserRect.x;
                    mousePos.y -= browserRect.y;

                    mousePos.x *= scale.x;
                    mousePos.y *= scale.y;

                    view.ProcessMouse(mousePos);            
                }           
            }

            if (Event.current.isKey)
            {
                view.ProcessKeyboard(Event.current);

                if (Event.current.keyCode == KeyCode.Tab || Event.current.character == '\t')
                    Event.current.Use ();
            }
        }
    }


    // Get the center position
    public void GetCenterPos (ref Vector2 pos)
    {
        pos.x = Screen.width / 2 - windowRect.width / 2;
        pos.y = Screen.height / 2 - windowRect.height / 2;
    }

    // Center the browser on the screen
    public void Center ()
    {
        Vector2 v = new Vector2 ();
        
        GetCenterPos (ref v);
        
        windowRect.x = v.x;
        windowRect.y = v.y;
    }

    void onJSMessage(UWKWebView view, string message, string json, Dictionary<string, object> values)
    {
        if (message == "fbAuthResponse")
        {                       
            accessToken = (string) values["accessToken"];   

            if (state != State.Start)
            {
                handleLoggedIn();
            }           
            
        }       
        else if (message == "fbNotLoggedIn")
        {
            accessToken = "";
            Debug.Log("fbNotLoggedIn");
        }
    }


}