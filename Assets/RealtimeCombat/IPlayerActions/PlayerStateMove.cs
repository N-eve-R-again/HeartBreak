using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerStateMove : IPlayerState
{
    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();
    private PlayerVFXLibrary vfx => PlayerStateRegistry.GetVfxLibrary();

    public void Enter(ref PlayerEntityData _data, IPlayerState _fromState)
    {

        return;
    }
 


    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {
        PolarCoordinate targetVelocity = PolarCoordinate.zero;

        if (_inputs.Back.Down) return this.GoTo<PlayerStateBackHop>();

        if (_inputs.Forward.Down) return this.GoTo<PlayerStateForwardHop>();

        if (_inputs.Attack.Down) return this.GoTo<PlayerStateForwardDashCharge>();

        if (_inputs.Dash.Down) return this.GoTo<PlayerStateOrbitalDash>();

        if (Mathf.Abs(_inputs.Move) > 0)
        {
            // Velocity directe basée sur inputd
            targetVelocity.angle = -_inputs.Move * (settings.moveangleMaxSpeed);// / settings.ringToDistance.Evaluate(_currentData.position.distance));
            targetVelocity.distance = 0; // -_inputs.Move.y * settings.movedistMaxSpeed;

            //_currentData.facing = new Vector2(MoveInputCameraSpace.x, MoveInputCameraSpace.z);
        }

        // Un seul lerp : velocity vers target
        float lerpSpeed = _currentData.velocity.magnitude < targetVelocity.magnitude ? settings.moveAccel : settings.moveDecel;
        _currentData.velocity = PolarCoordinate.Lerp(_currentData.velocity, targetVelocity, lerpSpeed * Time.deltaTime);

        // Appliquer
        _currentData.position += _currentData.velocity * Time.deltaTime;

        // Transition vers Idle
        if (_inputs.MoveMagnitude <= 0.1f && _currentData.velocity.magnitude < 0.1f)
        {
            _currentData.velocity = PolarCoordinate.zero;
            _currentData.facing = Vector2.up;
            return this.GoTo<PlayerStateIdle>();
        }
            _currentData.facing = Mathf.Abs(_inputs.Move) >= 0.1f ? new Vector2(_inputs.Move, 0f) : Vector2.up;

        return this.Stay();
        
    }

    public void Exit(ref PlayerEntityData _data)
    {
        return;
    }

}

