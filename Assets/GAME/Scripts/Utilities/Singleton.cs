namespace Project.Utilities
{
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static T _instance;
        private static readonly object padlock = new object();

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