using System;

using SA.Foundation.Patterns;

namespace SA.Foundation.Events
{

    public class SA_MonoEvents : SA_Singleton<SA_MonoEvents>
    {

        public event Action ApplicationQuit = delegate { };
        public event Action<bool> ApplicationFocus = delegate { };
        public event Action<bool> ApplicationPause = delegate { };

        public event Action OnUpdate = delegate { };

        protected override void OnApplicationQuit() {
            base.OnApplicationQuit();
            ApplicationQuit();
        }

        private void OnApplicationFocus(bool focus) {
            ApplicationFocus(focus);
        }

        private void OnApplicationPause(bool pause) {
            ApplicationPause(pause);
        }


        private void Update() {
            OnUpdate.Invoke();
        }
    }
}