using UnityEngine;

public class PlayerStateForwardDashCharge : IPlayerState
{
    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();
    private PlayerVFXLibrary vfx => PlayerStateRegistry.GetVfxLibrary();


    private float ringfrom;
    private int charge;
    float screeneffectforce;
    private bool willattack;

    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {

        vfx.chargeVolume.weight = screeneffectforce;
        vfx.SetScreenPowerUpAlpha(screeneffectforce);


        if (_inputs.Attack.Held)
        {
            _currentData.velocity = PolarCoordinate.Lerp(_currentData.velocity, PolarCoordinate.zero, Time.deltaTime * 0.5f);
            _currentData.time += Time.deltaTime;
            screeneffectforce = Mathf.Lerp(screeneffectforce, 0.5f, Time.deltaTime * 2f);

            if(charge == 0)
            {
                _currentData.position.y = settings.chargeDashJump.Evaluate(_currentData.time / settings.chargeDashJumpDuration);
            }

            if (_currentData.time > settings.chargeDashChargeTime)
            {
                _currentData.time = 0;

                if (ringfrom - charge > 1f)
                {
                    charge += 1;
                    Debug.Log("charge " + charge);
                    vfx.attackGizmo.MoveOneRing();
                    vfx.chargeCompletePS.Play();
                    screeneffectforce = 1f;
                }
                else
                {
                    if (!willattack)
                    {
                        charge += 1;
                        willattack = true;
                        vfx.attackGizmo.Attack();
                        screeneffectforce = 1f;

                    }
                }


            }
            vfx.attackGizmo.UpdatePlayerPose(_currentData.position + new PolarCoordinate(0f, -charge, 0f));

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

                return this.GoTo<PlayerStateIdle>();
            }
            else
            {
                if (willattack) return this.GoTo<PlayerStateAttackDash>();
                return this.GoTo<PlayerStateForwardDashExecute>();
            }

        }


    }

    public void Exit(ref PlayerEntityData _data)
    {
        vfx.attackGizmo.Validate();
        vfx.chargePS.Stop();
        vfx.DeactivateChargeCam();

        _data.time = 0f;
        _data.velocity.distance = -charge;

        return;
    }

    public void Enter(ref PlayerEntityData _data, IPlayerState _fromState)
    {
        vfx.attackGizmo.Charge(_data.position);
        vfx.ActivateChargeCam();
        vfx.chargePS.Play();
        vfx.chargeVolume.weight = 0f;
        charge = 0;
        ringfrom = _data.position.distance;
        screeneffectforce = 0f;
        willattack = false;
        //_data.velocity = PolarCoordinate.zero;
        _data.time = 0f;
        return;
    }
}


