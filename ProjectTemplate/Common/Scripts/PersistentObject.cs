using UnityEngine;

namespace Slayground.Common
{
    public class PersistentObject : MonoBehaviour
    {
        private void Awake() => DontDestroyOnLoad(gameObject);
    }
}
