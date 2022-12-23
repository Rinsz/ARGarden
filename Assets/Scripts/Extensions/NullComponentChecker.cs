using UnityEngine;

namespace Extensions
{
    public static class NullComponentChecker
    {
        public static void LogIfComponentNull(Component component, string message)
        {
            if (!component)
            {
                Debug.LogError(message);
            }
        }
    }
}