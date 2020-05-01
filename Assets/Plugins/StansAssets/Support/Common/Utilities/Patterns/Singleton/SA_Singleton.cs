////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

namespace SA.Common.Pattern {

    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        private static T s_instance = null;
        private static bool s_isApplicationIsQuitting = false;
      


        //--------------------------------------
        //  INITIALIZATION
        //--------------------------------------

        protected virtual void Awake() {
            DontDestroyOnLoad(gameObject);
            if(gameObject.transform.parent == null) {
                gameObject.transform.SetParent(SingletonParent.Transform);
            }
        }

        public static T Instance {
            get {
                if (s_isApplicationIsQuitting) {
                    //Debug.Log(typeof(T) + " [Mog.Singleton] is already destroyed. Returning null. Please check HasInstance first before accessing instance in destructor.");
                    return null;
                }
                Instantiate();
                return s_instance;
            }
        }


        //--------------------------------------
        //  PUBLIC METHODS
        //--------------------------------------


        /// <summary>
        /// This methods is created in case you just want Instance to exists
        /// but don't really need to call any class methods
        /// this cab be usful if your class can receive message by gameoject name
        /// without direclty refering to a class instance
        /// </summary>
        public static void Instantiate() {
            if (s_instance == null) {
                s_instance = Object.FindObjectOfType(typeof(T)) as T;
                if (s_instance == null) {
                    s_instance = new GameObject().AddComponent<T>();
                    s_instance.gameObject.name = s_instance.GetType().FullName;
                }
            }
        }


        //--------------------------------------
        //  GET / SET
        //--------------------------------------

        public static bool HasInstance {
            get {
                return !IsDestroyed;
            }
        }

        public static bool IsDestroyed {
            get {
                if (s_instance == null) {
                    return true;
                } else {
                    return false;
                }
            }
        }


        //--------------------------------------
        //  UNITY ACTION HANDLERS
        //--------------------------------------


		/// <summary>
		/// When Unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when application quits.
		/// If any script calls Instance after it have been destroyed, 
		///   it will create a buggy ghost object that will stay on the Editor scene
		///   even after stopping playing the Application. Really bad!
		/// So, this was made to be sure we're not creating that buggy ghost object.
		/// </summary>
		protected virtual void OnDestroy () {
			s_instance = null;
			s_isApplicationIsQuitting = true;
			//Debug.Log(typeof(T) + " [Mog.Singleton] instance destroyed with the OnDestroy event");
		}
		
		protected virtual void OnApplicationQuit () {
			s_instance = null;
			s_isApplicationIsQuitting = true;
			//Debug.Log(typeof(T) + " [Mog.Singleton] instance destroyed with the OnApplicationQuit event");
		}

	}



    static class SingletonParent
    {
        private static Transform s_transform = null;


        public static Transform Transform {
            get {
                if (s_transform == null) {
                    var go = new GameObject("Singletons");
                    Object.DontDestroyOnLoad(go);
                    s_transform = go.transform;
                }
                return s_transform;
            }
        }
    }

}
