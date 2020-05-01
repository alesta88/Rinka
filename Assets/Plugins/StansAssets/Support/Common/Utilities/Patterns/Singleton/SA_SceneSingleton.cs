////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace SA.Common.Pattern {

	public abstract class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour {

        private static T s_instance = null;


		public static T Instance {
			get {
                if (s_instance == null) {
                    s_instance = Object.FindObjectOfType(typeof(T)) as T;
                    if (s_instance == null) {
                        s_instance = new GameObject ().AddComponent<T> ();
                        s_instance.gameObject.name = s_instance.GetType ().FullName;
					}
				}
                return s_instance;
			}
		}


        public static bool HasInstance {
            get {
                if (s_instance == null) {
                    return true;
                } else {
                    return false;
                }
            }
        }


	}

}
