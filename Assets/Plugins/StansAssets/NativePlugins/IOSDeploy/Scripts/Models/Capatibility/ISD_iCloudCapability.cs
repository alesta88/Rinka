using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.IOSDeploy
{
    [System.Serializable]
    public class ISD_iCloudCapability 
    {
        public bool KeyValueStorage = false;
        public bool iCloudDocument = false;
        public List<string> CustomContainers;
      
    }
}
