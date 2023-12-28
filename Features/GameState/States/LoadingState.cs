using System.Threading.Tasks;
using Slayground.Features.SceneLoader;

namespace Slayground.Features.GameState
{
    public class LoadingState : BaseState
    {
        public string CurrentScene { get; }
        public string NextScene { get; }
        
        private readonly ISceneLoader _sceneLoader;

        public LoadingState(ISceneLoader sceneLoader, string currentScene, string nextScene)
        {
            _sceneLoader = sceneLoader;
            CurrentScene = currentScene;
            NextScene = nextScene;
        }

        public override async Task EnterState()
        {
            await _sceneLoader.Unload(CurrentScene);
            await _sceneLoader.Load(NextScene);
        }

        public override async Task ExitState()
        {
            await Task.CompletedTask;
        }
    }
}