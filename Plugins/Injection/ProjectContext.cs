using System.Collections.Generic;
using Injection;
using UnityEngine;
using UnityEngine.Assertions;

namespace Slayground.Injection
{
    public class ProjectContext : MonoBehaviour
    {
        private static ProjectContext _instance;
        public static ProjectContext Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                
                InstantiateAndInitialize();
                Assert.IsNotNull(_instance);

                return _instance;
            }
        }

        public Injector Injector { get; } = new($"ProjectContext");

        public List<InstallerBase> installers = new();

        private void Awake()
        {
            foreach (InstallerBase installer in installers)
            {
                installer.CreateAll();
            }
            foreach (InstallerBase installer in installers)
            {
                installer.BindAll(Injector);
            }
            foreach (InstallerBase installer in installers)
            {
                installer.InjectAll(Injector);
            }
            foreach (InstallerBase installer in installers)
            {
                installer.InitializeAll();
            }
        }

        public static void InstantiateAndInitialize()
        {
            _instance = Instantiate(TryGetPrefab()).GetComponent<ProjectContext>();
            DontDestroyOnLoad(_instance);
        }

        private static GameObject TryGetPrefab()
        {
            Object[] prefabs = Resources.LoadAll("ProjectContext", typeof(GameObject));

            if (prefabs.Length > 0)
            {
                Assert.IsTrue(prefabs.Length == 1, "Found multiple project context prefabs at resource path ProjectContext");
                return (GameObject)prefabs[0];
            }

            throw new System.Exception("Unable to find ProjectContext prefab. " +
                                       "Create one and make sure it is inside a Resources folder");
        }
    }
}