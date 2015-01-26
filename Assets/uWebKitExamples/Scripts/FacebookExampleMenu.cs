/******************************************
  * uWebKit 
  * (c) 2014 THUNDERBEAST GAMES, LLC
  * http://www.uwebkit.com
  * sales@uwebkit.com
*******************************************/

using UnityEngine;
using System.Collections;
public class FacebookExampleMenu : MonoBehaviour
{

	void Start()
	{

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

		if (SourceCodePopup.usePopup)
		if (GUI.Button (brect, "View Source")) 
		{	
			if (gameObject.GetComponent<SourceCodePopup>() == null)
			{
				sourcePopup = gameObject.AddComponent<SourceCodePopup>(); 		
				sourcePopup.URL = "https://github.com/uWebKit/uWebKit/tree/uWebKit2-Beta/uWebKit/Assets/uWebKitExamples/Facebook";
			}
			else
			{
				gameObject.SendMessage("SourcePopupClosed");
			}
		}		

	}

	void SourcePopupClosed()
	{
		UnityEngine.Object.Destroy(gameObject.GetComponent<SourceCodePopup>());		
	}

	SourceCodePopup sourcePopup;

	
}