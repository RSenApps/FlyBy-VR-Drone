using UnityEngine;
using System.Collections;
	
// UWKConfig is used to configure uWebKit settings
// such as proxy support
public static class UWKConfig
{
			
	//  Set proxy information here
	
	/// <summary>
	/// enable disable proxy support
	/// </summary>
	public static bool ProxyEnabled = false;

	/// <summary>
	/// The proxy hostname.
	/// </summary>
	public static string ProxyHostname = "";
	/// <summary>
	/// The proxy port.
	/// </summary>
	public static int ProxyPort = 0;

	// define if necessary, leave empty otherwise
	
	/// <summary>
	/// The proxy username.
	/// </summary>
	public static string ProxyUsername = "";
	
	/// <summary>
	/// The proxy password.
	/// </summary>
	public static string ProxyPassword = "";
	
	//  Set Auth information here
	
	public static bool AuthEnabled = false;
	
	public static string AuthUsername = "";
	
	public static string AuthPassword = "";		
			
	
}
	

