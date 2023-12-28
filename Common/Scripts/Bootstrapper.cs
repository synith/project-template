using Injection;
using Slayground.Features.GameManager;
using Slayground.Injection;
using UnityEngine;

namespace Slayground.Common
{
    public class Bootstrapper : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() => ProjectContext.InstantiateAndInitialize();
        
        [Inject] private IGameManager _gameManager;

        private void Start()
        {
            _gameManager.LoadMainMenu();
        }
    }
}
