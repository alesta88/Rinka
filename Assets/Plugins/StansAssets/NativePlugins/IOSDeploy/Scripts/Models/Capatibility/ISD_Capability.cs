using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.IOSDeploy
{
    [System.Serializable]
    public class ISD_Capability 
    {
        public ISD_CapabilityType CapabilityType;
        public string EntitlementsFilePath = string.Empty;
        public bool AddOptionalFramework = false;
    }
}