using Injection;

namespace Slayground.Features.GameState
{
    public class GameStateManagerInstaller : InstallerBase
    {
        private IGameStateManager _gameStateManager;
        
        public override void CreateAll()
        {
            base.CreateAll();
            _gameStateManager = new GameStateManager();
        }

        public override void BindAll(Injector injector)
        {
            base.BindAll(injector);
            injector.Bind(_gameStateManager);
        }
    }
}