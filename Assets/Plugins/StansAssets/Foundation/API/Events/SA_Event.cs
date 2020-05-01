using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Foundation.Events
{

    public class SA_Event<T> : SA_iEvent<T>
    {

        private class SafeActionInfo
        {
            public Action<T> Action;
            public object Target;
        }

        private List<SafeActionInfo> m_targetedActions = new List<SafeActionInfo>();

        /// <summary>
        /// Add listener to the SA_Event.
        /// </summary>
        /// <param name="listner"> Callback function. </param> 
        public void AddListener(Action<T> listner) {

            if (listner == null) { return; }

            var info = new SafeActionInfo();
            info.Target = this;
            info.Action = listner;

            m_targetedActions.Add(info);
        }

        /// <summary>
        /// Add null pointer safe listener to the SA_Event.
        /// If your target object will be == null or Equals(null) event will not be fired
        /// Use it if you do not want to unsubscribe on destory or using anonymus methos
        /// </summary>
        /// <param name="callbackTarget"> Callback function. </param> 
        /// <param name="listner"> Callback function. </param> 
        public void AddSafeListener(object callbackTarget, Action<T> listner) {

            if (listner == null) { return; }

            var info = new SafeActionInfo();
            info.Target = callbackTarget;
            info.Action = listner;

            m_targetedActions.Add(info);
        }

        /// <summary>
        /// Invoke all registered callbacks
        /// </summary>
        public void Invoke(T obj) {
            var invocationList = new List<SafeActionInfo>(m_targetedActions);

            foreach(var info in invocationList) {
                if (info.Target != null && !info.Target.Equals(null)) {
                    info.Action.Invoke(obj);
                }
            }
        }


        /// <summary>
        /// Remove listener from the SA_Event.
        /// </summary>
        /// <param name="listner"> Callback function. </param> 
        public void RemoveListener(Action<T> listner) {
            foreach (var info in m_targetedActions) {
                if(listner.Equals(info.Action)) {
                    m_targetedActions.Remove(info);
                    return;
                }
            }
        }

        
        /// <summary>
        /// Remove all registred listener from the SA_Event.
        /// </summary>
        public void RemoveAllListeners() {
            m_targetedActions.Clear();
        }
    }







    




    public class SA_Event : SA_iEvent
    {

        private class SafeActionInfo
        {
            public Action Action;
            public object Target;
        }

        private List<SafeActionInfo> m_targetedActions = new List<SafeActionInfo>();

        /// <summary>
        /// Add listener to the SA_Event.
        /// </summary>
        /// <param name="listner"> Callback function. </param> 
        public void AddListener(Action listner) {

            if (listner == null) { return; }

            var info = new SafeActionInfo();
            info.Target = this;
            info.Action = listner;

            m_targetedActions.Add(info);
        }

        /// <summary>
        /// Add null pointer safe listener to the SA_Event.
        /// If your target object will be == null or Equals(null) event will not be fired
        /// Use it if you do not want to unsubscribe on destory or using anonymus methos
        /// </summary>
        /// <param name="callbackTarget"> Callback function. </param> 
        /// <param name="listner"> Callback function. </param> 
        public void AddSafeListener(object callbackTarget, Action listner) {

            if (listner == null) { return; }

            var info = new SafeActionInfo();
            info.Target = callbackTarget;
            info.Action = listner;

            m_targetedActions.Add(info);
        }

        /// <summary>
        /// Invoke all registered callbacks
        /// </summary>
        public void Invoke() {
            var invocationList = new List<SafeActionInfo>(m_targetedActions);

            foreach (var info in invocationList) {
                if (info.Target != null && !info.Target.Equals(null)) {
                    info.Action.Invoke();
                }
            }
        }


        /// <summary>
        /// Remove listener from the SA_Event.
        /// </summary>
        /// <param name="listner"> Callback function. </param> 
        public void RemoveListener(Action listner) {
            foreach (var info in m_targetedActions) {
                if (listner.Equals(info.Action)) {
                    m_targetedActions.Remove(info);
                    return;
                }
            }
        }


        /// <summary>
        /// Remove all registred listener from the SA_Event.
        /// </summary>
        public void RemoveAllListeners() {
            m_targetedActions.Clear();
        }
    }




}