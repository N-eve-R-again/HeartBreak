using UnityEngine;

public class PlayerStateForwardHop : IPlayerState
{
    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();



    private float ringfrom, ringto;

    private bool wantstofwdash = false;
    private bool wantstobwdash = false;

    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {
        float ratio = _currentData.time / settings.ForwardHopSpeed;

        if (ratio > settings.hopInputBufferRatio)
        {

            if (_inputs.Forward.Down) { 
                wantstofwdash = true;
                wantstobwdash = false;
            }
            if (_inputs.Back.Held)
            {
                wantstobwdash = true;
                wantstofwdash = false;
            }

        }

      

        _currentData.position.distance = Mathf.LerpUnclamped(ringfrom, ringto, settings.forwardHopEase.Evaluate(ratio));

        //_currentData.position.y = settings.backwardHopJumpCurve.Evaluate(ratio) * settings.BackwardHopSmallJumpAmplitude;

        _currentData.position += _currentData.velocity * Time.deltaTime;


        if (ratio > settings.forwardHopInteruptRatio) {

            if (wantstobwdash)
            {
                Debug.Log("Interupted forward hop for backward hop");
                return this.GoTo<PlayerStateBackHop>();
            }
        }

        if (ratio > 1f)
        {
            _currentData.position.distance = ringto;

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
