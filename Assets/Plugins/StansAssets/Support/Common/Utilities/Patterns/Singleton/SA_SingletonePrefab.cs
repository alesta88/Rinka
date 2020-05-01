////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;



namespace SA.Common.Pattern
{
  
    public abstract class SingletonePrefab<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T s_instance = null;

        public static T Instance {

            get {
                if (s_instance == null) {
                    s_instance = FindObjectOfType(typeof(T)) as T;
                    if (s_instance == null) {
                        GameObject prefab = UnityEngine.Object.Instantiate(Resources.Load(typeof(T).Name)) as GameObject;
                        s_instance = prefab.GetComponent<T>();
                        DontDestroyOnLoad(prefab);
                    }
                }

                return s_instance;
            }
        }

        public static bool HasInstance {
            get {
                return s_instance != null;
            }
        }
    }
    
}