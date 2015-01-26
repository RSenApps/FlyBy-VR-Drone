using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacebookExample : MonoBehaviour
{   
    void OnLoggedIntoFacebook(FacebookLoginInfo info)
    {
        webView = info.WebView;
        accessToken = info.AccessToken;
		if (outputAccessToken)
			Debug.Log (accessToken);
        getFriends();
    }

    IEnumerator getProfileTexture(FacebookFriend friend)
    {
        string url = "https://graph.facebook.com/" + friend.ID + "/picture";

        WWW www = new WWW(url);

        yield return www;

        if (www.texture != null)
        {
            // store the profile pic
            friend.ProfilePic = www.texture;

            int index = friends.IndexOf(friend) + 1;

            // get first 4 friends
            if (index <= 4 && index < friends.Count)
            {
                StartCoroutine("getProfileTexture", friends[index]);                
            }

        }
        else
        {
            Debug.Log("Error getting profile pic for " + friend.Name);
        }

    }

    void OnGUI()
    {
        int x = 10;
        
        foreach (var friend in friends)
        {
            if (friend.ProfilePic != null)
            {
                if (GUI.Button(new Rect(x, 10, friend.ProfilePic.width + 32, friend.ProfilePic.height + 32), friend.ProfilePic))
                {
                    Debug.Log("Clicked on: " + friend.Name);        
                }

                x += friend.ProfilePic.width + 64;
            
            }

        }

    }

    void getFriends()
    {
        var request = new FBRequest(webView, "/me/friends");

        request.OnSuccess += (view, json, values) =>
        {
            // example of parsing return data
            var friendData = values["data"] as List<object>;
            foreach (Dictionary<string, object> friend in friendData) 
            {   
                friends.Add(new FacebookFriend(friend["name"] as string, friend["id"] as string));
            }

            if (friends.Count > 0)
                StartCoroutine("getProfileTexture", friends[0]);
                
        };

        request.OnError += (view, json, values) =>
        {
            Debug.Log("On Error: " + json);
        };      

        request.Send();                 
    }

	bool outputAccessToken = false;
    UWKWebView webView;
    string accessToken = null;
    List<FacebookFriend> friends = new List<FacebookFriend>();

}


