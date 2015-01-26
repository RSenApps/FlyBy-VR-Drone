/******************************************
  * uWebKit 
  * (c) 2014 THUNDERBEAST GAMES, LLC
  * http://www.uwebkit.com
  * sales@uwebkit.com
*******************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

/// <summary>
/// URLChangedDelegate - Event fired when the URL has been changed
/// either by user input or due to a page redirect
/// </summary>
public delegate void URLChangedDelegate (UWKWebView view, string url);

/// <summary>
///  TitleChangedDelegate - Event fired when the title of a web page has changed
/// </summary>
public delegate void TitleChangedDelegate (UWKWebView view, string title);

/// <summary>
/// LoadFinishedDelegate - Event fired once the page has been fully loaded
/// </summary>
public delegate void LoadFinishedDelegate(UWKWebView view);

/// <summary>
/// LoadProgressDelegate - Event fired as the page loads to show progress
/// </summary>
public delegate void LoadProgressDelegate(UWKWebView view, int progress);

/// <summary>
/// ContentSizeChangedDelegate - Event fired when the content size of the page changes
/// </summary>
public delegate void ContentSizeChangedDelegate(UWKWebView view, int width, int heights);

/// <summary>
/// JSResultDelegate - Event fired with result of javascript call
/// </summary>
public delegate void JSResultDelegate(UWKWebView view, uint id, string result);

/// <summary>
/// JSResultDelegate - Event fired with result of javascript call
/// </summary>
public delegate void JSConsoleDelegate(UWKWebView view, string message, int line, string source);

/// <summary>
/// JSMessageReceived - Event fired with result of javascript message
/// </summary>
public delegate void JSMessageReceivedDelegate(UWKWebView view, string message, string json, Dictionary<string, object> values);

/// <summary>
/// RequestNewViewDelegate - Event fired when the view wants to open a new view (for example a popup)
/// </summary>
public delegate void RequestNewViewDelegate (UWKWebView view, string url);       

/// <summary>
/// JSEvalDelegate - Event fired when return value of some evaluated Javascript is returned
/// </summary>
public delegate void JSEvalDelegate (string value); 

/// <summary>
/// WebView component used to render and interact with web content
/// </summary>
public class UWKWebView : MonoBehaviour
{

    #region Inspector Fields

    /// <summary>
    /// The initial URL to load
    /// </summary>    
    public string URL;

    /// <summary>
    /// Gets the current width of the UWKWebView
    /// </summary>        
    public int CurrentWidth = 1024;

    /// <summary>
    /// Gets the current height of the UWKWebView
    /// </summary>        
    public int CurrentHeight = 1024;

    /// <summary>
    /// Max width of the UWKWebView, defined at creation time.  It is possible to set the 
    /// view's current width to be equal or smaller than this value
    /// </summary>    
    public int MaxWidth = 1024;

    /// <summary>
    /// Max height of the UWKWebView, defined at creation time.  It is possible to set the 
    /// view's current height to be equal or smaller than this value
    /// </summary>        
    public int MaxHeight = 1024;
	
	/// <summary>
	/// Used to make the scroll wheel/trackpad more sensitive
	/// </summary>        
	public float ScrollSensitivity = 1.0f;

	#endregion
	
	/// <summary>
    /// Naivation for going backwards and forwards through the UWKWebView's history
    /// view's current height to be equal or smaller than this value
    /// </summary>        
    public enum Navigation
    {
        Forward = 0,
        Back
    }   

    /// <summary>
    /// Gets the width of the UWKWebView's content
    /// </summary>        
    public int ContentWidth
    {
        get { return contentWidth; }
    }

    /// <summary>
    /// Gets the height of the UWKWebView's content
    /// </summary>        
    public int ContentHeight
    {
        get { return contentHeight; }
    }

    /// <summary>
    /// Texture2D which is used for the page's contents
    /// </summary>        
    [HideInInspector]
    public Texture2D WebTexture;

    /// <summary>
    /// Texture2D which is used for the page's icon
    /// </summary>            
    [HideInInspector]
    public Texture2D WebIcon;

    /// <summary>
    /// A unique ID for this UWKWebView
    /// </summary>        
    [HideInInspector]
    public uint ID;

    /// <summary>
    /// The title of the current loaded page
    /// </summary>        
    [HideInInspector]
    public string Title = "";

    /// <summary>
    /// Delegate fired when the URL of the page has changed
    /// </summary>    
    public URLChangedDelegate URLChanged;

    /// <summary>
    /// Delegate fired when the Title of the page has changed
    /// </summary>    
    public TitleChangedDelegate TitleChanged;

    /// <summary>
    /// Delegate fired when the page has finished loaded
    /// </summary>    
    public LoadFinishedDelegate LoadFinished;

    /// <summary>
    /// Delegate fired when the page has updated the loading progress 0-100
    /// </summary>    
    public LoadProgressDelegate LoadProgress;

    /// <summary>
    /// Delegate fired when the content size of the page has changed
    /// </summary>    
    public ContentSizeChangedDelegate ContentSizeChanged;

    /// <summary>
    /// Delegate fired when the page has output something to console.log
    /// </summary>    
    public JSConsoleDelegate JSConsole;

    /// <summary>
    /// Delegate fired when the page has received a Javascript message
    /// </summary>    
    public JSMessageReceivedDelegate JSMessageReceived;

    /// <summary>
    /// Delegate fired when the page has requested a popup window be created
    /// </summary>    
    public RequestNewViewDelegate NewViewRequested;

    /// <summary>
    /// Dynamically adds a UWKWebView component to a GameObject with initialization that isn't possible using GameObject.AddComponent due
    /// to lack of constructor parameters
    /// </summary>    
    public static UWKWebView AddToGameObject(GameObject gameObject, string url = "", int maxWidth = 1024, int maxHeight = 1024)
    {
        // setup some construction parameters used in Awake method
        createMode = true;
        createURL = url;
        createMaxWidth = maxWidth;
        createMaxHeight = maxHeight;

        // create the view component
        UWKWebView view = gameObject.AddComponent<UWKWebView>();

        // no longer in create mode
        createMode = false;

        return view;
    }

    /// <summary>
    /// Sets the current width and height of the UWKWebView
    /// </summary>    
    public void SetCurrentSize(int width, int height)
    {
		if (currentWidth == width && currentHeight == height)
        {
            return;
        }    

        if (width > MaxWidth)
            width = MaxWidth;

        if (height > MaxHeight)
            height = MaxHeight;

        if (width < 32)
            width = 32;

        if (height < 32)
            height = 32;

        CurrentWidth = width;
        CurrentHeight = height;
		currentWidth = width;
		currentHeight = height;

        UWKPlugin.UWK_MsgSetCurrentSize(ID, width, height);

    }

    /// <summary>
    /// Sets the zoom factor of the page
    /// </summary>    
    public void SetZoomFactor(float zoom)
    {
        UWKPlugin.UWK_MsgSetZoomFactor(ID, zoom);
    }

    /// <summary>
    /// Sets the page's sceoll positon
    /// </summary>    
    public void SetScrollPosition(int x, int y)
    {
        UWKPlugin.UWK_MsgSetScrollPosition(ID, x, y);
    }

    /// <summary>
    /// Globally disabled mouse and keyboard input from webviews
    /// </summary>    
    public static void EnableInput()
    {
        inputDisabled = false;
    }

    /// <summary>
    /// Globally enables mouse and keyboard input from webviews
    /// </summary>    
    public static void DisableInput()
    {
        inputDisabled = true;
    }    

    /// <summary>
    /// Makes the page visible, the page will be updated and refreshed by the Wweb rendering process 
    /// </summary>    
    public void Show()
    {
        visible = true;
        UWKPlugin.UWK_MsgShow(ID, true);
    }

    /// <summary>
    /// Hide the page, the page will no longer be rendered by the web rendering process saving CPU time
    /// </summary>    
    public void Hide()
    {
        visible = false;
        UWKPlugin.UWK_MsgShow(ID, false);
    }

    /// <summary>
    /// Gets whether the page is visible or not
    /// </summary>    
    public bool Visible()
    {
        return visible;
    }   

    /// <summary>
    /// Enabled or disables alpha mask rendering of view
    /// </summary>    
    public void SetAlphaMask(bool enabled)
    {
        UWKPlugin.UWK_MsgSetAlphaMask(ID, enabled);
    }   

    /// <summary>   
    /// Sets the color of the text input caret in the form of 0xAARRGGBB 
    /// Default is opaque black 0xFF000000
    /// </summary>
    public void SetTextCaretColor (uint color)
    {
        UWKPlugin.UWK_MsgSetTextCaretColor(ID, color);
    }

    /// <summary>
    /// Clears all user cookies 
    /// </summary>    
    public static void ClearCookies()
    {
        UWKCore.ClearCookies();
    }   

    /// <summary>
    /// Moves forward in the page history
    /// </summary>    
    public void Forward()
    {
        Navigate(Navigation.Forward);
    }

    /// <summary>
    /// Moves back in the page history
    /// </summary>    
    public void Back()
    {
        Navigate(Navigation.Back);
    }

    /// <summary>
    /// Navigates forward or back in the page history
    /// </summary>    
    public void Navigate(Navigation n)
    {
        UWKPlugin.UWK_MsgNavigate(ID, (int) n);
    }

    /// <summary>
    /// Sets the framerate that the view is rendered at in the web rendering process
    /// Default is 30fps, to set 60fps you would call view.SetFrameRate(60);
    /// Please note that higher fps settings will increase CPU load
    /// </summary>    
    public void SetFrameRate(int framerate)
    {
        UWKPlugin.UWK_MsgSetFrameRate(ID, framerate);
    }

    /// <summary>
    /// Sets the user agent the browser reports, setting the agent to "" 
    /// will use the default uWebKit agent
    /// </summary>
    public void SetUserAgent (string agent = "")
    {
        UWKPlugin.UWK_MsgSetUserAgent(ID, agent);        
    }

    /// <summary>
    /// Navigate the view to the specified URL (http://, file://, etc)
    /// </summary>
    public void LoadURL (string url)
    {
        
        if (url == null || url.Length == 0)     
            return;

        UWKPlugin.UWK_MsgLoadURL(ID, url);        
    }

    /// <summary>
    /// Loads the specified HTML string directly in the view, can be used for generating web content on the fly
    /// </summary>
    public void LoadHTML (string HTML, string baseURL = "")
    {
        UWKPlugin.LoadHTML(ID, HTML, baseURL);
    }

    /// <summary>
    /// Reloads the current page contents
    /// </summary>
    public void Reload ()
    {
        UWKPlugin.UWK_MsgViewReload(ID);
    }

    /// <summary>
    /// Stops the current page load
    /// </summary>
    public void Stop ()
    {
        UWKPlugin.UWK_MsgViewStop(ID);
    }


    /// <summary>
    /// Sends a Javascript message to the page
    /// </summary>
    public void SendJSMessage(string msgName, Dictionary<string,object> msgValues)
    {
        var json = UWKJson.Serialize(msgValues);
        UWKPlugin.SendJSMessage(ID, msgName, json);
    }

    /// <summary>
    /// Sends a Javascript message to the page
    /// </summary>
    public void SendJSMessage(string msgName)
    {
        var json = "{}";
        UWKPlugin.SendJSMessage(ID, msgName, json);
    }

    /// <summary>
    /// Sends a Javascript message to the page
    /// </summary>
    public void SendJSMessage(string msgName, string key, object value)
    {
        var dict = new Dictionary<string,object> ();
        dict[key] = value;
        SendJSMessage(msgName, dict);    
    }

    /// <summary>
    /// Sends a Javascript message to the page
    /// </summary>
    public void SendJSMessage(string msgName, object value)
    {
        var dict = new Dictionary<string,object> ();
        dict["value"] = value;
        SendJSMessage(msgName, dict);    
    }

    /// <summary>
    /// Evaluates Javascript on the page
    /// Example with return value: EvaluateJavascript("document.title", (value) => { Debug.Log(value); });
    /// </summary>
    public void EvaluateJavascript(string javascript, JSEvalDelegate callback = null)
    {
        if (callback != null)
            UWKPlugin.EvaluateJavascript(ID, javascript, (id, value) => { callback(value); });
        else
            UWKPlugin.EvaluateJavascript(ID, javascript);
    }

    /// <summary
    /// Returns the file:// URL of the applications data path
    /// </summary>
    public static string GetApplicationDataURL()
    {
        #if UNITY_STANDALONE_WIN
            return "file:///" + Application.dataPath;       
        #else
            if (Application.isEditor)
                return "file://" + Application.dataPath;
            else
                return "file://" + Application.dataPath + "/Data";
        #endif       
    }

    /// <summary>
    /// Process the mouse given mousePos coordinates
    /// </summary>
    public void ProcessMouse(Vector3 mousePos)    
    {
        if (inputDisabled)
            return;

        //mousePos.y = Screen.height - mousePos.y;      

        if ((int)mousePos.x != lastMouseX || (int)mousePos.y != lastMouseY)
        {
            UWKPlugin.UWK_MsgMouseMove(ID, (int) mousePos.x, (int) mousePos.y);
            lastMouseX = (int) mousePos.x;
            lastMouseY = (int) mousePos.y;
        }
        
        float scroll = Input.GetAxis ("Mouse ScrollWheel");

        if (scroll != 0.0f) 
        {
            #if UNITY_STANDALONE_WIN
            scroll *= 15.0f;
            #else
            scroll *= 1.2f;
            #endif

			scroll *= ScrollSensitivity;

            UWKPlugin.UWK_MsgMouseScroll (ID, lastMouseX, lastMouseY, scroll);
        }        

        for (int i = 0; i < 3; i++) 
        {

            if (Input.GetMouseButtonDown (i)) 
            {
                if (!mouseStates[i])
                {
                    mouseStates[i] = true;
                    UWKPlugin.UWK_MsgMouseButtonDown (ID, (int)mousePos.x, (int)mousePos.y, i);
                }
            }

            if (Input.GetMouseButtonUp (i)) 
            {
                if (mouseStates[i])
                {
                    mouseStates[i] = false;
                    UWKPlugin.UWK_MsgMouseButtonUp (ID, (int)mousePos.x, (int)mousePos.y, i);
                }
            }
        }                   
    }

    /// <summary>
    /// Process a Unity keyboard event
    /// </summary>
    public void ProcessKeyboard(Event keyEvent)
    {

        if (inputDisabled)
            return;        

        UnityKeyEvent uevent = new UnityKeyEvent();            

        uevent.Type = keyEvent.type == EventType.KeyDown ? 1u : 0u;
        uevent.KeyCode = (uint) keyEvent.keyCode;
        uevent.Character = (uint) keyEvent.character;

        // encode modifiers
        uevent.Modifiers = 0;

        if (keyEvent.command)
            uevent.Modifiers |= (uint) UnityKeyModifiers.CommandWin;

        if (keyEvent.alt)
            uevent.Modifiers |= (uint) UnityKeyModifiers.Alt;

        if (keyEvent.control)
            uevent.Modifiers |= (uint) UnityKeyModifiers.Control;

        if (keyEvent.shift)
            uevent.Modifiers |= (uint) UnityKeyModifiers.Shift;

        if (keyEvent.numeric)
            uevent.Modifiers |= (uint) UnityKeyModifiers.Numeric;

        if (keyEvent.functionKey)
            uevent.Modifiers |= (uint) UnityKeyModifiers.FunctionKey;

        if (keyEvent.capsLock)
            uevent.Modifiers |= (uint) UnityKeyModifiers.CapsLock;

        UWKPlugin.UWK_PostUnityKeyEvent(ID, ref uevent);

    }    

	public void DrawTexture(Rect position, bool alphaBlend = true)
	{
		float tw = (float)MaxWidth;
		float th = (float)MaxHeight;

		Rect sourceRect = new Rect(0, 0, CurrentWidth, CurrentHeight);

		sourceRect.x = sourceRect.x / tw ;
		sourceRect.y =  1.0f  -  ( sourceRect.y + sourceRect.height )  / th ;
		sourceRect.width = sourceRect.width / tw ;
		sourceRect.height = sourceRect.height / th ;	

		GUI.DrawTextureWithTexCoords ( position , WebTexture , sourceRect ,  alphaBlend );
	}

    /// <summary>
    /// Sets the icon texture to the given width/height and image bytes
    /// </summary>
    public void IconChanged(int width, int height, byte[] bytes)
    {
        if (WebIcon == null)
            WebIcon = new Texture2D (width, height);

        WebIcon.LoadImage (bytes);
        WebIcon.Apply();
    }

    /// <summary>
    /// Page is focusing a text entry area in IME mode
    /// </summary>
    public void IMEFocusIn(ref UWKMessage msg)
    {
        IMEActive = true;
        IMEInputRect = new Rect(msg.iParams[0], msg.iParams[1], msg.iParams[2], msg.iParams[3]);
        IMEInputType = UWKPlugin.GetMessageString(ref msg, 0);
        IMEText = UWKPlugin.GetMessageString(ref msg, 1);
    }

    /// <summary>
    /// Page is removing focus for text area in IME mode
    /// </summary>
    public void IMEFocusOut()
    {
        IMEActive = false;
    }

    /// <summary>
    /// Opens a platform Web Inspector Window
    /// </summary>
    public void ShowInspector ()
    {
        UWKPlugin.UWK_MsgViewShowInspector(ID);
    }

    /// <summary>
    /// Closes the Web Inspector window Associated with this View
    /// </summary>
    public void CloseInspector ()
    {
        UWKPlugin.UWK_MsgViewCloseInspector(ID);
    }


    #region Default delegate handlers

    void loadFinished(UWKWebView view)
    {
        // Dictionary<string, object> values = new Dictionary<string, object>();
        // values["Whee"] = 42;
        // SendJSMessage("testMessage", values);
    }

    void loadProgress(UWKWebView view, int progress)
    {

    }

    void urlChanged(UWKWebView view, string url)
    {

    }

    void newViewRequested(UWKWebView view, string url)
    {
        // Default handler loads in this view

        LoadURL(url);   

    }

    void titleChanged(UWKWebView view, string title)
    {
        Title = title;
    }

    void jsConsole(UWKWebView view, string message, int line, string source)
    {
        Debug.Log("Javascript: " + message + " " + line + " " + source);
    }

    void jsMessageReceived(UWKWebView view, string message, string json, Dictionary<string, object> values)
    {
        // Debug.Log("Javascript Message: " + message + " " + value);
    }

    void contentSizeChanged(UWKWebView view, int width, int height)
    {
        contentWidth = width;
        contentHeight = height;
    }

    #endregion

    /// <summary>
    /// Initializes the UWKWebView, registers default delagates, and creates textures for the page and icon
    /// to lack of constructor parameters
    /// </summary>    
    void Awake()
    {
        // ensure core is up
        UWKCore.Init();

        if (createMode)
        {
            URL = createURL;
            MaxWidth = createMaxWidth;
            MaxHeight = createMaxHeight;
			CurrentWidth = MaxWidth;
			CurrentHeight = MaxHeight;
        }

        if (MaxWidth < 64)
            MaxWidth = 64;

        if (MaxHeight < 64)
            MaxHeight = 64;

		if (CurrentWidth > MaxWidth)
        	CurrentWidth = MaxWidth;
		if (CurrentHeight > MaxHeight)
       		CurrentHeight = MaxHeight;

		maxWidth = MaxWidth;
		maxHeight = MaxHeight;

        // default delegate handlers
        LoadFinished += loadFinished;
        URLChanged += urlChanged;
        TitleChanged += titleChanged;
        JSConsole += jsConsole;
        JSMessageReceived += jsMessageReceived;
        LoadProgress += loadProgress;
        ContentSizeChanged += contentSizeChanged;
        NewViewRequested += newViewRequested;

        TextureFormat format = TextureFormat.ARGB32;

        if (SystemInfo.graphicsDeviceVersion.IndexOf("Direct3D 11") != -1)
        {
            format = TextureFormat.BGRA32;
        }

        // note that on Direct3D11 shared gpu textures, mipmapping is not allowed
        WebTexture = new Texture2D( MaxWidth, MaxHeight, format, false);

        Color32[] colors = new Color32[MaxWidth * MaxHeight];

        for (int i = 0; i < MaxWidth * MaxHeight; i++)
        {
            colors[i]. r = colors[i].g = colors[i].b = colors[i]. a = 0;
        }

        WebTexture.SetPixels32(colors);
        WebTexture.Apply();

        ID = UWKCore.CreateView(this, MaxWidth, MaxHeight, "", WebTexture.GetNativeTexturePtr());      

    }   

    void Start()
    {
        
        if (CurrentWidth != MaxWidth || CurrentHeight != MaxHeight) 
		{
			SetCurrentSize (CurrentWidth, CurrentHeight);
		}
		else 
		{
			currentWidth = MaxWidth;
			currentHeight = MaxHeight;
		}

        LoadURL(URL);

    }

	void Update()
	{
		if (CurrentWidth != currentWidth || CurrentHeight != currentHeight)
			SetCurrentSize (CurrentWidth, CurrentHeight);

		// in case changed, MaxWidth and MaxHeight are defined at creation 
		MaxWidth = maxWidth;
		MaxHeight = maxHeight;

	}

    void OnDestroy()
    {
        UWKCore.DestroyView(this);

        if (WebIcon != null)
        {
            UnityEngine.Object.Destroy(WebIcon);
            WebIcon = null;
        }

        if (WebTexture != null)
        {
            UnityEngine.Object.Destroy(WebTexture);
            WebTexture = null;
        }

    }


    #region IME Support

    /// <summary>
    /// Draws the text IME for Chinese, Japanese, Korean languages
    /// </summary>
    public void DrawTextIME (int x, int y)
    {
        if (!IMEActive)
            return;
        
        GUI.SetNextControlName ("UWK_IMETextField");

        Rect t = new Rect (x + IMEInputRect.x, y + IMEInputRect.y, IMEInputRect.width, IMEInputRect.height);

        // GUI.TextField && GUI.PasswordField not returning the IME character with the _ under it
        
        string currentIME = "";
        
        if (IMEInputType != "password")
            currentIME = GUI.TextField (t, IMEText);
        else
            currentIME = GUI.PasswordField (t, IMEText, "*" [0]);
        

        if (currentIME != IMEText) 
        {
            IMEText = currentIME;
            UWKPlugin.SetIMEText(ID, IMEText);
        }
        
        GUI.FocusControl ("UWK_IMETextField");
                    
    }    
 
    /// <summary>
    /// Whether the IME input text field is currently active
    /// </summary>            
    [HideInInspector]
    public bool IMEActive = false;

    /// <summary>
    /// The rect of the IME input text field
    /// </summary>                
    [HideInInspector]
    public Rect IMEInputRect;

    /// <summary>
    /// The current text entry type, "password" will input '*' 
    /// </summary>            
    [HideInInspector]
    public string IMEInputType = "";

    /// <summary>
    /// The current text value of the IME text entry field
    /// </summary>            
    [HideInInspector]
    public string IMEText = "";

    #endregion

    #region Private Fields    

    // we need to track mouse states and Unity's OnGUI method method may be called more than once
    bool[] mouseStates = new bool[3] {false, false, false};    

    int lastMouseX = -1;
    int lastMouseY = -1;
	
    int contentWidth = 0;
    int contentHeight = 0;

    // since Unity can't do getters/setters as inspector fields
    int currentHeight;
    int currentWidth;

	int maxHeight;
	int maxWidth;


    bool visible = true;

    // create mode construction paramaters
    static bool createMode = false;
    static int createMaxWidth;
    static int createMaxHeight;
    static string createURL;

    static bool inputDisabled = false;

    #endregion


}
