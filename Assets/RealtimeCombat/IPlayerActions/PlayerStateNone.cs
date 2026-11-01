using UnityEngine;


public class PlayerStateNone : IPlayerState
{
    public Transition Execute(PlayerContext ctx)
    {
        return this.Stay();
    }

    public void Exit(PlayerContext ctx)
    {
        return;
    }

    public void Enter(PlayerContext ctx, IPlayerState _fromState)
    {
        return;
    }

    public void ResetInternal()
    {
        return;
    }
}