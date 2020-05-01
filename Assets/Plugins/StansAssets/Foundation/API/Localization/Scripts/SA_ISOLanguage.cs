using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SA.Foundation.Localization
{
    [System.Serializable]
    public class SA_ISOLanguage
    {
        [SerializeField] string m_code;
        [SerializeField] string m_name;
        [SerializeField] string m_nativeName;


        /// <summary>
        /// ISO Language code
        /// </summary>
        public string Code {
            get {
                return m_code;
            }
        }


        /// <summary>
        /// Full Languages name. Example: Russian
        /// </summary>
        public string Name {
            get {
                return m_name;
            }
        }


        /// <summary>
        /// Full Languages name. Example: Русский
        /// </summary>
        public string NativeName {
            get {
                return m_nativeName;
            }
        }
    }
}