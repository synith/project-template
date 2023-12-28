using System.Threading.Tasks;

namespace Slayground.Features.GameState
{
    /// <summary>
    /// BaseState
    /// </summary>
    public abstract class BaseState
    {
        public abstract Task EnterState();
        public abstract Task ExitState();
    }
}