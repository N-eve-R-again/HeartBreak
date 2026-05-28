using UnityEngine;

public class PlayerStateForwardHop : IPlayerState
{
    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();



    private float ringfrom, ringto;

    private bool wantstofwdash = false;
    private bool wantstobwdash = false;

    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {
        _currentData.time += Time.deltaTime;
        float ratio = _currentData.time / settings.BackwardHopSpeed;

        if (ratio > settings.hopInputBufferRatio)
        {

            if (_inputs.Forward.Down) { 
                wantstofwdash = true;
                wantstobwdash = false;
            }
            if (_inputs.Back.Down)
            {
                wantstobwdash = true;
                wantstofwdash = false;
            }

        }

      

        _currentData.position.distance = Mathf.LerpUnclamped(ringfrom, ringto, settings.backwardHopEase.Evaluate(ratio));

        _currentData.position.y = settings.backwardHopJumpCurve.Evaluate(ratio) * settings.BackwardHopSmallJumpAmplitude;

        _currentData.position += _currentData.velocity * Time.deltaTime;


        if (_currentData.time > settings.BackwardHopSpeed)
        {

            _currentData.time = 0;
            _currentData.position.distance = ringto;

            if (wantstobwdash) return this.GoTo<PlayerStateBackHop>();
            if (wantstofwdash) return this.ResetState(ref _currentData);
            if (_inputs.Attack.Held) return this.GoTo<PlayerStateForwardDashCharge>();
            return this.GoTo<PlayerStateIdle>();


        }


        return this.Stay();
    }

    public void Exit(ref PlayerEntityData _data)
    {
        wantstofwdash = false;
        wantstobwdash = false;
        return;
    }

    public void Enter(ref PlayerEntityData _data, IPlayerState _fromState)
    {
        wantstofwdash = false;
        wantstobwdash = false;
        _data.time = 0f;
        ringfrom = _data.position.distance;
        ringto = _data.position.distance - 1f;
        if (ringto <= 0) ringto = ringfrom;
        return;
    }
}
