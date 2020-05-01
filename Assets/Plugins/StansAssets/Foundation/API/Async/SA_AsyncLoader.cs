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

namespace SA.Foundation.Async {

	public static class SA_AsyncLoader  {

		/// <summary>
		/// Asynchronously loads image from web and converts it to the Texture2D object
		/// <param name="url">Texture web url</param>
		/// <param name="callback">Texture load callback</param>
		/// </summary>
		public static void LoadWebTexture(string url,  Action<Texture2D> callback) {
			var loader = SA_WWWTextureLoader.Create();

			loader.OnLoad += callback;
			loader.LoadTexture(url);
		}


        /// <summary>
        /// Asynchronously loads local prefab  and converts it to the GameObject
        /// <param name="localPath">Local prefab path</param>
        /// <param name="callback">Prefab load callback</param>
        /// </summary>
    
        public static void LoadResource<T>(string path, Action<T> callback) where T : UnityEngine.Object {
            SA_Coroutine.Start(ResourceLoadCoroutine<T>(path, callback));
        }




        private static IEnumerator ResourceLoadCoroutine<T>(string path, Action<T> callback) where T : UnityEngine.Object {
            ResourceRequest request = Resources.LoadAsync<GameObject>(path);

            yield return request;
            if (request.asset == null) {
                Debug.LogWarning("Resource not found at path: " + path);
                callback(null);
            } else {
                T resource = UnityEngine.Object.Instantiate(request.asset) as T;
                callback(resource);
            }
        }

    }

}