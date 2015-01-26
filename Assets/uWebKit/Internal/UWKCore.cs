
/******************************************
  * uWebKit 
  * (c) 2014 THUNDERBEAST GAMES, LLC
  * http://www.uwebkit.com
  * sales@uwebkit.com
*******************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

// INTERNAL UWEBKIT CORE

/// <summary>
/// Internal Web Core Component
/// </summary>
public class UWKCore : MonoBehaviour
{

    /// <summary>
    /// Main initialization of web core
    /// </summary>
    public static void Init()
    {
        // if we're already initialized, return immediately
        if (sInstance != null)
            return;

        // we want to run in the background 
        Application.runInBackground = true;

        // check for IME and enable it if necessary
        string lang = Application.systemLanguage.ToString();
        
        if (lang == "Chinese" || lang == "Japanese" || lang == "Korean")
            IMEEnabled = true;

        // initialize the native plugin
        UWKPlugin.Initialize();             

        // add ourselves to a new game object
        GameObject go = new GameObject ("UWKCore");
        UnityEngine.Object.DontDestroyOnLoad (go);
        UWKCore core = go.AddComponent<UWKCore> ();

        // we're all ready to go
        sInstance = core;
    }    


    // sets up a coroutine to be called at the end of the frame
    IEnumerator Start () 
    {
        yield return StartCoroutine("CallPluginAtEndOfFrames");
    }

    // issues GL event which will update the web texture on native plugin side
    private IEnumerator CallPluginAtEndOfFrames ()
    {    
        while (true) 
        {
            // Wait until all frame rendering is done
            yield return new WaitForEndOfFrame();     
            GL.IssuePluginEvent (renderEventUpdateTextures);
        }
    }    

    float lastUpdate = 0.0f;

    void Update()
    {

#if UNITY_EDITOR        
        if(EditorApplication.isCompiling) 
        {
            Debug.Log("Unity is recompiling scripts, play session ended.");
            EditorApplication.ExecuteMenuItem ("Edit/Play");
            return; 
        }
#endif      

#if UNITY_4_3
        lastUpdate += Time.deltaTime;
#else
        lastUpdate += Time.unscaledDeltaTime;
#endif
        // throttle
        if (lastUpdate > .033f)
        {
            lastUpdate = 0;
            UWKPlugin.UWK_Update();
        }

    }

    void OnDestroy()
    {
        // Core is coming down, close up shop and clean up
        GL.IssuePluginEvent (renderEventShutdown);
        UWKPlugin.UWK_Shutdown();    
        viewLookup = new Dictionary<uint, UWKWebView>();
    }


    /// <summary>
    /// Internal Plugin -> Unity message receiving
    /// </summary>
    public static void ProcessMessage(ref UWKMessage msg)
    {
        UWKWebView view = null;

        bool msgView = false;
        switch (msg.type)
        {
            case UWKMessageType.UMSG_VIEW_LOADFINISHED:
            case UWKMessageType.UMSG_VIEW_URLCHANGED:
            case UWKMessageType.UMSG_VIEW_TITLECHANGED:
            case UWKMessageType.UMSG_VIEW_LOADPROGRESS:
            case UWKMessageType.UMSG_VIEW_ICONCHANGED:
            case UWKMessageType.UMSG_JAVASCRIPT_CONSOLE:
            case UWKMessageType.UMSG_JAVASCRIPT_MESSAGE:
            case UWKMessageType.UMSG_VIEW_CONTENTSIZECHANGED:
            case UWKMessageType.UMSG_IME_FOCUSIN:
            case UWKMessageType.UMSG_IME_FOCUSOUT:
            case UWKMessageType.UMSG_VIEW_REQUESTNEWVIEW:
                msgView = true;
                break;
        }

        if (msgView)
        {
            if (!viewLookup.TryGetValue(msg.browserID, out view))
            {
                Debug.Log("Warning: Unable to get view for message: " + Enum.GetName(typeof(UWKMessageType), msg.type));
                return;
            }
        }

        switch (msg.type)
        {
            case UWKMessageType.UMSG_VIEW_LOADFINISHED:
                view.LoadFinished(view);
                break;
            case UWKMessageType.UMSG_VIEW_URLCHANGED:
                view.URLChanged(view, UWKPlugin.GetMessageString(ref msg, 0));
                break;
            case UWKMessageType.UMSG_VIEW_TITLECHANGED:
                view.TitleChanged(view, UWKPlugin.GetMessageString(ref msg, 0));
                break;
            case UWKMessageType.UMSG_VIEW_LOADPROGRESS:
                view.LoadProgress(view, msg.iParams[0]);
                break;
            case UWKMessageType.UMSG_VIEW_ICONCHANGED:

                int size = UWKPlugin.GetMsgDataSize(ref msg, 0);

                if (size > 0)
                {
                    byte[] bytes = new byte[size];

                    if (UWKPlugin.GetMsgDataBytes(ref msg, 0, size, bytes))
                    {
                        view.IconChanged(msg.iParams[0], msg.iParams[1], bytes);
                    }
                }

                break;
            case UWKMessageType.UMSG_JAVASCRIPT_CONSOLE:
                view.JSConsole(view, UWKPlugin.GetMessageString(ref msg, 0), msg.iParams[0], UWKPlugin.GetMessageString(ref msg, 1));
                break;
            case UWKMessageType.UMSG_JAVASCRIPT_MESSAGE:
                var json = UWKPlugin.GetMessageString(ref msg, 1);
                var dict = UWKJson.Deserialize(json) as Dictionary<string,object>;
                view.JSMessageReceived(view, UWKPlugin.GetMessageString(ref msg, 0), json, dict);            
                break;
            case UWKMessageType.UMSG_VIEW_CONTENTSIZECHANGED:
                view.ContentSizeChanged(view, msg.iParams[0], msg.iParams[1]);
                break;
            case UWKMessageType.UMSG_IME_FOCUSIN:
                view.IMEFocusIn(ref msg);
                break;
            case UWKMessageType.UMSG_IME_FOCUSOUT:
                view.IMEFocusOut();
                break;
            case UWKMessageType.UMSG_VIEW_REQUESTNEWVIEW:
                view.NewViewRequested(view, UWKPlugin.GetMessageString(ref msg, 0));
                break;
            case UWKMessageType.UMSG_ACTIVATION_STATE:
            #if UNITY_EDITOR   
	            if (msg.iParams[0] != 1 && msg.iParams[0] != 5)
	            {
					if (sInstance.gameObject.GetComponent<UWKActivation>() == null)
						sInstance.gameObject.AddComponent<UWKActivation>();
	            }
				UWKActivation.SetActivationState(msg.iParams[0]);
            #endif      

                break;

        }

    }

    public static void ClearCookies()
    {
        UWKCore.Init();
        UWKPlugin.UWK_ClearCookies();
    }

    /// <summary>
    /// Internal View Creation
    /// </summary>
    public static uint CreateView(UWKWebView view, int maxWidth, int maxHeight, string url, IntPtr nativeTexture)
    {
        uint id = UWKPlugin.UWK_CreateView(maxWidth, maxHeight, url, nativeTexture);
        viewLookup[id] = view;      
        return id;
    }

    /// <summary>
    /// Internal View Destruction
    /// </summary>
    public static void DestroyView(UWKWebView view)
    {
        if (viewLookup.ContainsKey(view.ID))
            viewLookup.Remove(view.ID);

        UWKPlugin.UWK_DestroyView(view.ID);
    }
	
	public static bool IMEEnabled = false;
    public static bool BetaVersion = false;


    static UWKCore sInstance = null;

    static int renderEventUpdateTextures = 1;
    static int renderEventShutdown = 2;

    static Dictionary<uint, UWKWebView> viewLookup = new Dictionary<uint, UWKWebView>();

}

