using UnityEngine;

public class PlayerActionIdle : IPlayerAction
{
    private readonly PlayerControllerSettings settings;

    public PlayerActionIdle(PlayerControllerSettings settings)
    {
        this.settings = settings;
    }

    public void Enter(ref PlayerEntityData _data, PlayerController.PlayerState _fromState)
    {

        return;
    }

    public void Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {
        _currentData.velocity = PolarCoordinate.Lerp(_currentData.velocity, PolarCoordinate.zero, settings.idleDecel * Time.deltaTime);
        if (_currentData.velocity.magnitude < 0.1f) _currentData.velocity = PolarCoordinate.zero;
        _currentData.position += _currentData.velocity * Time.deltaTime;

        _currentData.position = PlayerEntityData.Solve(_currentData.position, settings.minDist, settings.maxDist);

        if (_inputs.Move.magnitude < 0.1f)
        {
            _currentData.nextState = PlayerController.PlayerState.Idle;
            return;
        }
        else
        {
            _currentData.nextState = PlayerController.PlayerState.Move;
            return;
        }
    }

    public void Exit(ref PlayerEntityData _data)
    {
        return;
    }
}
