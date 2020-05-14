////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Deploy
// @author Stanislav Osipov (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_IOS || UNITY_TVOS

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;



namespace SA.IOSDeploy
{

    public class ISD_PostProcess
    {
        

        [PostProcessBuild(100)]
        public static void OnPostprocessBuild(BuildTarget target, string projectPath) {



            var pbxProjPath = PBXProject.GetPBXProjectPath(projectPath); 

            

            PBXProject proj = new PBXProject();
            proj.ReadFromFile(pbxProjPath);
            string targetGuid = proj.GetUnityMainTargetGuid(); //alestaios proj.TargetGuidByName("Unity-iPhone");
            
           

            RegisterAppLanguages();

            AddFlags(proj, targetGuid);
            AddLibraries(proj, targetGuid);
            AddFrameworks(proj, targetGuid);
            AddEmbededFrameworks(proj, targetGuid);
            AddPlistVariables(proj, projectPath, targetGuid);
            ApplyBuildSettings(proj, targetGuid);
            CopyAssetFiles(proj, projectPath, targetGuid);
            AddShellScriptBuildPhase(proj, targetGuid);
                    
            proj.WriteToFile(pbxProjPath);


			var capManager = new ProjectCapabilityManager(pbxProjPath, ISD_Settings.ENTITLEMENTS_FILE_NAME, "Unity-iPhone");
			AddCapabilities(proj, targetGuid, capManager);
			capManager.WriteToFile();
            


            //Some API simply doens not work so on this block we are aplplying a workaround
            //after unuty deploy scrips has stoped working

            //Addons EmbededFrameworks
            foreach (var framework in ISD_Settings.Instance.EmbededFrameworks) {
                string contents = File.ReadAllText(pbxProjPath);
                string pattern = "(?<=Embed Frameworks)(?:.*)(\\/\\* " + framework.FileName + "\\ \\*\\/)(?=; };)";
                string oldText = "/* " + framework.FileName + " */";
                string updatedText = "/* " + framework.FileName + " */; settings = {ATTRIBUTES = (CodeSignOnCopy, ); }";
                contents = Regex.Replace(contents, pattern, m => m.Value.Replace(oldText, updatedText));
                File.WriteAllText(pbxProjPath, contents);
            }


            //Capatibilities
            foreach (var cap in ISD_Settings.Instance.Capabilities) {
                if(cap.CapabilityType == ISD_CapabilityType.Cloud) {
                    var entitlements = new PlistDocument();
                    var infoPlistPath = projectPath + "/" + ISD_Settings.ENTITLEMENTS_FILE_NAME;
                    entitlements.ReadFromFile(infoPlistPath);

                    var plistVariable = new PlistElementArray();
                    entitlements.root["com.apple.developer.icloud-container-identifiers"] = plistVariable;

                    entitlements.WriteToFile(infoPlistPath);
                }
            }
                     

        }

        static void AddPlistVariables(PBXProject proj, string pathToBuiltProject, string targetGUID) {
            var infoPlist = new PlistDocument();
            var infoPlistPath = pathToBuiltProject + "/Info.plist";
            infoPlist.ReadFromFile(infoPlistPath);

            foreach(var variable in ISD_Settings.Instance.PlistVariables) {

               PlistElement plistVariable = null;
               switch (variable.Type) {
                    case ISD_PlistKeyType.String:
                        plistVariable = new PlistElementString(variable.StringValue);
                        break;
                    case ISD_PlistKeyType.Integer:
                        plistVariable = new PlistElementInteger(variable.IntegerValue);
                        break;
                    case ISD_PlistKeyType.Boolean:
                        plistVariable = new PlistElementBoolean(variable.BooleanValue);
                        break;
                    case ISD_PlistKeyType.Array:
                        plistVariable = CreatePlistArray(variable);
                        break;
                    case ISD_PlistKeyType.Dictionary:
                        plistVariable = CreatePlistDict(variable);
                        break;


                }

                infoPlist.root[variable.Name] = plistVariable;
            }

            infoPlist.WriteToFile(infoPlistPath);



           

        }


        static PlistElementArray CreatePlistArray(ISD_PlistKey variable, PlistElementArray array = null) {

            if(array == null) {
                array = new PlistElementArray();
            }

            foreach (string variableUniqueIdKey in variable.ChildrensIds) {
                ISD_PlistKey v = ISD_Settings.Instance.getVariableById(variableUniqueIdKey);

                switch(v.Type) {
                    case ISD_PlistKeyType.String:
                        array.AddString(v.StringValue);
                        break;
                    case ISD_PlistKeyType.Boolean:
                        array.AddBoolean(v.BooleanValue);
                        break;
                    case ISD_PlistKeyType.Integer:
                        array.AddInteger(v.IntegerValue);
                        break;
                    case ISD_PlistKeyType.Array:
                        CreatePlistArray(v, array.AddArray());
                        break;
                    case ISD_PlistKeyType.Dictionary:
                        CreatePlistDict(v, array.AddDict());
                        break;
                }
            }

            return array;
        }

        static PlistElementDict CreatePlistDict(ISD_PlistKey variable, PlistElementDict dict = null) {

            if (dict == null) {
                dict = new PlistElementDict();
            }

            foreach (string variableUniqueIdKey in variable.ChildrensIds) {
                ISD_PlistKey v = ISD_Settings.Instance.getVariableById(variableUniqueIdKey);

                switch (v.Type) {
                    case ISD_PlistKeyType.String:
                        dict.SetString(v.Name, v.StringValue);
                        break;
                    case ISD_PlistKeyType.Boolean:
                        dict.SetBoolean(v.Name, v.BooleanValue);
                        break;
                    case ISD_PlistKeyType.Integer:
                        dict.SetInteger(v.Name, v.IntegerValue);
                        break;
                    case ISD_PlistKeyType.Array:
                        var array = dict.CreateArray(v.Name);
                        CreatePlistArray(v, array);
                        break;
                    case ISD_PlistKeyType.Dictionary:
                        var d = dict.CreateDict(v.Name);
                        CreatePlistDict(v, d);
                        break;
                }
            }

            return dict;
        }




        static void ApplyBuildSettings(PBXProject proj, string targetGUID) {
            foreach(var property in ISD_Settings.Instance.BuildProperties) {
                proj.SetBuildProperty(targetGUID, property.Name, property.Value);
            }
        }

        static void AddFlags(PBXProject proj, string targetGuid) {
         
            foreach(var flag in ISD_Settings.Instance.Flags) {
                if(flag.Type == ISD_FlagType.LinkerFlag) {
                    proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", flag.Name);
                }

                if (flag.Type == ISD_FlagType.LinkerFlag) {
                    proj.AddBuildProperty(targetGuid, "OTHER_CFLAGS", flag.Name);
                }
            }
            
           
           
        }


        static void RegisterAppLanguages() {
            var CFBundleLocalizations = new ISD_PlistKey();
            CFBundleLocalizations.Name = "CFBundleLocalizations";
            CFBundleLocalizations.Type = ISD_PlistKeyType.Array;

            foreach (var lang in ISD_Settings.Instance.Languages) {
                var langItem = new ISD_PlistKey();
                langItem.Type = ISD_PlistKeyType.String;
                langItem.StringValue = lang.Name;
                CFBundleLocalizations.AddChild(langItem);
            }
        }


        static void AddCapabilities(PBXProject proj, string targetGuid, ProjectCapabilityManager capManager) {


            if(ISD_Settings.Instance.Capabilities.Count == 0) {
                return;
            }

           

            foreach (var cap in ISD_Settings.Instance.Capabilities) {
                switch(cap.CapabilityType) {  
                 
                    case ISD_CapabilityType.Cloud:
                        var cloudSettings = ISD_Settings.Instance.iCloudCapabilitySettings;
                        capManager.AddiCloud(cloudSettings.KeyValueStorage, cloudSettings.iCloudDocument, new string[] { });
                        break;
                    case ISD_CapabilityType.InAppPurchase:
                        capManager.AddInAppPurchase();
                        break;
                    case ISD_CapabilityType.GameCenter:
                        capManager.AddGameCenter();
                        break;
                    case ISD_CapabilityType.PushNotifications:
                        var pushSettings = ISD_Settings.Instance.PushNotificationsCapabilitySettings;
                        capManager.AddPushNotifications(pushSettings.Development);
                        break;

                    default:
                        var capability = ISD_PBXCapabilityTypeUtility.ToPBXCapability(cap.CapabilityType);
                        string entitlementsFilePath = null;
                        if(!string.IsNullOrEmpty(cap.EntitlementsFilePath)) {
                            entitlementsFilePath = cap.EntitlementsFilePath;
                        } 


                        proj.AddCapability(targetGuid, capability, entitlementsFilePath, cap.AddOptionalFramework); 
                        break;
                }
            }


        }


        static void AddFrameworks(PBXProject proj, string targetGuid) {
            foreach (ISD_Framework framework in ISD_Settings.Instance.Frameworks) {
                proj.AddFrameworkToProject(targetGuid, framework.Name, framework.IsOptional);
            }
        }

        static void AddEmbededFrameworks(PBXProject proj, string targetGuid) {
            foreach (var framework in ISD_Settings.Instance.EmbededFrameworks) {


                string fileGuid = proj.AddFile(framework.AbsoluteFilePath, "Frameworks/" + framework.FileName, PBXSourceTree.Source);
                string embedPhase = proj.AddCopyFilesBuildPhase(targetGuid, "Embed Frameworks", "", "10");
                proj.AddFileToBuildSection(targetGuid, embedPhase, fileGuid);

                #if UNITY_2017_4_OR_NEWER
                PBXProjectExtensions.AddFileToEmbedFrameworks(proj, targetGuid, fileGuid);
                #endif


                proj.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
                //proj.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(SRCROOT)/PATH_TO_FRAMEWORK/");

            }
        }


        static void AddShellScriptBuildPhase(PBXProject proj, string targetGuid) {
            foreach (var script in ISD_Settings.Instance.ShellScripts) {
                MethodInfo dynMethod = proj.GetType().GetMethod("AppendShellScriptBuildPhase",
                                                 BindingFlags.NonPublic | BindingFlags.Instance, //if static AND public
                                                 null,
                                                 new[] { typeof(string), typeof(string), typeof(string), typeof(string) },//specify arguments to tell reflection which variant to look for
                                                 null);
                                  
             
                dynMethod.Invoke(proj, new object[] { targetGuid, script.Name, script.Shell, script.Script });
            }
        }


        static void AddLibraries(PBXProject proj, string targetGuid) {
            foreach (var lib in ISD_Settings.Instance.Libraries) {
                proj.AddFrameworkToProject(targetGuid, lib.Name, lib.IsOptional);
            }
        }



        static void CopyAssetFiles(PBXProject proj, string pathToBuiltProject, string targetGUID) {
            
            foreach (ISD_AssetFile file in ISD_Settings.Instance.Files) {

                if (file.IsDirectory) {

                    foreach (var assetPath in Directory.GetFiles(file.RelativeFilePath)) {

                        string fileName = Path.GetFileName(assetPath);
                        string XCodeRelativePath = file.XCodeRelativePath + "/" + fileName;
                        CopyFile(XCodeRelativePath, assetPath, pathToBuiltProject, proj, targetGUID);
                    }

                } else {
                    CopyFile(file.XCodeRelativePath, file.RelativeFilePath, pathToBuiltProject, proj, targetGUID);
                }

            }
        }


        static void CopyFile(string XCodeRelativePath, string sourcePath, string pathToBuiltProject, PBXProject proj, string targetGUID) {

            var dstPath = Path.Combine(pathToBuiltProject, XCodeRelativePath);
            var rootPath = Path.GetDirectoryName(dstPath);

            if (!Directory.Exists(rootPath)) {
                Directory.CreateDirectory(rootPath);
            }

            File.Copy(sourcePath, dstPath);

            string name = proj.AddFile(XCodeRelativePath, XCodeRelativePath, PBXSourceTree.Source);
            proj.AddFileToBuild(targetGUID, name);
        }


    }
}
#endif