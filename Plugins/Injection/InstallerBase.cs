using UnityEngine;

namespace Injection
{
    public class InstallerBase : MonoBehaviour
    {
        public virtual void CreateAll()
        {

        }

        public virtual void BindAll(Injection.Injector injector)
        {
            
        }

        public virtual void InjectAll(Injection.Injector injector)
        {

        }

        public virtual void InitializeAll()
        {

        }
    }
}
