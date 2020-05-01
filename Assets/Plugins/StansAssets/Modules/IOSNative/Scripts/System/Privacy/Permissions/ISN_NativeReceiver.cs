using System;

namespace SA.IOSNative.Privacy
{
	public class NativeReceiver : SA.Common.Pattern.Singleton<NativeReceiver>
	{

		//--------------------------------------
		// Initialization
		//--------------------------------------


		public void Init() {

		}



		//--------------------------------------
		// Native Events
		//--------------------------------------
		void PermissionRequestResponseReceived(string permissionData) {
			PermissionsManager.PermissionRequestResponse (permissionData);
		}
	}
}

