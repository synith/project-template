using Injection;

namespace Slayground.Features.GameManager
{
    public class GameManagerInstaller : InstallerBase
    {
        private IGameManager _gameManager;
        
        public override void CreateAll()
        {
            base.CreateAll();
            _gameManager = new GameManager();
        }

        public override void BindAll(Injector injector)
        {
            base.BindAll(injector);
            injector.Bind(_gameManager);
        }

        public override void InjectAll(Injector injector)
        {
            base.InjectAll(injector);
            injector.Inject(_gameManager);
        }
    }
}