namespace Project.Utilities
{
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static readonly object padlock = new object();

        protected Singleton()
        {
        }

        public static T Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                    return _instance;
                }
            }
        }
    }
}