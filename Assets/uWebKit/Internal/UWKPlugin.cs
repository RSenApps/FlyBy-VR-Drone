/******************************************
  * uWebKit 
  * (c) 2014 THUNDERBEAST GAMES, LLC
  * http://www.uwebkit.com
  * sales@uwebkit.com
*******************************************/

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

// INTERNAL UWEBKIT PLUGIN IMPLEMENTATION


/// <summary>
/// Internal Plugin class for C#/C++ interop
/// </summary>
public class UWKPlugin
{    
    static void log (string message, int level)
    {
        Debug.Log (message);
    }

    static void error (string message, bool fatal)
    {
        Debug.Log (message);
    }

    public static void Initialize()
    {
        // TODO: setup initialization JSON elsewhere, possibly a file
        Dictionary<string,object> djson = new Dictionary<string,object>();

        Dictionary<string,object> app; 
        djson["application"] = app = new Dictionary<string,object>();

        app["hasProLicense"] = Application.HasProLicense();
        app["dataPath"] = Application.dataPath;
        app["persistentDataPath"] = Application.persistentDataPath;
        app["temporaryCachePath"] = Application.temporaryCachePath;
        app["isEditor"] = Application.isEditor;
        app["platform"] = Application.platform;
        app["unityVersion"] = Application.unityVersion;
        app["targetFrameRate"] = Application.targetFrameRate;
        app["graphicsDeviceVersion"] = SystemInfo.graphicsDeviceVersion;
        app["imeEnabled"] = UWKCore.IMEEnabled;

        // Proxy
        app["proxyEnabled"] = UWKConfig.ProxyEnabled;
        app["proxyHostName"] = UWKConfig.ProxyHostname;
        app["proxyUsername"] = UWKConfig.ProxyUsername;
        app["proxyPassword"] = UWKConfig.ProxyPassword;
        app["proxyPort"] = UWKConfig.ProxyPort;
        app["authEnabled"] = UWKConfig.AuthEnabled;
        app["authUsername"] = UWKConfig.AuthUsername;
        app["authPassword"] = UWKConfig.AuthPassword;
        
#if UNITY_EDITOR 
		app["companyName"] = PlayerSettings.companyName;
		app["productName"] = PlayerSettings.productName;
#else	
		// Unity doesn't provide this data at runtime
		// so, load it from our config

		app["companyName"] = "DefaultCompany";
		app["productName"] = "DefaultProduct";

		var cfgfile = Application.streamingAssetsPath + "/uWebKit/Config/uwebkit.cfg";

		if (File.Exists(cfgfile))
		{
			var jsonString = "";

            #if !UNITY_WEBPLAYER    
            jsonString = File.ReadAllText(cfgfile);
            #endif

			var cfg = UWKJson.Deserialize(jsonString) as Dictionary<string,object>;

			if (cfg.ContainsKey("companyName"))
				app["companyName"] = (string) cfg["companyName"];

			if (cfg.ContainsKey("productName"))
				app["productName"] = (string) cfg["productName"];
		
		}

#endif

        var json = UWKJson.Serialize(djson);
        var nativeString = NativeUtf8FromString(json);

        int betaCheck = UWK_Initialize(log, error, processMessage, nativeString);

        if (betaCheck >= 99)
        {
            UWKCore.BetaVersion = true;
            
            betaCheck -= 100;

            if (betaCheck == -1)
            {
                Debug.LogError ("uWebKit Beta Expired");
#if UNITY_EDITOR                
                EditorUtility.DisplayDialog ("uWebKit Beta Expired", "This BETA version of uWebKit has expired.\nPlease visit http://www.uwebkit.com", "Ok");            
                EditorApplication.ExecuteMenuItem ("Edit/Play");
#endif                
            }
            else 
            {
                string message = String.Format("There are {0} days left of this expiring uWebKit BETA", betaCheck);

                if (betaCheck == 0)
                    message = String.Format("There is less than a day left of this expiring uWebKit BETA");

                if (!UWK_HasDisplayedBetaMessage())    
                {
#if UNITY_EDITOR                
                    EditorUtility.DisplayDialog ("uWebKit BETA Version", "\nThis is a BETA version of uWebKit.\n\n"+message, "Ok");                                    
#endif
                }

                Debug.Log(message);    

            }
        }
        
        Marshal.FreeHGlobal(nativeString);
    }

    [DllImport("UWKPlugin", CharSet = CharSet.Ansi)]
    public static extern uint UWK_PostUnityKeyEvent (uint browserID, ref UnityKeyEvent keyEvent);

    [DllImport ("UWKPlugin")]
    public static extern int UWK_Initialize(LogCallbackDelegate logcb, LogErrorDelegate errorcb, ProcessMessageDelegate processcb, IntPtr initJson);

    [DllImport ("UWKPlugin")]
    public static extern void UWK_Update();

    [DllImport ("UWKPlugin")]
    public static extern bool UWK_HasDisplayedBetaMessage();    

    [DllImport ("UWKPlugin")]
    public static extern uint UWK_CreateView(int maxWidth, int maxHeight, [MarshalAs(UnmanagedType.LPStr)]String url, IntPtr nativeTexturePtr);    

    [DllImport ("UWKPlugin")]
    public static extern void UWK_DestroyView(uint browserID);    

    [DllImport ("UWKPlugin")]
    public static extern int UWK_Shutdown();

    [DllImport ("UWKPlugin")]
    public static extern void UWK_ClearCookies();

    [DllImport ("UWKPlugin")]
    public static extern void UWK_MsgMouseMove(uint browserID, int x, int y);

    [DllImport ("UWKPlugin")]
    public static extern void UWK_MsgMouseButtonDown(uint browserID, int x, int y, int button);

    [DllImport ("UWKPlugin")]
    public static extern void UWK_MsgMouseButtonUp(uint browserID, int x, int y, int button);

    [DllImport ("UWKPlugin")]
    public static extern void UWK_MsgMouseScroll(uint browserID, int x, int y, float scroll);

    [DllImport ("UWKPlugin")]
    public static extern uint UWK_MsgLoadURL(uint browserID, [MarshalAs(UnmanagedType.LPStr)]String url);

    [DllImport ("UWKPlugin")]
    public static extern uint UWK_MsgSetUserAgent(uint browserID, [MarshalAs(UnmanagedType.LPStr)]String agent);

	[DllImport ("UWKPlugin")]
	public static extern uint UWK_MsgActivate([MarshalAs(UnmanagedType.LPStr)]String key);    

    [DllImport ("UWKPlugin")]
    public static extern IntPtr UWK_GetMessageDataPtr(IntPtr msgPtr, int index);    

    [DllImport ("UWKPlugin")]
    private static extern uint UWK_MsgEvaluateJavascript(uint browserId, IntPtr utf8Javascript, AsyncMessageDelegate callback = null);

    [DllImport ("UWKPlugin")]
    private static extern uint UWK_MsgJSMessage(uint browserId, IntPtr utf8MsgName, IntPtr utf8JsonValue);

        [DllImport ("UWKPlugin")]
    private static extern void UWK_SetIMEText(uint browserId, IntPtr utf8IMEText);

    [DllImport ("UWKPlugin")]
    private static extern void UWK_MsgJSObjectSetProperty(IntPtr utf8ObjectName, IntPtr utf8PropertyName, IntPtr utf8Value);

    [DllImport ("UWKPlugin")]
    private static extern void UWK_MsgJSObjectRemove(IntPtr utf8ObjectName); 

    [DllImport ("UWKPlugin")]
    public static extern uint UWK_MsgShow(uint browserId, bool show = true);

    [DllImport ("UWKPlugin")]
    public static extern uint UWK_MsgViewReload(uint browserId);    

    [DllImport ("UWKPlugin")]
    public static extern void UWK_MsgViewStop(uint browserId);        

    [DllImport ("UWKPlugin")]
    public static extern uint UWK_MsgSetAlphaMask(uint browserId, bool enabled);  

    [DllImport ("UWKPlugin")]
    public static extern uint UWK_MsgSetTextCaretColor(uint browserId, uint color);  

    [DllImport ("UWKPlugin")]
    public static extern void UWK_MsgSetCurrentSize(uint browserID, int width, int height);

    [DllImport ("UWKPlugin")]
    public static extern void UWK_MsgSetZoomFactor(uint browserID, float zoom);

    [DllImport ("UWKPlugin")]
    public static extern void UWK_MsgSetScrollPosition(uint browserID, int x, int y);

    [DllImport ("UWKPlugin")]
    public static extern uint UWK_MsgNavigate(uint browserId, int value);

    [DllImport ("UWKPlugin")]
    public static extern uint UWK_MsgSetFrameRate(uint browserId, int frameRate);

    [DllImport ("UWKPlugin")]
    public static extern void UWK_MsgLoadHTML(uint browserId, IntPtr utf8HTML, IntPtr utf8BaseURL);

    [DllImport ("UWKPlugin")]
    public static extern void UWK_MsgViewShowInspector(uint browserId);        

    [DllImport ("UWKPlugin")]
    public static extern void UWK_MsgViewCloseInspector(uint browserId);        

    private static string stringFromNativeUtf8(IntPtr nativeUtf8) 
    {
        if (nativeUtf8 == IntPtr.Zero)
            return string.Empty;
        
        int len = 0;
        while (Marshal.ReadByte(nativeUtf8, len) != 0) 
        {
            ++len;
        }

        if (len == 0) 
            return string.Empty;

        byte[] buffer = new byte[len];
        Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer);
    }

    public static IntPtr NativeUtf8FromString(string managedString) 
    {
        int len = Encoding.UTF8.GetByteCount(managedString);
        byte[] buffer = new byte[len + 1];
        Encoding.UTF8.GetBytes(managedString, 0, managedString.Length, buffer, 0);
        IntPtr nativeUtf8 = Marshal.AllocHGlobal(buffer.Length);
        Marshal.Copy(buffer, 0, nativeUtf8, buffer.Length);
        return nativeUtf8;
    }

    [DllImport("UWKPlugin")]
    static extern bool UWK_GetMsgDataBytes(IntPtr msgPtr, int index, IntPtr pout, int size);

    [DllImport("UWKPlugin")]
    static extern int UWK_GetMsgDataSize(IntPtr msgPtr, int index);

    [DllImport("UWKPlugin")]
    public static extern void UWK_DevelopmentOnlyCrashProcess();

    [DllImport("UWKPlugin")]
    public static extern void UWK_DevelopmentOnlyCrashWebProcess();

    [DllImport("UWKPlugin")]
    public static extern void UWK_DevelopmentOnlyHangWebProcess();


    public static int GetMsgDataSize(ref UWKMessage msg, int index)
    {
        return UWK_GetMsgDataSize(msg._this, index);
    }

    public static bool GetMsgDataBytes (ref UWKMessage msg, int index, int sz, byte[] bytes)
    {
        GCHandle pinned = GCHandle.Alloc (bytes, GCHandleType.Pinned);        
        bool r = UWK_GetMsgDataBytes (msg._this, index, pinned.AddrOfPinnedObject (), sz);        
        pinned.Free ();        
        return r;        
    }

    public static void EvaluateJavascript(uint browserId, string javascript, AsyncMessageDelegate callback = null)
    {
        var nativeString = NativeUtf8FromString(javascript);

        UWK_MsgEvaluateJavascript(browserId, nativeString, callback);

        Marshal.FreeHGlobal(nativeString);

    }

    public static void LoadHTML(uint browserId, string html, string baseURL)
    {
        var nativeHTML= NativeUtf8FromString(html);
        var nativeBaseURL= NativeUtf8FromString(baseURL);

        UWK_MsgLoadHTML(browserId, nativeHTML, nativeBaseURL);

        Marshal.FreeHGlobal(nativeBaseURL);
        Marshal.FreeHGlobal(nativeHTML);
    }


    public static void SendJSMessage(uint browserId, string msgName, string json)
    {
        var nativeMsgName = NativeUtf8FromString(msgName);
        var nativeJson = NativeUtf8FromString(json);

        UWK_MsgJSMessage(browserId, nativeMsgName, nativeJson);

        Marshal.FreeHGlobal(nativeJson);
        Marshal.FreeHGlobal(nativeMsgName);
    }

    public static void SetIMEText(uint browserId, string imeText)
    {
        var nativeIMEText = NativeUtf8FromString(imeText);

        UWK_SetIMEText(browserId, nativeIMEText);

        Marshal.FreeHGlobal(nativeIMEText);
    }    

    public static string GetMessageString(ref UWKMessage msg, int index)
    {
        IntPtr address = UWK_GetMessageDataPtr(msg._this, index);
        return stringFromNativeUtf8(address);
    } 

    public static void RemoveJSObject(string objectName)
    {
        var nativeObjectName = NativeUtf8FromString(objectName);

        UWK_MsgJSObjectRemove(nativeObjectName);

        Marshal.FreeHGlobal(nativeObjectName);

    }

    public static void SetJSObjectProperty(string objectName, string propertyName, string value)
    {
        var nativeObjectName = NativeUtf8FromString(objectName);
        var nativePropertyName = NativeUtf8FromString(propertyName);
        var nativeValue = NativeUtf8FromString(value);

        UWK_MsgJSObjectSetProperty(nativeObjectName, nativePropertyName, nativeValue);

        Marshal.FreeHGlobal(nativeObjectName);
        Marshal.FreeHGlobal(nativePropertyName);
        Marshal.FreeHGlobal(nativeValue);
    }

    private static void processMessage (IntPtr pcmd)
    {
        UWKMessage msg = (UWKMessage) Marshal.PtrToStructure (pcmd, typeof(UWKMessage));
        msg._this = pcmd;
        UWKCore.ProcessMessage (ref msg);
    }
}

public enum UWKMessageType
{
    UMSG_INVALID,
    UMSG_VIEW_CREATE,
    UMSG_VIEW_DESTROY,
    UMSG_GPUSURFACE_INFO,
    UMSG_MOUSE_MOVE,
    UMSG_MOUSE_DOWN,
    UMSG_MOUSE_UP,
    UMSG_MOUSE_SCROLL,
    UMSG_KEY_DOWN,
    UMSG_KEY_UP,
    UMSG_LOG,
    UMSG_ERROR,
    UMSG_VIEW_LOADFINISHED,
    UMSG_VIEW_URLCHANGED,
    UMSG_VIEW_TITLECHANGED,
    UMSG_VIEW_ICONCHANGED,
    UMSG_VIEW_CONTENTSIZECHANGED,
    UMSG_VIEW_SETCURRENTSIZE,
    UMSG_VIEW_SETSCROLLPOSITION,
    UMSG_VIEW_EVALUATE_JAVASCRIPT,
    UMSG_VIEW_LOADURL,
    UMSG_VIEW_LOADHTML,
    UMSG_VIEW_STOP,
    UMSG_VIEW_SHOW,
    UMSG_VIEW_NAVIGATE,
    UMSG_VIEW_SETZOOMFACTOR,
    UMSG_VIEW_REQUESTNEWVIEW,
    UMSG_VIEW_SETALPHAMASK,
    UMSG_VIEW_SETTEXTCARETCOLOR,
    UMSG_VIEW_RELOAD,
    UMSG_VIEW_SETFRAMERATE,
    UMSG_VIEW_SETUSERAGENT,
    UMSG_VIEW_SHOWINSPECTOR,
    UMSG_VIEW_CLOSEINSPECTOR,
    UMSG_ASYNC_RESULT,
    UMSG_JAVASCRIPT_CONSOLE,
    UMSG_JAVASCRIPT_MESSAGE,
    UMSG_JSOBJECT_SETPROPERTY,
    UMSG_JSOBJECT_REMOVE,
    UMSG_VIEW_LOADPROGRESS,
    UMSG_SHUTDOWN_WEBRENDERPROCESS,
    UMSG_COOKIES_CLEAR,
    UMSG_IME_FOCUSIN,
    UMSG_IME_FOCUSOUT,
    UMSG_IME_SETTEXT,
    UMSG_DEV_CRASHWEBPROCESS,
    UMSG_DEV_HANGWEBPROCESS,
    UMSG_ACTIVATION_STATE,
    UMSG_ACTIVATE
};

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct UWKMessage
{

    public IntPtr _this;

    public UWKMessageType type;

    public uint asyncMessageID;

    public uint browserID;

    [MarshalAsAttribute(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I4, SizeConst = 16)]
    public int[] iParams;

    [MarshalAsAttribute(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I4, SizeConst = 16)]
    public int[] fParams;
    
    // this is int on 32 bit OSX, need to verify that windows/64 bit is same
    [MarshalAsAttribute(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I4, SizeConst = 16)]
    public int[] dataHandle;

    [MarshalAsAttribute(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I4, SizeConst = 16)]
    public uint[] dataSize;

}   

[Flags]
enum UnityKeyModifiers
{
    Shift =      0x1,
    Control =    0x2,
    Alt =        0x4, 
    CommandWin = 0x8, // windows or command key
    Numeric =    0x10, 
    CapsLock =   0x20,
    FunctionKey = 0x40  
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct UnityKeyEvent
{
    public uint Type; // 1 for down 0 for up
    public uint Modifiers;
    public uint KeyCode;
    public uint Character;
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ProcessMessageDelegate (IntPtr pcmd);

// TODO: expose these as an API struct
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void LogCallbackDelegate (string message, int level);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void LogErrorDelegate (string message, bool fatal);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void AsyncMessageDelegate (uint id, string value);



