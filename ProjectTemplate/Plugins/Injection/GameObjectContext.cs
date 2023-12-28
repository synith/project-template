using System.Collections.Generic;
using UnityEngine;


namespace Injection
{
    public class GameObjectContext : MonoBehaviour
    {
        [Inject] private Injector m_parentInjector;
        private Injector m_injector;
        public List<InstallerBase> Installers = new List<InstallerBase>();
        public Injector Injector
        {
            get => m_injector;
        }

        void Awake()
        {
            m_injector = new Injector($"GameObjectContext:{name}", m_parentInjector);
            foreach (var installer in Installers)
            {
                installer.CreateAll();
            }
            foreach (var installer in Installers)
            {
                installer.BindAll(m_injector);
            }
            foreach (var installer in Installers)
            {
                installer.InjectAll(m_injector);
            }
            foreach (var installer in Installers)
            {
                installer.InitializeAll();
            }

            Utils.InjectAllComponentsInGameObjectExcept(m_injector, gameObject, this);
        }
    }
}