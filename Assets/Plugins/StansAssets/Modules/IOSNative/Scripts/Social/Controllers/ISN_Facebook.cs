#define SOCIAL_API
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections.Generic;
#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

using SA.Common.Data;
using SA.Common.Pattern;
using SA.Common.Models;


namespace SA.IOSNative.Social
{

    public static class Facebook
    {
        public static event Action OnPostStart = delegate { };
        public static event Action<Result> OnPostResult = delegate { };


        //--------------------------------------
        //  INITIALIZATION
        //--------------------------------------

        static Facebook() {
            NativeListener.Instantiate();
        }

        //--------------------------------------
        //  PUBLIC METHODS
        //--------------------------------------

      
        public static void Post(string text, Action<Result> callback = null) {
            Post(text, null, new Texture2D[] { }, callback);
        }

        public static void Post(Texture2D image, Action<Result> callback = null) {
            Post(null, null, new Texture2D[] { image }, callback);
        }

        public static void Post(Texture2D[]  images, Action<Result> callback = null) {
            Post(null, null, images, callback);
        }

        public static void Post(string text, string url, Action<Result> callback = null) {
            Post(text, url, new Texture2D[] { }, callback);
        }

        public static void Post(string text, Texture2D image, Action<Result> callback = null) {
            Post(text, null, new Texture2D[] {image}, callback);
        }

        public static void Post(string text, Texture2D[] images, Action<Result> callback = null) {
            Post(text, null, images, callback);
        }

        public static void Post(string text, string url, Texture2D image, Action<Result> callback = null) {
            Post(text, url, new Texture2D[] { image }, callback);
        }


        public static void Post(string text, string url, Texture2D[] images, Action<Result> callback = null) {

            if (url == null) { url = string.Empty; }
            if (text == null) { text = string.Empty; }
            if(images == null) { images = new Texture2D[] { }; }

            if(callback != null) {
                OnPostResult += callback;
            }

            if(Application.platform == RuntimePlatform.IPhonePlayer) {
                OnPostStart();
            }

            List<string> media = new List<string>();
            foreach(Texture2D image in images) {
                byte[] val = image.EncodeToPNG();
                media.Add(Convert.ToBase64String(val));
            }

            string encodedMedia = Converter.SerializeArray(media.ToArray());
            Internal.ISN_FbPost(text, url, encodedMedia);
        }



        //--------------------------------------
        //  SUPPORT CLASSES
        //--------------------------------------



       


        private class NativeListener : Singleton<NativeListener> {
            
            private void OnFacebookPostFailed() {
                SA.Common.Models.Result result = new SA.Common.Models.Result(new SA.Common.Models.Error());
                OnPostResult(result);
            }

            private void OnFacebookPostSuccess() {
                SA.Common.Models.Result result = new SA.Common.Models.Result();
                OnPostResult(result);
            }
        }


        private static class Internal {

            #if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
            [DllImport ("__Internal")]
            private static extern void _ISN_FbPost(string text, string url, string encodedMedia);
            #endif


            public static void ISN_FbPost(string text, string url, string encodedMedia) {
                #if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
                _ISN_FbPost(text, text, encodedMedia);
                #endif
            }
        }



    }
}
