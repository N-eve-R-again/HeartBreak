using UnityEngine;

public class PlayerStateIdle : IPlayerState
{
    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();

    public void Enter(ref PlayerEntityData _data, IPlayerState _fromState)
    {

        return;
    }

    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {

        if (_inputs.Back.Down) return this.GoTo<PlayerStateBackHop>();

        if (_inputs.Forward.Down) return this.GoTo<PlayerStateForwardDash>();


        _currentData.velocity = PolarCoordinate.Lerp(_currentData.velocity, PolarCoordinate.zero, settings.idleDecel * Time.deltaTime);
        if (_currentData.velocity.magnitude < 0.1f) _currentData.velocity = PolarCoordinate.zero;
        _currentData.position += _currentData.velocity * Time.deltaTime;


        if (_inputs.MoveMagnitude < 0.1f)
        {
            return this.Stay();
        }
        else
        {
            return this.GoTo<PlayerStateMove>();
        }
    }

    public void Exit(ref PlayerEntityData _data)
    {
        return;
    }
}
