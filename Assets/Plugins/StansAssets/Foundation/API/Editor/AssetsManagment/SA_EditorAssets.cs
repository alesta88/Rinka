using System.Collections.Generic;
using SA.Foundation.Utility;
using UnityEditor;
using UnityEngine;

namespace SA.Foundation.Editor
{
    public static class SA_EditorAssets
    {

        private static Dictionary<string, Texture2D> s_icons = new Dictionary<string, Texture2D>();


        public static Font GetFontAtPath(string path) {
            string relativePath = "Assets/" + path;
            return AssetDatabase.LoadAssetAtPath(relativePath, typeof(Font)) as Font;
        }

        /// <summary>
        /// Load a Texture Asset at Asset/ folder relavtive path
        /// Also texture import options will be chnaged to a editor 
        /// </summary>
        /// <param name="path">Asset/ folder relavtive texture path</param> 
        public static Texture2D GetTextureAtPath(string path) {

            if (s_icons.ContainsKey(path)) {
                return s_icons[path];
            } else {

                string relativePath = "Assets/" + path;
                TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(relativePath);
                if (importer == null) {
                    return new Texture2D(0, 0); 
                }

                bool importRequired = false;

                if (importer.mipmapEnabled != false) { importer.mipmapEnabled = false; importRequired = true; } 
                if (importer.alphaIsTransparency != true) { importer.alphaIsTransparency = true; importRequired = true; }
                if (importer.wrapMode != TextureWrapMode.Clamp) { importer.wrapMode = TextureWrapMode.Clamp; importRequired = true; }
                if (importer.textureType != TextureImporterType.GUI) { importer.textureType = TextureImporterType.GUI; importRequired = true; }
                if (importer.npotScale != TextureImporterNPOTScale.None) { importer.npotScale = TextureImporterNPOTScale.None; importRequired = true; }
                if (importer.alphaSource != TextureImporterAlphaSource.FromInput) { importer.alphaSource = TextureImporterAlphaSource.FromInput; importRequired = true; }

                //Should we make additional option for this?
                if (importer.isReadable != true) { importer.isReadable = true; importRequired = true; }

                var settings = importer.GetPlatformTextureSettings(EditorUserBuildSettings.activeBuildTarget.ToString());
                if(settings.overridden) {
                    settings.overridden = false;
                    importer.SetPlatformTextureSettings(settings);
                }

                settings = importer.GetDefaultPlatformTextureSettings();
                if (!settings.textureCompression.Equals(TextureImporterCompression.Uncompressed)) {
                    settings.textureCompression = TextureImporterCompression.Uncompressed; 
                    importRequired = true;  
                }

                if (importRequired) {
                    importer.SetPlatformTextureSettings(settings);
                }

                Texture2D tex = AssetDatabase.LoadAssetAtPath(relativePath, typeof(Texture2D)) as Texture2D; 
                s_icons.Add(path, tex);

                return GetTextureAtPath(path);   
            }  
         }



        /// <summary>
        /// Enable's ot disable's script define line 
        /// </summary>
        /// <param name="file">path to a script file</param> 
        /// <param name="define">defined name</param> 
        /// <param name="isEnabled">new define state</param> 
        public static void ChangeScriptDefineState(string file, string define, bool isEnabled) {
            if (SA_FilesUtil.IsFileExists(file)) {
                string content = SA_FilesUtil.Read(file);


                int endlineIndex;
                endlineIndex = content.IndexOf(System.Environment.NewLine, System.StringComparison.CurrentCulture);
                if (endlineIndex == -1) {
                    endlineIndex = content.IndexOf("\n", System.StringComparison.CurrentCulture);
                }
                string TagLine = content.Substring(0, endlineIndex);

                if (isEnabled) {
                    content = content.Replace(TagLine, "#define " + define);
                } else {
                    content = content.Replace(TagLine, "//#define " + define);
                }
                SA_FilesUtil.Write(file, content);
            }
        }


      
    }
}