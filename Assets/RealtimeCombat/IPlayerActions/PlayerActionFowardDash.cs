using UnityEngine;

public class PlayerStateForwardDash : IPlayerState
{
    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();

    private float ringfrom, ringto;
    private int charge;
    private float timeToExecute;


    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {
        _currentData.time += Time.deltaTime;


        if (_inputs.Forward.Held)
        {
            Debug.Log("hold");
            if(_currentData.time > settings.forwardDashChargeTime)
            {
                _currentData.time = 0;

                if(ringfrom - charge > 1f)
                {
                    charge += 1;
                    Debug.Log("charge " + charge);
                }
                //feedback

            }
            return this.Stay();
        }

        if (_inputs.Forward.Up)
        {
            Debug.Log("executing");
            _currentData.time = 0f;
            ringto = ringfrom - charge;
            timeToExecute = settings.forwardDashSpeedByCharge * charge;

            return this.Stay();
        }

        Debug.Log("dashing");

        _currentData.time += Time.deltaTime;

        _currentData.position.distance = Mathf.LerpUnclamped(ringfrom,ringto, settings.forwardDashEase.Evaluate(_currentData.time / timeToExecute));

        if (_currentData.time > timeToExecute) {
        
            _currentData.time = 0;
            _currentData.position.distance = ringto;
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
        charge = 0;
        ringfrom = _data.position.distance;
        _data.velocity = PolarCoordinate.zero;
        _data.time = 0f;
        return;
    }
}