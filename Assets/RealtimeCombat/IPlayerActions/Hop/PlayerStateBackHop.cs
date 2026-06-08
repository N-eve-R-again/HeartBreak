using System.IO;
using UnityEngine;

public class PlayerStateBackHop : IPlayerState
{
    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();

    private float ringfrom, ringto;

    private bool wantstofwdash = false;

    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {


        wantstofwdash = _inputs.Forward.Held;
        

        _currentData.position.distance = Mathf.LerpUnclamped(ringfrom,ringto, settings.backwardHopEase.Evaluate(_currentData.time / settings.BackwardHopSpeed));

        _currentData.position.y = settings.backwardHopJumpCurve.Evaluate(_currentData.time / settings.BackwardHopSpeed) * settings.BackwardHopSmallJumpAmplitude;

        _currentData.position += _currentData.velocity * Time.deltaTime;


        if (_currentData.time > settings.BackwardHopSpeed) {

            _currentData.time = 0;
            _currentData.position.distance = ringto;

            if (wantstofwdash) return this.GoTo<PlayerStateForwardHop>();
            if (_inputs.Attack.Held) return this.GoTo<PlayerStateForwardDashCharge>();
            if (_inputs.Back.Held) { 

                return this.ResetState(ref _currentData); 
            }
            return this.GoTo<PlayerStateIdle>();


        }


        return this.Stay();
    }

    public void Exit(ref PlayerEntityData _data)
    {
        return;
    }

    public void Enter(ref PlayerEntityData _data, IPlayerState _fromState)
    {

        wantstofwdash = false;
        _data.time = 0f;
        ringfrom = _data.position.distance;
        ringto = Mathf.RoundToInt(_data.position.distance + 1f);
        if(ringto >= settings.ringRadii.Length) ringto = ringfrom;
        return;
    }
}