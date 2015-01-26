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
/// JSObject Allows you to create Javascript objects which are accessible from web content and persistent across page loads
/// See UnityInfoPage.cs for example usage.  Please note that the JSObjects are unidirectional from Unity -> Javascript
/// If you need bidirectional communication use the UWKWebView.SendJSMessage method and JSMessageReceived delegate
/// </summary>
public class JSObject
{

	/// <summary>
	/// Sets property on a named Javascript object a the string value, possibly creating the object
	/// if it doesn't exist, example: 
	/// JSObject.SetProperty("MyObject", "myStringValue", "Hello");
	/// available is MyObject.myStringValue in the page's javascript
	/// </summary>
	public static void SetProperty(string objectName, string propName, string value)
	{
		UWKPlugin.SetJSObjectProperty(objectName, propName, value);
	}

	/// <summary>
	/// Sets property on a named Javascript object a the string value, possibly creating the object
	/// if it doesn't exist, example: 
	/// JSObject.SetProperty("MyObject", "myIntValue", 42);
	/// available is MyObject.myIntValue in the page's javascript
	/// </summary>
	public static void SetProperty (string objectName, string propName, int value)
	{
		SetProperty (objectName, propName, value.ToString ());
	}

	/// <summary>
	/// Sets property on a named Javascript object a the string value, possibly creating the object
	/// if it doesn't exist, example: 
	/// JSObject.SetProperty("MyObject", "myFloatValue", 1.1);
	/// available is MyObject.myFloatValue in the page's javascript
	/// </summary>
	public static void SetProperty (string objectName, string propName, float value)
	{
		SetProperty (objectName, propName, value.ToString ());
	}
	
	/// <summary>
	/// Sets property on a named Javascript object a the string value, possibly creating the object
	/// if it doesn't exist, example: 
	/// JSObject.SetProperty("MyObject", "myBoolValue", true);
	/// available is MyObject.myBoolValue in the page's javascript
	/// </summary>
	public static void SetProperty (string objectName, string propName, bool value)
	{
		SetProperty (objectName, propName, value.ToString ());
	}

	/// <summary>
	/// Removes the named object from the page's javascript
	/// </summary>
	public static void Remove(string objectName)
	{
		UWKPlugin.RemoveJSObject(objectName);		
	}

}