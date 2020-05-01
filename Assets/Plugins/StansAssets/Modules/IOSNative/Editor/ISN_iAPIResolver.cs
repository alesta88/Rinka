using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.IOSNative.EditorTools
{
    
    public interface iAPIResolver
    {
        void OnAPIStateChnaged(bool enabled);
    }
}