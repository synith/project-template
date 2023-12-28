using System.Collections.Generic;
using Slayground.Injection;
using UnityEngine;

namespace Injection
{
    public class SceneContext : MonoBehaviour
    {
        [SerializeField] private Injector injector;
        public Injector Injector => injector;

        public List<InstallerBase> installers = new();

        private void Awake()
        {
            injector = new Injector($"SceneContext:{name}", ProjectContext.Instance.Injector);

            foreach (InstallerBase installer in installers)
            {
                installer.CreateAll();
            }
            foreach (InstallerBase installer in installers)
            {
                installer.BindAll(injector);
            }
            foreach (InstallerBase installer in installers)
            {
                installer.InjectAll(injector);
            }
            foreach (InstallerBase installer in installers)
            {
                installer.InitializeAll();
            }

            Utils.InjectAllSceneComponents(gameObject.scene, injector);
        }
    }
}