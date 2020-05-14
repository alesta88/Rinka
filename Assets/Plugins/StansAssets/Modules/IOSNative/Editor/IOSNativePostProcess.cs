#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;

using SA.IOSDeploy;

public class IOSNativePostProcess  {

	#if UNITY_IPHONE
	[PostProcessBuild(50)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {


		if(IOSNativeSettings.Instance.EnableForceTouchAPI && IOSNativeSettings.Instance.ForceTouchMenu.Count > 0) {

         
               

            
            ISD_PlistKey UIApplicationShortcutItems =  new ISD_PlistKey();
			UIApplicationShortcutItems.Name = "UIApplicationShortcutItems";
			UIApplicationShortcutItems.Type = ISD_PlistKeyType.Array;

			foreach(var item in IOSNativeSettings.Instance.ForceTouchMenu) {
				var ShortcutItem  = new ISD_PlistKey();
				ShortcutItem.Type = ISD_PlistKeyType.Dictionary;
				UIApplicationShortcutItems.AddChild (ShortcutItem);


				var ShortcutItemTitle =   new ISD_PlistKey();
				ShortcutItemTitle.Name = "UIApplicationShortcutItemTitle";
				ShortcutItemTitle.StringValue = item.Title;
				ShortcutItem.AddChild (ShortcutItemTitle);

				var ShortcutItemSubtitle =   new ISD_PlistKey();
				ShortcutItemSubtitle.Name = "UIApplicationShortcutItemSubtitle";
				ShortcutItemSubtitle.StringValue = item.Subtitle;
				ShortcutItem.AddChild (ShortcutItemSubtitle);


				var ShortcutItemType =   new ISD_PlistKey();
				ShortcutItemType.Name = "UIApplicationShortcutItemType";
				ShortcutItemType.StringValue = item.Action;
				ShortcutItem.AddChild (ShortcutItemType);

			}

           
			ISD_API.SetInfoPlistKey(UIApplicationShortcutItems);


		}


		if(IOSNativeSettings.Instance.EnablePermissionAPI) {

            ISD_API.AddFramework (ISD_iOSFramework.Photos);
			ISD_API.AddFramework (ISD_iOSFramework.Contacts);
			ISD_API.AddFramework (ISD_iOSFramework.EventKit);
		}


		if(IOSNativeSettings.Instance.EnablePushNotificationsAPI) {
			ISD_PlistKey UIBackgroundModes =  new ISD_PlistKey();
			UIBackgroundModes.Name = "UIBackgroundModes";
			UIBackgroundModes.Type = ISD_PlistKeyType.Array;

			ISD_PlistKey remoteNotification =  new ISD_PlistKey();
			remoteNotification.Name = "remote-notification";
			remoteNotification.StringValue = "remote-notification";
			remoteNotification.Type = ISD_PlistKeyType.String;

			UIBackgroundModes.AddChild (remoteNotification);
			ISD_API.SetInfoPlistKey(UIBackgroundModes);

		}




		if(IOSNativeSettings.Instance.EnableInAppsAPI) {
			ISD_API.AddFramework (ISD_iOSFramework.StoreKit);
		}

		if(IOSNativeSettings.Instance.EnableGameCenterAPI) {

			ISD_API.AddFramework (ISD_iOSFramework.GameKit);

			ISD_PlistKey UIRequiredDeviceCapabilities =  new ISD_PlistKey();
			UIRequiredDeviceCapabilities.Name = "UIRequiredDeviceCapabilities";
			UIRequiredDeviceCapabilities.Type = ISD_PlistKeyType.Array;

			ISD_PlistKey gamekit =  new ISD_PlistKey();
			gamekit.StringValue = "gamekit";
			gamekit.Type = ISD_PlistKeyType.String;
			UIRequiredDeviceCapabilities.AddChild(gamekit);


			ISD_PlistKey armv7 =  new ISD_PlistKey();
			armv7.StringValue = "armv7";
			armv7.Type = ISD_PlistKeyType.String;
			UIRequiredDeviceCapabilities.AddChild(armv7);


			ISD_API.SetInfoPlistKey(UIRequiredDeviceCapabilities);

		}

		if(IOSNativeSettings.Instance.UrlTypes.Count > 0) {
			ISD_PlistKey CFBundleURLTypes =  new ISD_PlistKey();
			CFBundleURLTypes.Name = "CFBundleURLTypes";
			CFBundleURLTypes.Type = ISD_PlistKeyType.Array;



			foreach(SA.IOSNative.Models.UrlType url in IOSNativeSettings.Instance.UrlTypes) {
				ISD_PlistKey URLTypeHolder =  new ISD_PlistKey();
				URLTypeHolder.Type = ISD_PlistKeyType.Dictionary;

				CFBundleURLTypes.AddChild (URLTypeHolder);


				ISD_PlistKey CFBundleURLName =  new ISD_PlistKey();
				CFBundleURLName.Type = ISD_PlistKeyType.String;
				CFBundleURLName.Name = "CFBundleURLName";
				CFBundleURLName.StringValue = url.Identifier;
				URLTypeHolder.AddChild (CFBundleURLName);


				ISD_PlistKey CFBundleURLSchemes =  new ISD_PlistKey();
				CFBundleURLSchemes.Type = ISD_PlistKeyType.Array;
				CFBundleURLSchemes.Name = "CFBundleURLSchemes";
				URLTypeHolder.AddChild (CFBundleURLSchemes);

				foreach(string scheme in url.Schemes) {
					ISD_PlistKey Scheme =  new ISD_PlistKey();
					Scheme.Type = ISD_PlistKeyType.String;
					Scheme.StringValue = scheme;

					CFBundleURLSchemes.AddChild (Scheme);
				}
			}

			foreach(ISD_PlistKey v in  SA.IOSDeploy.ISD_Settings.Instance.PlistVariables) {
				if(v.Name.Equals(CFBundleURLTypes.Name)) {
					SA.IOSDeploy.ISD_Settings.Instance.PlistVariables.Remove (v);
					break;
				}
			}
			SA.IOSDeploy.ISD_Settings.Instance.PlistVariables.Add (CFBundleURLTypes);
		}




		if(IOSNativeSettings.Instance.EnableSocialSharingAPI) {

			ISD_API.AddFramework (ISD_iOSFramework.Accounts);
			ISD_API.AddFramework (ISD_iOSFramework.Social);
			ISD_API.AddFramework (ISD_iOSFramework.MessageUI);
			


			string QueriesSchemesName = "LSApplicationQueriesSchemes";
            ISD_PlistKey LSApplicationQueriesSchemes = ISD_API.GetInfoPlistKey(QueriesSchemesName); 
			if(LSApplicationQueriesSchemes == null) {
				LSApplicationQueriesSchemes = new ISD_PlistKey();
				LSApplicationQueriesSchemes.Name = QueriesSchemesName;
				LSApplicationQueriesSchemes.Type = ISD_PlistKeyType.Array;
			}	

			ISD_PlistKey instagram =  new ISD_PlistKey();
			instagram.StringValue = "instagram";
			instagram.Type = ISD_PlistKeyType.String;
			LSApplicationQueriesSchemes.AddChild(instagram);

			ISD_PlistKey whatsapp =  new ISD_PlistKey();
			whatsapp.StringValue = "whatsapp";
			whatsapp.Type = ISD_PlistKeyType.String;
			LSApplicationQueriesSchemes.AddChild(whatsapp);


			ISD_API.SetInfoPlistKey (LSApplicationQueriesSchemes);

		}
			

		if(IOSNativeSettings.Instance.ApplicationQueriesSchemes.Count > 0) {
			string QueriesSchemesName = "LSApplicationQueriesSchemes";
			ISD_PlistKey LSApplicationQueriesSchemes = ISD_API.GetInfoPlistKey(QueriesSchemesName); 
			if(LSApplicationQueriesSchemes == null) {
				LSApplicationQueriesSchemes = new ISD_PlistKey();
				LSApplicationQueriesSchemes.Name = QueriesSchemesName;
				LSApplicationQueriesSchemes.Type = ISD_PlistKeyType.Array;
			}	


			foreach(var scheme in IOSNativeSettings.Instance.ApplicationQueriesSchemes) {
				ISD_PlistKey schemeName =  new ISD_PlistKey();
				schemeName.StringValue = scheme.Identifier;
				schemeName.Type = ISD_PlistKeyType.String;
				LSApplicationQueriesSchemes.AddChild(schemeName);
			}

			ISD_API.SetInfoPlistKey(LSApplicationQueriesSchemes);
		}




		if(IOSNativeSettings.Instance.EnableMediaPlayerAPI) {
			ISD_API.AddFramework (ISD_iOSFramework.MediaPlayer);
				

			var NSAppleMusicUsageDescription =  new ISD_PlistKey();
			NSAppleMusicUsageDescription.Name = "NSAppleMusicUsageDescription";
			NSAppleMusicUsageDescription.StringValue = IOSNativeSettings.Instance.AppleMusicUsageDescription;
			NSAppleMusicUsageDescription.Type = ISD_PlistKeyType.String;


			ISD_API.SetInfoPlistKey(NSAppleMusicUsageDescription);

		}
	

		if(IOSNativeSettings.Instance.EnableCameraAPI) {

			ISD_API.AddFramework (ISD_iOSFramework.MobileCoreServices);


			var NSCameraUsageDescription =  new ISD_PlistKey();
			NSCameraUsageDescription.Name = "NSCameraUsageDescription";
			NSCameraUsageDescription.StringValue = IOSNativeSettings.Instance.CameraUsageDescription;
			NSCameraUsageDescription.Type = ISD_PlistKeyType.String;


			ISD_API.SetInfoPlistKey(NSCameraUsageDescription);



			var NSPhotoLibraryUsageDescription =  new ISD_PlistKey();
			NSPhotoLibraryUsageDescription.Name = "NSPhotoLibraryUsageDescription";
			NSPhotoLibraryUsageDescription.StringValue = IOSNativeSettings.Instance.PhotoLibraryUsageDescription;
			NSPhotoLibraryUsageDescription.Type = ISD_PlistKeyType.String;


			ISD_API.SetInfoPlistKey(NSPhotoLibraryUsageDescription);


			var NSPhotoLibraryAddUsageDescription =  new ISD_PlistKey();
			NSPhotoLibraryAddUsageDescription.Name = "NSPhotoLibraryAddUsageDescription";
			NSPhotoLibraryAddUsageDescription.StringValue = IOSNativeSettings.Instance.PhotoLibraryUsageDescription;
			NSPhotoLibraryAddUsageDescription.Type = ISD_PlistKeyType.String;


			ISD_API.SetInfoPlistKey(NSPhotoLibraryAddUsageDescription);

		}

		if(IOSNativeSettings.Instance.EnableReplayKit) {
			ISD_API.AddFramework (ISD_iOSFramework.ReplayKit, weak: true);
		}


		if(IOSNativeSettings.Instance.EnableCloudKit) {
            ISD_API.AddFramework(ISD_iOSFramework.CloudKit, weak: true);
			string inheritedflag = "$(inherited)";
            ISD_API.AddFlag(inheritedflag, ISD_FlagType.LinkerFlag);
		}

		if(IOSNativeSettings.Instance.EnablePickerAPI) {
            ISD_API.AddFramework(ISD_iOSFramework.AssetsLibrary, weak: true);
		}


		if(IOSNativeSettings.Instance.EnableContactsAPI) {

            ISD_API.AddFramework(ISD_iOSFramework.Contacts, weak: true);
            ISD_API.AddFramework(ISD_iOSFramework.ContactsUI, weak: true);


			var NSContactsUsageDescription =  new ISD_PlistKey();
			NSContactsUsageDescription.Name = "NSContactsUsageDescription";
			NSContactsUsageDescription.StringValue = IOSNativeSettings.Instance.ContactsUsageDescription;
			NSContactsUsageDescription.Type = ISD_PlistKeyType.String;


			ISD_API.SetInfoPlistKey(NSContactsUsageDescription);

		}

		if(IOSNativeSettings.Instance.EnableSoomla) {

			ISD_API.AddFramework (ISD_iOSFramework.AdSupport);
            ISD_API.AddLibrary(ISD_iOSLibrary.libsqlite3);

			#if UNITY_5
				string soomlaLinkerFlag = "-force_load Libraries/Plugins/iOS/libSoomlaGrowLite.a";
			#else
				string soomlaLinkerFlag = "-force_load Libraries/libSoomlaGrowLite.a";
#endif

            ISD_API.AddFlag(soomlaLinkerFlag, ISD_FlagType.LinkerFlag);
		}

		if(IOSNativeSettings.Instance.EnableUserNotificationsAPI) {
			ISD_API.AddFramework (ISD_iOSFramework.UserNotifications);
		}

		Debug.Log("ISN Postprocess Done");

	
	}
	#endif
}
#endif