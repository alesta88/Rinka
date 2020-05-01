using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_IOS || UNITY_TVOS
using UnityEditor.iOS.Xcode;
#endif

namespace SA.IOSDeploy
{

    public class ISD_PBXCapabilityTypeUtility
    {

        #if UNITY_IOS || UNITY_TVOS


        public static PBXCapabilityType ToPBXCapability(ISD_CapabilityType capability) {
            switch (capability) {

                case ISD_CapabilityType.Cloud:
                    return PBXCapabilityType.iCloud;
                case ISD_CapabilityType.PushNotifications:
                    return PBXCapabilityType.PushNotifications;
                case ISD_CapabilityType.GameCenter:
                    return PBXCapabilityType.GameCenter;
                case ISD_CapabilityType.Wallet:
                    return PBXCapabilityType.Wallet;
                case ISD_CapabilityType.Siri:
                    return PBXCapabilityType.Siri;
                case ISD_CapabilityType.ApplePay:
                    return PBXCapabilityType.ApplePay;
                case ISD_CapabilityType.InAppPurchase:
                    return PBXCapabilityType.InAppPurchase;
                case ISD_CapabilityType.Maps:
                    return PBXCapabilityType.Maps;
                case ISD_CapabilityType.PersonalVPN:
                    return PBXCapabilityType.PersonalVPN;

                case ISD_CapabilityType.BackgroundModes:
                    return PBXCapabilityType.BackgroundModes;
                case ISD_CapabilityType.KeychainSharing:
                    return PBXCapabilityType.KeychainSharing;
                case ISD_CapabilityType.InterAppAudio:
                    return PBXCapabilityType.InterAppAudio;

                case ISD_CapabilityType.AssociatedDomains:
                    return PBXCapabilityType.AssociatedDomains;
                case ISD_CapabilityType.AppGroups:
                    return PBXCapabilityType.AppGroups;
                case ISD_CapabilityType.DataProtection:
                    return PBXCapabilityType.DataProtection;
                case ISD_CapabilityType.HomeKit:
                    return PBXCapabilityType.HomeKit;
                case ISD_CapabilityType.HealthKit:
                    return PBXCapabilityType.HealthKit;


                case ISD_CapabilityType.WirelessAccessoryConfiguration:
                    return PBXCapabilityType.WirelessAccessoryConfiguration;
                default:
                    return PBXCapabilityType.iCloud;
            }
        }

        #endif

    }
}