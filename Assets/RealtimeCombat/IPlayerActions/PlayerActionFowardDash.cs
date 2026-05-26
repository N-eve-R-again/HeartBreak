using UnityEngine;
using UnityEngine.Rendering;

public class PlayerStateForwardDashCharge : IPlayerState
{
    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();
    private PlayerVFXLibrary vfx => PlayerStateRegistry.GetVfxLibrary();


    private float ringfrom;
    private int charge;
    float screeneffectforce;

    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {

        vfx.chargeVolume.weight = screeneffectforce;
        vfx.SetScreenPowerUpAlpha(screeneffectforce);

        if (_inputs.Forward.Held)
        {
            _currentData.velocity = PolarCoordinate.Lerp(_currentData.velocity, PolarCoordinate.zero, Time.deltaTime * 0.5f);
            _currentData.time += Time.deltaTime;
            screeneffectforce = Mathf.Lerp(screeneffectforce, 0.5f, Time.deltaTime * 2f);

            if(charge == 0)
            {
                _currentData.position.y = settings.smalljump.Evaluate(_currentData.time / settings.smalljumptime);
            }

            if (_currentData.time > settings.forwardDashChargeTime)
            {
                _currentData.time = 0;

                if (ringfrom - charge > 1f)
                {
                    charge += 1;
                    Debug.Log("charge " + charge);

                    vfx.chargeCompletePS.Play();
                    screeneffectforce = 1f;
                }


            }


            _currentData.position += _currentData.velocity * Time.deltaTime;
            _currentData.facing = Vector2.up + Vector2.right * (_inputs.Move * 0.35f);
            return this.Stay();
        }
        else
        {
            if(charge < 1)
            {
                screeneffectforce = 0f;
                vfx.chargeVolume.weight = screeneffectforce;
                vfx.SetScreenPowerUpAlpha(screeneffectforce);
                vfx.DeactivateChargeCam();
                return this.GoTo<PlayerStateIdle>();
            }
            else
            {
                vfx.chargeCompletePS.Play();
                return this.GoTo<PlayerStateForwardDashExecute>();
            }

        }


    }

    public void Exit(ref PlayerEntityData _data)
    {

        vfx.chargePS.Stop();


        _data.time = 0f;
        _data.velocity.distance = -charge;

        return;
    }

    public void Enter(ref PlayerEntityData _data, IPlayerState _fromState)
    {
        vfx.ActivateChargeCam();
        vfx.chargePS.Play();
        vfx.chargeVolume.weight = 0f;
        charge = 0;
        ringfrom = _data.position.distance;
        screeneffectforce = 0f;
        //_data.velocity = PolarCoordinate.zero;
        _data.time = 0f;
        return;
    }
}


public class PlayerStateForwardDashExecute : IPlayerState
{
    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();
    private PlayerVFXLibrary vfx => PlayerStateRegistry.GetVfxLibrary();

    private float ringfrom, ringto;

    private float timeToExecute;

    private Vector2 originalfacing;

    public void Enter(ref PlayerEntityData data, IPlayerState fromState)
    {
        originalfacing = data.facing;
        timeToExecute = Mathf.Abs(settings.forwardDashSpeedByChargeIncr * data.velocity.distance + settings.forwardDashSpeedByChargeBase);
        ringfrom = data.position.distance;
        ringto = data.position.distance + data.velocity.distance;
        data.velocity.distance = 0f;
    }

    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData inputs)
    {

        Debug.Log("dashing");

        _currentData.time += Time.deltaTime;

        float ratio = _currentData.time / timeToExecute;
        vfx.chargeVolume.weight = Mathf.Lerp(vfx.chargeVolume.weight, 0f, ratio);
        vfx.SetScreenPowerUpAlpha(Mathf.Lerp(vfx.screenPowerUp.alpha, 0f, ratio));
        _currentData.position.distance = Mathf.LerpUnclamped(ringfrom, ringto, settings.forwardDashEase.Evaluate(ratio));
        _currentData.velocity.angle = Mathf.Lerp(_currentData.velocity.angle, settings.moveangleMaxSpeed * -inputs.Move / settings.ringToDistance.Evaluate(_currentData.position.distance), settings.forwardDashAngularRampUp.Evaluate(ratio));
        _currentData.position.angle += _currentData.velocity.angle * Time.deltaTime;
        _currentData.facing = Vector2.Lerp(originalfacing, new Vector2(-_currentData.velocity.angle, 1f).normalized, settings.forwardDashAngularRampUp.Evaluate(ratio));

        if (_currentData.time > timeToExecute)
        {
            vfx.DeactivateChargeCam();
            vfx.chargeVolume.weight = 0f;
            if (inputs.MoveMagnitude > 0f)
            {
                return this.GoTo<PlayerStateMove>();
            }
            return this.GoTo<PlayerStateIdle>();
        }

        return this.Stay();
    }

    public void Exit(ref PlayerEntityData data)
    {
        data.time = 0;
        data.position.distance = ringto;

    }
}