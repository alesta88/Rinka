using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.IOSNative.EditorTools
{

    public class SocialAPIResolver : iAPIResolver
    {

        private const string API_NAME = "SOCIAL_API";
        private List<string> m_socialFiles = null;


        public SocialAPIResolver() {

            m_socialFiles = new List<string>();

            m_socialFiles.Add(IOSNativeSettingsEditor.ISN_SCRIPTS + "Social/Controllers/ISN_Facebook.cs");
            m_socialFiles.Add(IOSNativeSettingsEditor.ISN_SCRIPTS + "Social/Controllers/ISN_Twitter.cs");
            m_socialFiles.Add(IOSNativeSettingsEditor.ISN_SCRIPTS + "Social/Controllers/ISN_Instagram.cs");
            m_socialFiles.Add(IOSNativeSettingsEditor.ISN_SCRIPTS + "Social/Controllers/ISN_Mail.cs");
            m_socialFiles.Add(IOSNativeSettingsEditor.ISN_SCRIPTS + "Social/Controllers/ISN_Meida.cs");
            m_socialFiles.Add(IOSNativeSettingsEditor.ISN_SCRIPTS + "Social/Controllers/ISN_TextMessage.cs");
            m_socialFiles.Add(IOSNativeSettingsEditor.ISN_SCRIPTS + "Social/Controllers/ISN_Whatsapp.cs");

        }


        public void OnAPIStateChnaged(bool enabled) {

            foreach (string file in m_socialFiles) {
                SA.Common.Editor.Tools.ChnageDefineState(file, API_NAME, enabled);
            }


            if (!enabled) {
                Common.Editor.Instalation.RemoveIOSFile("ISN_SocialGate");
            } else {
                Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_SocialGate.mm.txt", SA.Common.Config.IOS_DESTANATION_PATH + "ISN_SocialGate.mm");
            }

        }


    }
}