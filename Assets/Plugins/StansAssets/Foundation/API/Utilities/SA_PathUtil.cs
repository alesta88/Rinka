using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace SA.Foundation.Utility
{

    public static class SA_PathUtil 
    {
        public const string ASSETS = "Assets/";
        public const string FOLDER_SEPARATOR = "/";



        /// <summary>
        /// Use this method to make sure given path is a correct 
        /// "Asset" folder relative path.
        /// Methods will check path end and begin, and will try to fix is 
        /// if any issue is found
        /// </summary>
        /// <param name="path"> Filesystem "Asset" folder relative path.</param> 
        public static string FixRelativePath(string path) {

            if (path.StartsWith(FOLDER_SEPARATOR, System.StringComparison.CurrentCulture)) {
                path = path.Remove(0, 1);
            }

            if (path.StartsWith(ASSETS, System.StringComparison.CurrentCulture)) {
                path = path.Remove(0, ASSETS.Length);
            }

            return path;
        }


        /// <summary>
        /// Convert's an application file path to absolute system path.
        /// For Editor application relative path is the root of an Asset folder.
        /// </summary>
        /// <param name="relativePath"> Filesystem "Asset" folder relative path.</param> 
        public static string ConvertRelativeToAbsolutePath(string relativePath) {

            relativePath = FixRelativePath(relativePath);

            string dataPath;
            if(Application.isEditor) {
                dataPath = Application.dataPath;
            } else {
                dataPath = Application.persistentDataPath;
            }

            return dataPath + FOLDER_SEPARATOR + relativePath;
        }


        /// <summary>
        /// Convert's an Asset folder relative path to Project folder relative patj
        /// For Editor application relative path is the root of an Asset folder.
        /// </summary>
        /// <param name="relativePath"> Filesystem "Asset" folder relative path.</param> 
        public static string ConvertAssetRelativeToProjectRelative(string relativePath) {
            relativePath = FixRelativePath(relativePath);
            return ASSETS + relativePath;
        }



        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk
        /// </summary>
        /// <param name="path"> Filesystem "Asset" folder relative path.</param> 
        public static bool IsDirectoryExists(string path) {
            return Directory.Exists(ConvertRelativeToAbsolutePath(path));
        }


        public static List<string> GetDirectoriesOutOfPath(string path) {

            List<string> directories = new List<string>();
            string parentFolder = string.Empty;
            int separatorIndex = path.IndexOf(FOLDER_SEPARATOR, System.StringComparison.CurrentCulture);
            while (separatorIndex != -1) {

                int offset = separatorIndex + 1;
                string testedFolder = string.Concat(parentFolder, path.Substring(0, offset));
                directories.Add(testedFolder);

                path = path.Substring(offset, path.Length - offset);
                separatorIndex = path.IndexOf(FOLDER_SEPARATOR, System.StringComparison.CurrentCulture);
                parentFolder = testedFolder;
            }

            return directories;
        }


        /// <summary>
        /// Methods return's name of the last directory in a path
        /// for example: from /x/y/z/ -> z will be result 
        /// </summary>
        public static string GetPathDirectoryName(string folderPath) {

            if (folderPath.EndsWith(SA_PathUtil.FOLDER_SEPARATOR, System.StringComparison.CurrentCulture)) {
                folderPath = folderPath.Substring(0, folderPath.Length - 1);
            }

            int separatorIndex = folderPath.LastIndexOf(SA_PathUtil.FOLDER_SEPARATOR, System.StringComparison.CurrentCulture);

            if (separatorIndex == -1) {
                return folderPath;
            } else {
                int offset = separatorIndex + 1;
                return folderPath.Substring(offset, folderPath.Length - offset);
            }
        }


    }
}