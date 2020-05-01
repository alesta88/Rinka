using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Foundation.Events
{

    public interface SA_iSafeEvent<T>
    {
        /// <summary>
        /// Add null pointer safe listener to the SA_Event.
        /// If your target object will be == null or Equals(null) event will not be fired
        /// Use it if you do not want to unsubscribe on destory or using anonymus methos
        /// </summary>
        /// <param name="callbackTarget"> Callback function. </param> 
        /// <param name="listner"> Callback function. </param> 
        void AddSafeListener(object callbackTarget, Action<T> listner);

        /// <summary>
        /// Remove listener from the SA_Event.
        /// </summary>
        /// <param name="listner"> Callback function. </param> 
        void RemoveListener(Action<T> listner);

    }

    public interface SA_iSafeEvent
    {
        /// <summary>
        /// Add null pointer safe listener to the SA_Event.
        /// If your target object will be == null or Equals(null) event will not be fired
        /// Use it if you do not want to unsubscribe on destory or using anonymus methos
        /// </summary>
        /// <param name="callbackTarget"> Callback function. </param> 
        /// <param name="listner"> Callback function. </param> 
        void AddSafeListener(object callbackTarget, Action listner);

        /// <summary>
        /// Remove listener from the SA_Event.
        /// </summary>
        /// <param name="listner"> Callback function. </param> 
        void RemoveListener(Action listner);

    }
}