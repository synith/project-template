using Injection;
using Slayground.Features.GameManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Slayground.Features.MainMenu.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;
        
        [Inject] private IGameManager _gameManager;
        
        private void Awake()
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
            quitButton.onClick.AddListener(OnQuitButtonClicked);
            
            InputSystem.onDeviceChange += OnDeviceChange;
        }

        private void Start()
        {
            playButton.Select();
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
            quitButton.onClick.RemoveListener(OnQuitButtonClicked);
            
            InputSystem.onDeviceChange -= OnDeviceChange;
        }
        
        private void OnPlayButtonClicked()
        {
            _gameManager.StartGame();
        }
        
        private void OnQuitButtonClicked()
        {
            _gameManager.QuitGame();
        }
        
        private void OnDeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
        {
            if (inputDeviceChange == InputDeviceChange.Added)
            {
                if (inputDevice is Gamepad)
                {
                    playButton.Select();
                }
            }
        }
    }
}
