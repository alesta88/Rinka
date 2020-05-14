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

    public static class Instagram
    {
        public static event Action OnPostStart = delegate { };
        public static event Action<Result> OnPostResult = delegate { };


        //--------------------------------------
        //  INITIALIZATION
        //--------------------------------------

        static Instagram() {
            NativeListener.Instantiate();
        }

        //--------------------------------------
        //  PUBLIC METHODS
        //--------------------------------------


        public static void Post(Texture2D image, Action<Result> callback = null) {
            Post(image, null, callback);
        }


        public static void Post(Texture2D image, string message, Action<Result> callback = null) {
           
            if (message == null) { message = string.Empty; }

            if (callback != null) {
                OnPostResult += callback;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                OnPostStart();
            }
        
            byte[] val = image.EncodeToPNG();
            string encodedMedia = Convert.ToBase64String (val);

      
            Internal.ISN_InstaShare(encodedMedia, message);
  
        }


        //--------------------------------------
        //  SUPPORT CLASSES
        //--------------------------------------



        private class NativeListener : Singleton<NativeListener> {
            
            private void OnInstaPostSuccess() {
                Result result = new Result();
                OnPostResult(result);
            }


            private void OnInstaPostFailed(string data) {
                int code = Convert.ToInt32(data);

                Error error = new Error(code, "Posting Failed");
                Result result = new Result(error);
                OnPostResult(result);

            }
        }


        private static class Internal {

            #if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
            [DllImport ("__Internal")]
            private static extern void _ISN_InstaShare(string encodedMedia, string message);
            #endif

            public static void ISN_InstaShare(string encodedMedia, string message) {
                #if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
                _ISN_InstaShare(encodedMedia, message);
                #endif
            }
        }



    }
}
