using System;
using Injection;
using Slayground.Features.GameManager;
using Slayground.Features.GameState;
using UnityEngine;

namespace Slayground.Common
{
    public class Test : MonoBehaviour
    {
        [Inject] private IGameStateManager _gameStateManager;
        [Inject] private IGameManager _gameManager;
        
        private BaseState CurrentState => _gameStateManager.CurrentState;

        private TestMovementInputReader _testMovementInputReader;

        private void Awake()
        {
            _testMovementInputReader = new TestMovementInputReader();
        }

        private void OnEnable()
        {
            _gameStateManager.OnStateChanged += OnStateChange;
        }

        private void OnDisable()
        {
            _gameStateManager.OnStateChanged -= OnStateChange;
        }
        
        private void OnStateChange(BaseState state)
        {
            Debug.Log($"STATE: {state.GetType().Name}");
            
            if (state is LoadingState loadingState)
            {
                Debug.Log($"LOADING: {loadingState.CurrentScene} -> {loadingState.NextScene}");
            }

            if (state is PlayingState)
            {
                _testMovementInputReader.Enable();
            }
            else
            {
                _testMovementInputReader.Disable();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                switch (CurrentState)
                {
                    case MainMenuState:
                        _gameManager.QuitGame();
                        break;
                    case PlayingState or PausedState:
                        _gameManager.LoadMainMenu();
                        break;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                switch (CurrentState)
                {
                    case PlayingState:
                        _gameManager.PauseGame();
                        break;
                    case PausedState:
                        _gameManager.ResumeGame();
                        break;
                }
            }
        }
    }
}
