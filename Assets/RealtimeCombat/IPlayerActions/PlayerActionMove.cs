using UnityEditor;
using UnityEngine;

public class PlayerActionMove : IPlayerAction
{
    private readonly PlayerControllerSettings settings;

    public PlayerActionMove(PlayerControllerSettings settings)
    {
        this.settings = settings;
    }
    public void Enter(ref PlayerEntityData _data, PlayerController.PlayerState _fromState)
    {

        return;
    }

    public void Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {
        PolarCoordinate targetVelocity = PolarCoordinate.zero;


        if (_inputs.Move.magnitude > 0)
        {
            // Velocity directe basée sur inputd
            targetVelocity.angle = -_inputs.Move.x * (settings.moveangleMaxSpeed / _currentData.position.distance);
            targetVelocity.distance = -_inputs.Move.y * settings.movedistMaxSpeed;

            //_currentData.facing = new Vector2(MoveInputCameraSpace.x, MoveInputCameraSpace.z);
        }

        // Un seul lerp : velocity vers target
        float lerpSpeed = _inputs.Move.magnitude > 0 ? settings.moveAccel : settings.moveDecel;
        _currentData.velocity = PolarCoordinate.Lerp(_currentData.velocity, targetVelocity, lerpSpeed * Time.deltaTime);

        // Appliquer
        _currentData.position += _currentData.velocity * Time.deltaTime;

        _currentData.position = PlayerEntityData.Solve(_currentData.position, settings.minDist, settings.maxDist);
        // Transition vers Idle
        if (_inputs.Move.magnitude <= 0.1f && _currentData.velocity.magnitude < 0.1f)
        {
            _currentData.velocity = PolarCoordinate.zero;
            _currentData.nextState = PlayerController.PlayerState.Idle;
            return;
        }

        _currentData.nextState = PlayerController.PlayerState.Move;
        return;
    }

    private float DT(float value)
    {
        return value * Time.deltaTime;
    }
    private Vector3 DT(Vector3 vector)
    {
        return vector * Time.deltaTime;
    }

    public void Exit(ref PlayerEntityData _data)
    {
        return;
    }

}
