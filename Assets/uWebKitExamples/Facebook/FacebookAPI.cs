
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

class FacebookFriend
{
    public FacebookFriend(string name, string id, Texture2D profilePic = null)
    {
        Name = name;
        ID = id;
        ProfilePic = profilePic;
    }
    
    public string Name;
    public string ID;
    public Texture2D ProfilePic;
}

public delegate void FBSuccessDelegate (UWKWebView view, string json, Dictionary<string, object> values);
public delegate void FBErrorDelegate (UWKWebView view, string json, Dictionary<string, object> values);

public class FBRequest
{

    public FBRequest(UWKWebView view, string path, string method, string parameters)
    {
        this.view = view;
        request =  string.Format(
        @"FB.api('{0}', {1}, {2}, function(response) {
            UWK.sendMessage('{3}', response);
        });", path, method, parameters, message);
    }

    public FBRequest(UWKWebView view, string path, string parameters)
    {
        this.view = view;
        request =  string.Format(
        @"FB.api('{0}', {1}, function(response) {{
            UWK.sendMessage('{2}', response);
        }});", path, parameters, message);
        
    }

    public FBRequest(UWKWebView view, string path)
    {
        this.view = view;
        request =  string.Format(
        @"FB.api('{0}', function(response) {{
            UWK.sendMessage('{1}', response);
        }});", path, message);      
    }

    public void Send()
    {
        view.JSMessageReceived += onJSMessage;
        view.EvaluateJavascript(request);
    }

    public FBSuccessDelegate OnSuccess;
    public FBErrorDelegate OnError;

    private void onJSMessage(UWKWebView view, string message, string json, Dictionary<string, object> values)
    {
        if (message != this.message)
            return;

        object errorObject;
        if (values.TryGetValue("error", out errorObject))
        {
            if (OnError != null)
                OnError(view, json, values);
        }
        else
        {
            if (OnSuccess != null)
                OnSuccess(view, json, values);
        }

    }

    static uint messageID = 0;

    UWKWebView view;
    string request;
    string message = string.Format("fbRequest{0}", messageID++);

}    