////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;

using SA.Foundation.Async;

namespace SA.Foundation.Utility {


	public static class SA_ScreenUtil  {


        /// <summary>
        /// Takes a screenshot.
        /// </summary>
        /// <param name="callback"> Result callback.</param> 
		public static void TakeScreenshot(Action<Texture2D> callback) {
            SA_Coroutine.Start(TakeScreenshotCoroutine(callback));
		}



        private static IEnumerator TakeScreenshotCoroutine(Action<Texture2D> callback) {

            yield return new WaitForEndOfFrame();
            // Create a texture the size of the screen, RGB24 format
            int width =  UnityEngine.Screen.width;
            int height = UnityEngine.Screen.width; ;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

            // Read screen contents into the texture
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            callback.Invoke(tex);

        }

    }

}

