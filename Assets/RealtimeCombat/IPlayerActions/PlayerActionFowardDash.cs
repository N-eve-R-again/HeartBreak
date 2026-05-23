using UnityEngine;

public class PlayerActionForwardDash : IPlayerAction
{
    private readonly PlayerControllerSettings settings;

    private float ringfrom, ringto;
    private int charge;
    private float timeToExecute;
    public PlayerActionForwardDash(PlayerControllerSettings settings)
    {
        this.settings = settings;
    }

    public void Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
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
            _currentData.nextState = PlayerController.PlayerState.ForwardDash;
            return;
        }

        if (_inputs.Forward.Up)
        {
            Debug.Log("executing");
            _currentData.time = 0f;
            ringto = ringfrom - charge;
            timeToExecute = settings.forwardDashSpeedByCharge * charge;

            _currentData.nextState = PlayerController.PlayerState.ForwardDash;
            return;
        }

        Debug.Log("dashing");

        _currentData.time += Time.deltaTime;

        _currentData.position.distance = Mathf.LerpUnclamped(ringfrom,ringto, settings.forwardDashEase.Evaluate(_currentData.time / timeToExecute));
        _currentData.nextState = PlayerController.PlayerState.ForwardDash;

        if (_currentData.time > timeToExecute) {
        
            _currentData.nextState = PlayerController.PlayerState.Idle;
            _currentData.time = 0;
            _currentData.position.distance = ringto;
            return;
        }

    }

    public void Exit(ref PlayerEntityData _data)
    {
        return;
    }

    public void Enter(ref PlayerEntityData _data, PlayerController.PlayerState _fromState)
    {
        charge = 0;
        ringfrom = _data.position.distance;
        _data.velocity = PolarCoordinate.zero;
        _data.time = 0f;
        return;
    }
}