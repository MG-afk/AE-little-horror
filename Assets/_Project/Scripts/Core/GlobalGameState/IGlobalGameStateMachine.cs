namespace AE.Core.GlobalGameState
{
    public interface IGlobalGameStateMachine
    {
        void ChangeState(GameMode gameMode);
    }
}