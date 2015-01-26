#if UNITY_EDITOR
#if !UNITY_WEBPLAYER

// We don't want to add a top level "Editor" folder for the uWebKit package
// so. we preprocess guard instead

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public static class UWKDeploySupport
{
	
    static void RemovePaths(List<string> paths)
    {
        foreach (var _path in paths)
        {
            string path = _path;
            
            if (Application.platform != RuntimePlatform.OSXEditor)
                path = path.Replace("/", "\\");
            
            if (!Directory.Exists(path))
                Debug.LogError("Unable to remove deployment path: " + path);
            
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            
            if (Directory.Exists(path))
                Debug.LogError("Unable to remove deployment path: " + path);    
        }
    }
    
    [PostProcessBuild] 
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (path.Contains(".exe"))
            path = path.Replace(".exe", "_Data");
        
        bool windowsBuild = target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64;
        bool linuxBuild = target == BuildTarget.StandaloneLinux || target == BuildTarget.StandaloneLinux64 || target == BuildTarget.StandaloneLinuxUniversal;
        bool osxBuild = target == BuildTarget.StandaloneOSXIntel || target == BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneOSXUniversal;

        #if UNITY_5_0
        bool iosBuild = target == BuildTarget.iOS;
        #else
        bool iosBuild = target == BuildTarget.iPhone;
        #endif

        // build the target-specific streaming asset directories used for deployment
        List<string> assetDirectories = new List<string>()
        {
            "uWebKit/Mac/x86",
            "uWebKit/Mac/x86_64",
            "uWebKit/Windows/x86",
            "uWebKit/Windows/x86_64"
        };
        
        string assetDirectoryPrefix = string.Empty;
        
        if (windowsBuild || linuxBuild)
        {
            assetDirectoryPrefix = path + "/StreamingAssets/";
        }
        else if (osxBuild)
        {
            assetDirectoryPrefix = path + "/Contents/Data/StreamingAssets/";
        }
        else if (iosBuild)
        {
            assetDirectoryPrefix = path + "/Data/Raw/";
        }
        else
        {
            // log an error but at least allow compiling to continue by preventing further errors
            Debug.LogError("Unable to remove deployment paths for uWebKit on this platform");
            return;
        }
        
        List<string> deployPathsToDelete = new List<string>();
        foreach (string assetDirectory in assetDirectories)
        {
            deployPathsToDelete.Add(assetDirectoryPrefix + assetDirectory);
        }
        
        // protect the directory for the current target by removing it from the deletion list
        if (target == BuildTarget.StandaloneWindows)
        {
            deployPathsToDelete.Remove(path + "/StreamingAssets/uWebKit/Windows/x86");
        }
        else if (target == BuildTarget.StandaloneWindows64)
        {
            deployPathsToDelete.Remove(path + "/StreamingAssets/uWebKit/Windows/x86_64");
        }
        else if (target == BuildTarget.StandaloneOSXIntel)
        {
            deployPathsToDelete.Remove(path + "/Contents/Data/StreamingAssets/uWebKit/Mac/x86");
        }
        else if (target == BuildTarget.StandaloneOSXIntel64)
        {   
            deployPathsToDelete.Remove(path + "/Contents/Data/StreamingAssets/uWebKit/Mac/x86_64");
        }
        
        RemovePaths(deployPathsToDelete);

		// save off config for data that Unity does not make available at runtime
		string cfgpath = assetDirectoryPrefix + "/uWebKit/Config";
		if (!Directory.Exists (cfgpath))
			Directory.CreateDirectory (cfgpath);;
		
		Dictionary<string,object> jconfig = new Dictionary<string,object> ();
		jconfig["companyName"] = PlayerSettings.companyName;
		jconfig["productName"] = PlayerSettings.productName;
		var json = UWKJson.Serialize(jconfig);
		
		using (StreamWriter cfgfile = new StreamWriter(cfgpath + "/uwebkit.cfg"))
		{
			cfgfile.Write(json);
		}

    }
}

#endif
#endif