using UnityEngine;

public class PlayerStateForwardDashExecute : IPlayerState
{
    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();
    private PlayerVFXLibrary vfx => PlayerStateRegistry.GetVfxLibrary();

    private float ringfrom, ringto;

    private float timeToExecute;
    private float originalangularVel;

    private Vector2 originalfacing;

    public void Enter(ref PlayerEntityData data, IPlayerState fromState)
    {
        originalfacing = data.facing;
        timeToExecute = Mathf.Abs(settings.forwardDashSpeedByChargeIncr * data.velocity.distance) + settings.forwardDashSpeedByChargeBase;
        ringfrom = data.position.distance;
        ringto = data.position.distance + data.velocity.distance;
        data.velocity.distance = 0f;
        originalangularVel = data.velocity.angle;

        vfx.shooter.ShootPeel(data.position);
    }

    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData inputs)
    {

        Debug.Log("dashing");

        float ratio = _currentData.time / timeToExecute;
        _currentData.position.distance = Mathf.LerpUnclamped(ringfrom, ringto, settings.forwardDashEase.Evaluate(ratio));
        //_currentData.velocity.angle = Mathf.Lerp(_currentData.velocity.angle, settings.moveangleMaxSpeed * -inputs.Move, settings.forwardDashAngularRampUp.Evaluate(ratio) * Time.deltaTime);
        _currentData.position.angle += _currentData.velocity.angle * Time.deltaTime;
        //_currentData.facing = Vector2.Lerp(originalfacing, new Vector2(-_currentData.velocity.angle, 1f).normalized, settings.forwardDashAngularRampUp.Evaluate(ratio));

        if(Mathf.Sign(inputs.Move) != Mathf.Sign(_currentData.velocity.angle)){
            _currentData.velocity.angle = Mathf.Lerp(_currentData.velocity.angle, originalangularVel * 0.45f, ratio);
            Debug.Log("what ?");
        }

        if (_currentData.time > timeToExecute)
        {


            if (Mathf.Sign(inputs.Move) != Mathf.Sign(_currentData.velocity.angle))
            {
                _currentData.velocity.angle = settings.moveangleMaxSpeed * -inputs.Move * 1.25f;
            }
            else
            {
                _currentData.velocity.angle = settings.moveangleMaxSpeed * -inputs.Move;
            }

            return this.GoTo<PlayerStateMove>();

        }

        return this.Stay();
    }

    public void Exit(ref PlayerEntityData data)
    {
        data.position.distance = ringto;

    }
}
