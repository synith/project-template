using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Slayground.Features.SceneLoader
{
    public interface ISceneLoader
    {
        Task Load(string sceneName);
        Task Unload(string sceneName);
        void UnloadAllScenes();
    }

    public static class SceneNames
    {
        public const string MainMenu = "MainMenu_Scene";
        public const string Game = "Game_Scene";
        public const string Bootstrap = "Bootstrap_Scene";
    }

    public class SceneLoader : ISceneLoader
    {
        public async Task Load(string sceneName) => await LoadSceneAsync(sceneName);
        public async Task Unload(string sceneName) => await UnloadSceneAsync(sceneName);
        public void UnloadAllScenes() => UnloadAllScenesAsync();

        private static async Task LoadSceneAsync(string sceneName)
        {
            if (IsSceneLoaded(sceneName)) return;
            
            Debug.Log($"LOAD: {sceneName}.");
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        private static async Task UnloadSceneAsync(string sceneName)
        {
            if (!IsSceneLoaded(sceneName)) return;
            
            Debug.Log($"UNLOAD: {sceneName}.");
            await SceneManager.UnloadSceneAsync(sceneName);
        }

        private static async void UnloadAllScenesAsync()
        {
            List<AsyncOperation> asyncOperations = new();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == SceneNames.Bootstrap) continue;
                asyncOperations.Add(SceneManager.UnloadSceneAsync(scene));
            }

            foreach (AsyncOperation asyncOperation in asyncOperations)
            {
                if (asyncOperation == null) continue;
                await asyncOperation;
            }
        }

        private static bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}