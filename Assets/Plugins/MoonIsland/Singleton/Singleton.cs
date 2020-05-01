namespace MoonIsland {
    public class Singleton<T> where T : new() {
        static public bool IsInstantiated => m_instance != null;

        static T m_instance;
        static public T Instance {
            get {
                if(m_instance == null) {
                    CreateInstance();
                }
                return m_instance;
            }
        }

        public void Create() { }

        static void CreateInstance() {
            if(m_instance == null) {
                m_instance = new T();
            }
        }
    }
}

