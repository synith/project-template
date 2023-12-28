using Injection;

namespace Slayground.Features.SceneLoader
{
    public class SceneLoaderInstaller : InstallerBase
    {
        private ISceneLoader _sceneLoader;
        
        public override void CreateAll()
        {
            base.CreateAll();
            _sceneLoader = new SceneLoader();
        }

        public override void BindAll(Injector injector)
        {
            base.BindAll(injector);
            injector.Bind(_sceneLoader);
        }
    }
}