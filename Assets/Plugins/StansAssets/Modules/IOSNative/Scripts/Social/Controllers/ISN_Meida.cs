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

    public static class Meida
    {

        public static event Action<Result> OnShareResult = delegate { };

        //--------------------------------------
        //  PUBLIC METHODS
        //--------------------------------------


        public static void Share(string message, Action<Result> callback = null) {
            Share(message, new Texture2D[] {}, callback);
        }

        public static void Share(Texture2D image, Action<Result> callback = null) {
            Share(null, new Texture2D[] { image }, callback);
        }

        public static void Share(Texture2D[] images, Action<Result> callback = null) {
            Share(null, images, callback);
        }

        public static void Share(string message, Texture2D image, Action<Result> callback = null) {
            Share(message, new Texture2D[] { image }, callback);
        }


        public static void Share(string message, Texture2D[] images, Action<Result> callback = null) {
            if (message == null) { message = string.Empty; }

            if (callback != null) {
                OnShareResult += callback;
            }

            List<string> media = new List<string>();
            foreach (Texture2D image in images) {
                byte[] val = image.EncodeToPNG();
                media.Add(Convert.ToBase64String(val));
            }

            string encodedMedia = Converter.SerializeArray(media.ToArray());

            Internal.ISN_MediaShare(message, encodedMedia);
        }


        //--------------------------------------
        //  SUPPORT CLASSES
        //--------------------------------------



        private static class Internal {

            #if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
            [DllImport ("__Internal")]
            private static extern void _ISN_MediaShare(string text, string encodedMedia);
            #endif

            public static void ISN_MediaShare(string text, string encodedMedia) {
                #if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
                _ISN_MediaShare(text, encodedMedia);
                #endif
            }

        }



    }
}
