////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Deploy
// @author Stanislav Osipov (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using SA.Foundation.Config;


namespace SA.IOSDeploy {

	public class ISD_EditorMenu : EditorWindow {
				
        [MenuItem(SA_Config.EDITOR_MENU_ROOT + "IOS Deploy/Settings" , false, 300)]
		public static void Edit() {
			Selection.activeObject = ISD_Settings.Instance;
		}

		
        [MenuItem(SA_Config.EDITOR_MENU_ROOT + "IOS Deploy/Documentation" , false, 400)]
		public static void ISDSetupPluginSetUp() {
			string url = "https://unionassets.com/ios-deploy/manual";
			Application.OpenURL(url);
		}

	}

}
