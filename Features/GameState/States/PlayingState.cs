using System.Threading.Tasks;
using UnityEngine;

namespace Slayground.Features.GameState
{
    public class PlayingState : BaseState
    {
        public override async Task EnterState()
        {
            await Task.CompletedTask;
        }

        public override async Task ExitState()
        {
            await Task.CompletedTask;
        }
    }
}