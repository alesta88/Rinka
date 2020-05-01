////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Foundation.Utility {

	public static class SA_FilesUtil {


        /// <summary>
        /// Determines whether the specific file exists
        /// </summary>
        /// <param name="path"> Filesystem "Asset" folder relative path.</param>
        public static bool IsFileExists(string path) {
            if (string.IsNullOrEmpty(path)) {
				return false;
			}

            path = FixRelativePath(path, createFolders: false);
            return File.Exists (SA_PathUtil.ConvertRelativeToAbsolutePath(path));
		}



        /// <summary>
        /// Creates or overrides file in a specified path
        /// </summary>
        /// <param name="path"> Filesystem "Asset" folder relative path.</param>
        public static void CreateFile(string path) {

            path = FixRelativePath(path);
            File.Create(SA_PathUtil.ConvertRelativeToAbsolutePath(path));

		}


        /// <summary>
        /// Delete the specified file
        /// </summary>
        /// <param name="path"> Filesystem "Asset" folder relative path.</param>
        public static void DeleteFile(string path) {
            if (IsFileExists(path)) {
                path = SA_PathUtil.ConvertRelativeToAbsolutePath(path); 
                File.Delete(path);
            }
        }

        /*
        public static List<string> GetFilesFromDirectory(string path) {
            var result = new List<string>();

            return result;
        }
        */
        public static List<string> GetFilesFromDirectory(string path, params string[] extentions) {

            path = SA_PathUtil.ConvertRelativeToAbsolutePath(path);
            string[] files = Directory.GetFiles(path);
            List<string> result = new List<string>();

            if(extentions.Length != 0) {
                foreach (string file in files) {
                    foreach (var extention in extentions) {
                        if (extention.Equals(Path.GetExtension(file))) {
                            result.Add(Path.GetFileName(file));
                            break;
                        }
                    }
                }
            } else {
                result = new List<string>(files);
                 
            }


            return result;
        }

        /// <summary>
        /// Writes a string to the specific file
        /// </summary>
        /// <param name="path"> Filesystem "Asset" folder relative file path.</param>
        /// <param name="contents"> the string content to write </param>
        public static void Write(string path, string contents) {

            path = FixRelativePath(path);
            string absolutePath = SA_PathUtil.ConvertRelativeToAbsolutePath(path);
			
            TextWriter tw = new StreamWriter(absolutePath, false);
			tw.Write(contents);
			tw.Close(); 
		}


        /// <summary>
        /// Open's a text file, reads all the lines of the file, and the closes the file
        /// </summary>
        /// <param name="path"> Filesystem "Asset" folder relative file path.</param>
        public static string Read(string path) {
			#if !UNITY_WEBPLAYER
			if(IsFileExists(path)) {
                path = SA_PathUtil.ConvertRelativeToAbsolutePath(path); 
                return File.ReadAllText(path);
			} else {
                return string.Empty;
			}
			#endif

		}
		


        /// <summary>
        /// Copies and exsiting file to a new file. If file already exists upder the specified 
        /// destination, file will be overrided.
        /// </summary>
        /// <param name="srcPath"> Filesystem "Asset" folder relative source file path.</param>
        /// <param name="destPath"> Filesystem "Asset" folder relative destination file path.</param>
        public static void CopyFile(string srcPath, string destPath) {
            
			if (IsFileExists (srcPath) && !srcPath.Equals(destPath)) {


                srcPath = FixRelativePath(srcPath, createFolders: false);
                destPath = FixRelativePath(destPath);
				
                string absoluteSrcPath = SA_PathUtil.ConvertRelativeToAbsolutePath(srcPath);
                string absoluteDestPath = SA_PathUtil.ConvertRelativeToAbsolutePath(destPath);


                File.Copy(absoluteSrcPath, absoluteDestPath, true);

			}
		}
		

      
        /// <summary>
        /// Copies and exsiting directory to a new location.
        /// </summary>
        /// <param name="srcPath"> Filesystem "Asset" folder relative source file path.</param>
        /// <param name="destPath"> Filesystem "Asset" folder relative destination file path.</param>
        public static void CopyDirectory(string srcPath, string destPath) {

            #if !UNITY_WEBPLAYER

            srcPath = FixRelativePath(srcPath);
            destPath = FixRelativePath(destPath);


            srcPath   = SA_PathUtil.ConvertRelativeToAbsolutePath(srcPath); 
            destPath  = SA_PathUtil.ConvertRelativeToAbsolutePath (destPath);
			
			//Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(srcPath, "*", SearchOption.AllDirectories)) {
                Directory.CreateDirectory(dirPath.Replace(srcPath, destPath));
			}
			

			//Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories)) {
                File.Copy(newPath, newPath.Replace(srcPath, destPath), true);
			}
			
			#endif

		}
		
		/// <summary>
		/// Creates or overrides directory in a specified path
		/// </summary>
		/// <param name="path"> Filesystem "Asset" folder relative path.</param>
		public static void CreateDirectory(string path) {
			path = FixRelativePath(path);
			Directory.CreateDirectory(SA_PathUtil.ConvertRelativeToAbsolutePath(path));
		}

        /// <summary>
        /// Delete the specified directory and subdirectories and files in the directory
        /// </summary>
        /// <param name="folderPath"> Filesystem "Asset" directory relative file path.</param>
		public static void DeleteDirectory(string folderPath) {
            #if !UNITY_WEBPLAYER
            folderPath = SA_PathUtil.ConvertRelativeToAbsolutePath(folderPath); 
            Directory.Delete(folderPath, true);
			#endif

		}
		

        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk
        /// </summary>
        /// <param name="path"> Filesystem "Asset" folder relative path.</param> 
        public static bool IsDirectoryExists(string path) {
            return SA_PathUtil.IsDirectoryExists(path);
        }



        //--------------------------------------
        // Private Section
        //--------------------------------------


        private static string FixRelativePath(string path, bool createFolders = true) {
            path = SA_PathUtil.FixRelativePath(path);
            if (createFolders) {
                CreatePathFolders(path);
            }

            return path;
        }

        private static void CreatePathFolders(string path) {
            foreach (var dir in SA_PathUtil.GetDirectoriesOutOfPath(path)) {
                if (!IsDirectoryExists(dir)) {
                    string dirAbsolutePath = SA_PathUtil.ConvertRelativeToAbsolutePath(dir);
                    Directory.CreateDirectory(dirAbsolutePath);
                }
            }
        }
	}
}


