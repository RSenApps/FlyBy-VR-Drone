using UnityEngine;
using System.Collections;
using System;

public class PNGLoader : MonoBehaviour {
	// Use this for initialization
	public string url = "file:///c://Users/Ryan/test.jpg";
	IEnumerator Start() {
		while(true) {

				// Start a download of the given URL
				WWW www = new WWW(url);
				
				// wait until the download is done
				yield return www;
			if (!(www.texture.height == 8 && www.texture.width == 8))
			{
				// assign the downloaded image to the main texture of the object
				renderer.material.mainTexture = www.texture;
			}

		}

	}
}
