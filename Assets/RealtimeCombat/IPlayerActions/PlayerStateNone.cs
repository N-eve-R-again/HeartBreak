using UnityEngine;


public class PlayerStateNone : IPlayerState
{
    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();

    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {
        return this.Stay();
    }

    public void Exit(ref PlayerEntityData _data)
    {
        return;
    }

    public void Enter(ref PlayerEntityData _data, IPlayerState _fromState)
    {
        return;
    }
}