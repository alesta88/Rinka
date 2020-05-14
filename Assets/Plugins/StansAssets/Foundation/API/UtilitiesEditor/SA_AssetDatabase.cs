using System.IO;
using UnityEngine;
using SA.Foundation.Utility;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SA.Foundation.UtilitiesEditor
{
    
    public static class SA_AssetDatabase
    {
     

        /// <summary>
        /// Creates a new asset at path.
        /// </summary>
        /// <param name="asset"> Object to use in creating the asset. </param> 
        /// <param name="path"> Filesystem "Asset" folder relative path.</param> 
        public static void CreateAsset(Object asset, string path) {
            path = FixRelativePath(path);
            SA_AssetDatabaseProxy.Create(asset, path);
        }


        /// <summary>
        ///  Returns the path name relative to the project folder where the asset is stored.
        ///  All paths are relative to the project folder, for example: "Assets/Plugins/MyTextures/hello.png".
        /// </summary>
        /// <param name="assetObject"> A reference to the asset. </param> 
        public static string GetProjectFolderRelativePath(Object assetObject) {
            return SA_AssetDatabaseProxy.GetAssetPath(assetObject);
        }

        /// <summary>
        ///  Returns the path name relative to the Assets/ folder where the asset is stored.
        ///  All paths are relative to the project folder, for example: "Plugins/MyTextures/hello.png".
        /// </summary>
        /// <param name="assetObject"> A reference to the asset. </param> 
        public static string GetAssetFolderRelativePath(Object assetObject) {
            return FixRelativePath(SA_AssetDatabaseProxy.GetAssetPath(assetObject), false);
        }


        /// <summary>
        ///  Returns the absolute path name relative of a given asset
        ///  for example: "/Users/user/Project/Assets/MyTextures/hello.png".
        /// </summary>
        /// <param name="assetObject"> A reference to the asset. </param> 
        public static string GetAbsoluteAssetPath(Object assetObject) {
            string relativePath = GetAssetFolderRelativePath(assetObject);
            return SA_PathUtil.ConvertRelativeToAbsolutePath(relativePath);
        }


        /// <summary>
        /// Duplicates the asset at path and stores it at newPath.
        /// Returns true if the asset has been successfully duplicated, false if it doesn't exit or couldn't be duplicated.
        /// All paths are relative to the project folder, for example: "Plugins/IOS/hello.png".
        /// </summary>
        /// <param name="path"> Filesystem "Asset" source relative path. </param> 
        /// <param name="newPath"> Filesystem "Asset" destination relative path.</param> 
        public static bool CopyAsset(string path, string newPath) {
            newPath = FixRelativePath(newPath);
            return SA_AssetDatabaseProxy.Copy(path, newPath);
        }


        /// <summary>
        /// Move an asset file (or folder) from one folder to another.
        /// Returns an empty string if the asset has been successfully moved, otherwise an error message.
        /// All paths are relative to the project Asset folder, for example: "Plugins/IOS/hello.png".
        /// </summary>
        /// <param name="oldPath"> Filesystem "Asset" source relative path. </param> 
        /// <param name="newPath"> Filesystem "Asset" destination relative path.</param> 
        public static string MoveAsset(string oldPath, string newPath) {
            oldPath = FixRelativePath(oldPath);
            newPath = FixRelativePath(newPath);
            return SA_AssetDatabaseProxy.Move(oldPath, newPath);
        }


        /// <summary>
        /// Moves the asset at path to the trash.
        /// Returns true if the asset has been successfully removed, false if it doesn't exit or couldn't be moved to the trash. 
        /// All paths are relative to the project folder, for example: "Plugins/IOS/hello.png".
        /// </summary>
        /// <returns> xx </returns>
        /// <param name="path"> Filesystem "Asset"relative path. </param>  
        public static bool DeleteAsset(string path) {
            path = FixRelativePath(path);
            return SA_AssetDatabaseProxy.Delete(path);
        }


        /// <summary>
        /// Returns the first asset object of type type at given path assetPath.
        /// 
        /// Some asset files may contain multiple objects. (such as a Maya file which may contain multiple Meshes and GameObjects). 
        /// All paths are relative to the project Asset folder, for example: "MyTextures/hello.png".
        /// 
        /// The <see cref="assetPath"/> parameter is not case sensitive.
        /// ALL asset names and paths in Unity use forward slashes, even on Windows.
        /// This returns only an asset object that is visible in the Project view.If the asset is not found LoadAssetAtPath returns Null.
        /// </summary>
        /// <returns>The asset at path.</returns>
        /// <param name="assetPath">Path of the asset to load.</param>
        /// <typeparam name="T"> Data type of the asset.</typeparam>
        public static T LoadAssetAtPath<T>(string assetPath) where T : Object {
            assetPath = FixRelativePath(assetPath);
            return SA_AssetDatabaseProxy.LoadAssetAtPath<T>(assetPath);
        }


        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk
        /// </summary>
        /// <param name="path"> Filesystem "Asset" folder relative path.</param> 
        public static bool IsDirectoryExists(string path) {
            return SA_PathUtil.IsDirectoryExists(path);
        }








        private static string FixRelativePath(string path, bool validateFoldersPath = true) {

            path = SA_PathUtil.FixRelativePath(path);
            if(validateFoldersPath) {
                ValidateFoldersPath(path);
            }

            return path;
        }


        private static void ValidateFoldersPath(string path) {
            string parentDir = string.Empty;
            foreach(var dir in SA_PathUtil.GetDirectoriesOutOfPath(path)) {
                if (!IsDirectoryExists(dir)) {
                    string dirName = SA_PathUtil.GetPathDirectoryName(dir);
                    SA_AssetDatabaseProxy.CreateFolder(parentDir, dirName);
                }
                parentDir = dir;
            }
        }

      



        private class SA_AssetDatabaseProxy
        {
            public static void Create(Object asset, string path) {
                path = FixPath(path);
#if UNITY_EDITOR
                AssetDatabase.CreateAsset(asset, path);
#endif
            }


            public static bool Copy(string path, string newPath) {

                path = FixPath(path);
                newPath = FixPath(newPath);
#if UNITY_EDITOR
                return AssetDatabase.CopyAsset(path, newPath);
#else
            return false;
#endif
            }

            public static string Move(string oldPath, string newPath) {
                oldPath = FixPath(oldPath);
                newPath = FixPath(newPath);
#if UNITY_EDITOR
                return AssetDatabase.MoveAsset(oldPath, newPath);
#else
            return "";
#endif
            }

            public static bool Delete(string path) {
                path = FixPath(path);
#if UNITY_EDITOR
                return AssetDatabase.MoveAssetToTrash(path);
#else
                return false;
#endif
            }


            public static void CreateFolder(string parentFolder, string newFolderName) {
                parentFolder = FixPath(parentFolder);
#if UNITY_EDITOR
                AssetDatabase.CreateFolder(parentFolder, newFolderName);
#endif
            }

            public static string GetAssetPath(Object assetObject) {
#if UNITY_EDITOR
                return AssetDatabase.GetAssetPath(assetObject);
#else
                return string.Empty;
#endif

            }

            public static T LoadAssetAtPath<T>(string assetPath) where T : UnityEngine.Object {
#if UNITY_EDITOR
                assetPath = FixPath(assetPath);
                return AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
                return null;
#endif

            }


            private static string FixPath(string path) {
                path = string.Concat(SA_PathUtil.ASSETS, path);
                if (path.EndsWith(SA_PathUtil.FOLDER_SEPARATOR, System.StringComparison.CurrentCulture)) {
                    path = path.Substring(0, path.Length - 1);
                }

                return path;

            }
        }

    }
}