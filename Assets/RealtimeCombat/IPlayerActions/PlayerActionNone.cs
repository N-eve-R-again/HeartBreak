using UnityEngine;


public class PlayerActionNone : IPlayerAction
{
    private readonly PlayerControllerSettings settings;

    public PlayerActionNone(PlayerControllerSettings settings)
    {
        this.settings = settings;
    }

    public void Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {
        return;
    }

    public void Exit(ref PlayerEntityData _data)
    {
        return;
    }

    public void Enter(ref PlayerEntityData _data, PlayerController.PlayerState _fromState)
    {
        return;
    }
}