using System.Threading.Tasks;
using UnityEngine;

namespace Slayground.Features.GameState
{
    public class GameStateManager : IGameStateManager
    {
        public event System.Action<BaseState> OnStateChanged;

        public BaseState CurrentState { get; private set; }

        public async Task ChangeState(BaseState state)
        {
            if (state == null) return;
            if (CurrentState != null)
            {
                if (CurrentState.GetType() == state.GetType()) return;
                await CurrentState.ExitState();
            }
            
            CurrentState = state;
            await CurrentState.EnterState();
            OnStateChanged?.Invoke(state);
        }
    }

    public interface IGameStateManager
    {
        public BaseState CurrentState { get; }

        public event System.Action<BaseState> OnStateChanged;

        Task ChangeState(BaseState state);
    }
}