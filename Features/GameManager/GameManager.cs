using System.Threading.Tasks;
using Injection;
using Slayground.Features.GameState;
using Slayground.Features.SceneLoader;
using UnityEngine;

namespace Slayground.Features.GameManager
{
    public class GameManager : IGameManager
    {
        [Inject] private IGameStateManager _gameStateManager;
        [Inject] private ISceneLoader _sceneLoader;

        public async void LoadMainMenu()
        {
            await LoadNextScene(new MainMenuState(), SceneNames.Game, SceneNames.MainMenu);
        }

        public async void StartGame()
        {
            await LoadNextScene(new PlayingState(), SceneNames.MainMenu, SceneNames.Game);
        }

        private async Task LoadNextScene(BaseState endState, string currentScene, string nextScene)
        {
            if (_gameStateManager.CurrentState != null)
            {
                if (_gameStateManager.CurrentState.GetType() == endState.GetType()) return;
            }

            await ChangeState(new LoadingState(_sceneLoader, currentScene, nextScene));
            await ChangeState(endState);
        }

        public async void QuitGame()
        {
            await ChangeState(new QuittingState(_sceneLoader));
        }

        public async void PauseGame()
        {
            await ChangeState(new PausedState());
        }

        public async void ResumeGame()
        {
            await ChangeState(new PlayingState());
        }

        private async Task ChangeState(BaseState state) => await _gameStateManager.ChangeState(state);
    }

    public interface IGameManager
    {
        void LoadMainMenu();
        void StartGame();
        void QuitGame();
        void ResumeGame();
        void PauseGame();
    }
}