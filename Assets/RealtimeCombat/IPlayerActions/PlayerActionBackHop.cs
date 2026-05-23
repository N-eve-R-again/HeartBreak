using System.IO;
using UnityEngine;

public class PlayerActionBackHop : IPlayerAction
{
    private readonly PlayerControllerSettings settings;

    private float ringfrom, ringto;

    public PlayerActionBackHop(PlayerControllerSettings settings)
    {
        this.settings = settings;
    }

    public void Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {
        if(ringfrom == ringto)
        {
            _currentData.nextState = PlayerController.PlayerState.Idle;
            return;
        }

        _currentData.time += Time.deltaTime;

        _currentData.position.distance = Mathf.LerpUnclamped(ringfrom,ringto, settings.backwardHopEase.Evaluate(_currentData.time / settings.BackwardHopSpeed));

        _currentData.position.y = settings.backwardHopJumpCurve.Evaluate(_currentData.time / settings.BackwardHopSpeed) * settings.BackwardHopSmallJumpAmplitude;

        _currentData.position += _currentData.velocity * Time.deltaTime;


        if (_currentData.time > settings.BackwardHopSpeed) {
        
            _currentData.nextState = PlayerController.PlayerState.Idle;
            _currentData.time = 0;
            _currentData.position.distance = ringto;
            return;
        }

        

        _currentData.nextState = PlayerController.PlayerState.BackHop;
        return;
    }

    public void Exit(ref PlayerEntityData _data)
    {
        return;
    }

    public void Enter(ref PlayerEntityData _data, PlayerController.PlayerState _fromState)
    {
        _data.time = 0f;
        ringfrom = _data.position.distance;
        ringto = _data.position.distance + 1f;
        if(ringto > settings.ringRadii.Length) ringto = ringfrom;
        return;
    }
}