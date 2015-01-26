using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;


using System;

/// <summary>
/// Internal class used for product activation
/// </summary>
public class UWKActivation : MonoBehaviour
{

	static Rect windowRect = new Rect (0, 0, 400, 300);

	GUIStyle largeFontLabel;
	GUIStyle largeFontTextField;

	void Awake ()
	{
		// ensure Core is up
		UWKCore.Init ();
	}

	void Start()
	{
		Center ();

		//UWKWebView.DisableInput();	
	}


	bool validateKey (string key)
	{

		if (!key.StartsWith ("P-") && !key.StartsWith ("S-")) {
			return false;
		}
		
		if (key.Length != 21)
			return false;
		
		int count = 0;
		foreach (char c in key)
			if (c == '-')
				count++;
		
		if (count != 4)
			return false;
		
		return true;
	}


	public static void SetActivationState(int state)
	{
		activationState = 3;
		activating = false;

		if (!showActivationMessage)
			return;
			
		// act1 or act2
		if (activationState == 1) 
		{
			activating = false;
			activateWindow = false;			
			EditorUtility.DisplayDialog ("uWebKit Activated", "Thank you!", "Ok");			
			EditorApplication.ExecuteMenuItem ("Edit/Play");			
		}

		else if (activationState == 2) 
		{
			activating = false;
			EditorUtility.DisplayDialog ("uWebKit Activation", "This key is invalid, please check the key and try again.\n", "Ok");
		}

		else if (activationState == 4) 
		{
			// no activations			
			activating = false;
			EditorUtility.DisplayDialog ("uWebKit Activation Failed", "Activation Count exceeded, please contact sales@uwebkit.com for more information", "Ok");
		}

		else if (activationState == 5) 
		{
			// problem
			activating = false;
			activateWindow = false;		
			EditorUtility.DisplayDialog ("uWebKit Activation", "There was an issue contacting the Activation Server.\n\nThe product is available, however you may be asked to activate again.", "Ok");			
			EditorApplication.ExecuteMenuItem ("Edit/Play");			
		}
		else if (activationState == 3)
		{
			return;
		}

	}


	void windowFunction (int windowID)
	{
		Rect titleRect = new Rect (0, 0, 400, 24);
		
		if (!activating) {

			GUILayout.BeginVertical ();
						
			GUILayout.Space (8);	
			
			GUILayout.Label ("You can evaluate uWebKit without activating, select Activate Later below.\n\nuWebKit has 2 activations per product key.  Activation is a one time process per machine.  Email sales@uwebkit.com with any issues\n\nIMPORTANT:\n\n1) Please ensure Build Settings are set to PC/Mac Standalone before activating\n\n2) If you are behind a web proxy, edit UWKConfig.cs with proxy config");
						
			GUILayout.Space (16);			
			
			GUILayout.BeginHorizontal ();			
			GUILayout.Label ("Please Enter Activation Key:", largeFontLabel);
			GUILayout.EndHorizontal ();
			
			GUILayout.BeginHorizontal ();
			activationCode = GUILayout.TextField (activationCode, 64, largeFontTextField, GUILayout.Width (280)).Trim ();						
			GUILayout.EndHorizontal ();
			
			GUILayout.Space (32);

			GUILayout.BeginHorizontal ();
			
			if (GUILayout.Button ("Activate", GUILayout.Height (64))) 
			{

				if (!validateKey (activationCode)) 
				{
					EditorUtility.DisplayDialog ("uWebKit Activation", "This key is invalid, please check the key and try again.\n", "Ok");
				} 
				else 
				{
					showActivationMessage = true;
					activating = true;
					UWKPlugin.UWK_MsgActivate(activationCode);
				}
			}

			GUILayout.EndHorizontal ();

			GUILayout.Space (16);

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Purchase", GUILayout.Height (64))) {
				
				Application.OpenURL ("http://www.uwebkit.com/store");
				
			}

			if (GUILayout.Button ("Activate Later", GUILayout.Height (64))) {
				activateLater = true;
				UWKWebView.EnableInput();
			}

			GUILayout.EndHorizontal ();
			
			GUILayout.EndVertical ();			

			
		} else {
			GUILayout.Label ("Activating... Please Wait");
		}
		
		GUI.DragWindow (titleRect);
		
	}

	void OnGUI ()
	{
		if (activateLater)
			return;

		if (largeFontLabel == null)
		{
			largeFontLabel = new GUIStyle("label");
        	largeFontTextField = new GUIStyle("textfield");

        	largeFontLabel.fontSize = 16;		
			largeFontTextField.fontSize = 16;

		}

		if (activateWindow)
		{
			windowRect = GUILayout.Window (activationWindowID, windowRect, windowFunction, "uWebKit Activation");
			GUI.BringWindowToFront(activationWindowID);
			GUI.FocusWindow(activationWindowID);

		}
	}


	void Update ()
	{

	}

	void reset ()
	{
		activating = false;
		activateWindow = true;
		Center ();
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

	static int activationWindowID = -2999;

	string activationCode = "";
	static bool activating = false;
	static bool activateWindow = true;
	static bool activateLater = false;
	static bool showActivationMessage = false;
	static int activationState = -1;

}

#endif

