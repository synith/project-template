using System.Threading.Tasks;
using UnityEngine;

namespace Slayground.Features.GameState
{
    public class PausedState : BaseState
    {
        public override async Task EnterState()
        {
            Time.timeScale = 0f;
            await Task.CompletedTask;
        }

        public override async Task ExitState()
        {
            Time.timeScale = 1f;
            await Task.CompletedTask;
        }
    }
}