using System.Threading.Tasks;
using Slayground.Common;
using Slayground.Features.SceneLoader;
using UnityEngine;

namespace Slayground.Features.GameState
{
    public class QuittingState : BaseState
    {
        private readonly ISceneLoader _sceneLoader;
        
        public QuittingState(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        public override async Task EnterState()
        {
            _sceneLoader.UnloadAllScenes();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            await Task.CompletedTask;
        }

        public override async Task ExitState()
        {
            await Task.CompletedTask;
        }
    }
}