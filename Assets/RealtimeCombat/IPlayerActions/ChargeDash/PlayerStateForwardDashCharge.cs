using UnityEngine;

public class PlayerStateForwardDashCharge : IPlayerState
{
    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();
    private PlayerVFXLibrary vfx => PlayerStateRegistry.GetVfxLibrary();


    private float ringfrom;
    private int charge;
    private bool willattack;

    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {
        if (_inputs.Attack.Held)
        {
            _currentData.velocity = PolarCoordinate.Lerp(_currentData.velocity, PolarCoordinate.zero, Time.deltaTime * 0.5f);
            _currentData.time += Time.deltaTime;

            if (charge == 0)
            {
                _currentData.position.y = settings.chargeDashJump.Evaluate(_currentData.time / settings.chargeDashJumpDuration);
            }

            bool test = (ringfrom - charge == 1);
            float preshot = test ? vfx.attackGizmo.attackpreshot : vfx.attackGizmo.gotopreshot;

            if (_currentData.time > settings.chargeDashChargeTime - preshot)
            {
                if (test)
                {
                    vfx.attackGizmo.Attack();
                }
                else
                {
                    vfx.attackGizmo.MoveOneRing();
                }

            }

            if (_currentData.time > settings.chargeDashChargeTime)
            {
                _currentData.time = 0;



                if (ringfrom - charge > 1f)
                {
                    charge += 1;
                    Debug.Log("charge " + charge);

                    vfx.chargeCompletePS.Play();
                    vfx.ChargeEffectPulse(0.8f); // snap à 1, le sustain ramène à 0.5
                }
                else
                {
                    if (!willattack)
                    {
                        charge += 1;
                        willattack = true;

                        vfx.ChargeEffectPulse(1f);
                        vfx.ChargeEffectSustain(1f, 1f);
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
            if (charge < 1)
            {
                //vfx.ChargeEffectKill();
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

        // On dit juste "fade out" — la library le fait à son rythme
        vfx.ChargeEffectFadeOut(10f);

        _data.time = 0f;
        _data.velocity.distance = -charge;
    }

    public void Enter(ref PlayerEntityData _data, IPlayerState _fromState)
    {
        vfx.attackGizmo.Charge(_data.position);
        vfx.ActivateChargeCam();
        vfx.chargePS.Play();
        vfx.ChargeEffectKill();
        vfx.ChargeEffectSustain(0.25f, 2f);
        charge = 0;
        ringfrom = _data.position.distance;
        willattack = false;
        _data.time = 0f;
    }
}