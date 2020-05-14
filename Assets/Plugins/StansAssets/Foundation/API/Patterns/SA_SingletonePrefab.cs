////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;


namespace SA.Foundation.Patterns
{

    /// <summary>
    /// Class can be used for treating some prefabs as singletones.
    /// Class name should match a prefab name inside the Resources folder
    /// </summary>
    public abstract class SA_SingletonePrefab<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _Instance = null;


        /// <summary>
        /// Returns a singleton class instance
        /// If current instance is not assigned it will try to find an object of the instance type,
        /// in case instance already exists on a scene. If not, new instance will be created.
        /// </summary>
        public static T Instance {
            get {
                if (_Instance == null) {
                    _Instance = FindObjectOfType(typeof(T)) as T;
                    if (_Instance == null) {


                        GameObject prefab = UnityEngine.Object.Instantiate(Resources.Load(typeof(T).Name)) as GameObject;
                        _Instance = prefab.GetComponent<T>();
                        DontDestroyOnLoad(prefab);
                    }
                }

                return _Instance;
            }
        }


        /// <summary>
        /// True if Singleton Instance exists
        /// </summary>
        public static bool HasInstance {
            get {
                return _Instance != null;
            }
        }
    }
    
}