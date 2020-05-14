////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Deploy
// @author Stanislav Osipov (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

using SA.Foundation.Config;
using SA.Foundation.Patterns;
using SA.Foundation.Localization;



namespace SA.IOSDeploy
{

    public class ISD_Settings : SA_ScriptableSingleton<ISD_Settings>
    {

        public const string VERSION_NUMBER = "1.0.0";
        public const string ENTITLEMENTS_FILE_NAME = "ios_deploy.entitlements";
        public const string IOS_DEPLOY_FOLDER       = SA_Config.STANS_ASSETS_NATIVE_PLUGINS_PATH + "IOSDeploy/";
       



        //Editor Window
        public bool IsfwSettingOpen;
        public bool IsLibSettingOpen;
        public bool IslinkerSettingOpne;
        public bool IscompilerSettingsOpen;
        public bool IsPlistSettingsOpen;
        public bool IsLanguageSettingOpen = true;
        public bool IsDefFrameworksOpen = false;
        public bool IsDefLibrariesOpen = false;
        public bool IsBuildSettingsOpen;
        public int ToolbarIndex = 0;


        //Post Process Libs
        public List<ISD_Framework> Frameworks = new List<ISD_Framework>();
        public List<ISD_EmbedFramework> EmbededFrameworks = new List<ISD_EmbedFramework>();
        public List<ISD_Library> Libraries = new List<ISD_Library>();
        public List<ISD_Flag> Flags = new List<ISD_Flag>();
        public List<ISD_PlistKey> PlistVariables = new List<ISD_PlistKey>();
        public List<ISD_PlistKeyId> VariableDictionary = new List<ISD_PlistKeyId>();
        public List<SA_ISOLanguage> Languages = new List<SA_ISOLanguage>();

       
        public List<ISD_ShellScript> ShellScripts = new List<ISD_ShellScript>();
        [SerializeField] List<ISD_BuildProperty> m_buildProperties = new List<ISD_BuildProperty>();


        //--------------------------------------
        // Capabilities
        //--------------------------------------

        public List<ISD_Capability> Capabilities = new List<ISD_Capability>();
        public ISD_iCloudCapability iCloudCapabilitySettings = new ISD_iCloudCapability();
        public ISD_PushNotificationsCapability PushNotificationsCapabilitySettings = new ISD_PushNotificationsCapability();



        public List<ISD_AssetFile> Files = new List<ISD_AssetFile>();


        //--------------------------------------
        // Variables
        //--------------------------------------



        public void AddVariableToDictionary(string uniqueIdKey, ISD_PlistKey var) {
            ISD_PlistKeyId newVar = new ISD_PlistKeyId();
            newVar.uniqueIdKey = uniqueIdKey;
            newVar.VariableValue = var;
            VariableDictionary.Add(newVar);
        }

        public void RemoveVariable(ISD_PlistKey v, IList ListWithThisVariable) {
            if (ISD_Settings.Instance.PlistVariables.Contains(v)) {
                ISD_Settings.Instance.PlistVariables.Remove(v);
            } else {
                foreach (ISD_PlistKeyId vid in VariableDictionary) {
                    if (vid.VariableValue.Equals(v)) {
                        VariableDictionary.Remove(vid);
                        string id = vid.uniqueIdKey;
                        if (ListWithThisVariable.Contains(id))
                            ListWithThisVariable.Remove(vid.uniqueIdKey);
                        break;
                    }
                }
            }

            //remove junk

            List<ISD_PlistKeyId> keysInUse = new List<ISD_PlistKeyId>(VariableDictionary);

            foreach (var id in VariableDictionary) {
                bool isInUse = IsInUse(id.uniqueIdKey, PlistVariables);
                if (!isInUse) {
                    keysInUse.Remove(id);
                }
            }

            VariableDictionary = keysInUse;

        }


        private bool IsInUse(string id, List<ISD_PlistKey> list) {
            foreach (var key in list) {
                if (key.ChildrensIds.Contains(id)) {
                    return true;
                } else {
                    bool inUse = IsInUse(id, key.Children);
                    if (inUse) {
                        return true;
                    }
                }
            }

            return false;
        }

        public ISD_PlistKey getVariableById(string uniqueIdKey) {
            foreach (ISD_PlistKeyId vid in VariableDictionary) {
                if (vid.uniqueIdKey.Equals(uniqueIdKey)) {
                    return vid.VariableValue;
                }
            }
            return null;
        }




        //--------------------------------------
        // Build Properties
        //--------------------------------------

        public List<ISD_BuildProperty> BuildProperties {
            get {
                if (m_buildProperties.Count == 0) {
                    ISD_BuildProperty property;
                    property = new ISD_BuildProperty("ENABLE_BITCODE", "NO");
                    m_buildProperties.Add(property);

                    property = new ISD_BuildProperty("ENABLE_TESTABILITY", "NO");
                    m_buildProperties.Add(property);

                    property = new ISD_BuildProperty("GENERATE_PROFILING_CODE", "NO");
                    m_buildProperties.Add(property);
                }

                return m_buildProperties;
            }
        }



    }
}
