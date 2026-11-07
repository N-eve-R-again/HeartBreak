using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerStateForwardDashCharge : IPlayerState
{
    public OrbitalDashComposer dashComposer;

    private float ringfrom;
    private int charge;
    private bool willattack;
    bool dashing;

    public void Init(PlayerContext ctx)
    {
        dashComposer = new OrbitalDashComposer(ctx.Settings.orbitalDashForceCurve, ctx.Settings.orbitalDashForce * 1.5f, 0f, 0.15f, 1f);
    }


    public Transition Execute(PlayerContext ctx)
    {
        if (ctx.Inputs.Attack.Held)
        {
            if (ctx.Inputs.Dash.Down && dashComposer.Done)
            {
                float dir = ctx.Inputs.Move != 0 ? -ctx.Inputs.Move : ctx.Data.velocity.angle;
                dashComposer.Init(dir);
                ctx.VFX.PlayOrbitalDashPS(Vector3.forward);
            }
            if (!dashComposer.Done)
            {
                ctx.Data.velocity.angle = dashComposer.Step(Time.deltaTime);
                ctx.Data.facing = dashComposer.GetFacing();
            }
            else
            {
                ctx.Data.velocity = PolarCoordinate.Lerp(ctx.Data.velocity, PolarCoordinate.zero, Time.deltaTime * ctx.Settings.chargeDashFriction);

                bool drifing = Mathf.Sign(ctx.Inputs.Move) == Mathf.Sign(ctx.Data.velocity.angle);
                if (ctx.Inputs.MoveMagnitude == 0) drifing = false;
                if (drifing)
                {
                    ctx.Data.velocity = PolarCoordinate.Lerp(ctx.Data.velocity, PolarCoordinate.zero, Time.deltaTime * ctx.Settings.chargeDashInputInfluence);
                }

                if (charge == 0)
                {
                    ctx.Data.position.y = ctx.Settings.chargeDashJump.Evaluate(ctx.Data.time / ctx.Settings.chargeDashJumpDuration);
                }

                Vector2 tempFacing = Mathf.Sign(ctx.Data.velocity.angle) < 0 ? Vector2.left : Vector2.right;
                float tempForce = drifing ? 0.50f : 0f;
                ctx.Data.facing = Vector2.up + tempFacing * tempForce;

            }


            bool test = (ringfrom - charge <= 2);
            float preshot = test ? ctx.VFX.attackGizmo.attackpreshot : ctx.VFX.attackGizmo.gotopreshot;

            if (ctx.Data.time > ctx.Settings.chargeDashChargeTime - preshot)
            {
                if (test)
                {
                    ctx.VFX.attackGizmo.Attack();
                }
                else
                {
                    ctx.VFX.attackGizmo.MoveOneRing();
                }

            }

            if (ctx.Data.time > ctx.Settings.chargeDashChargeTime)
            {
                ctx.Data.time = 0;

                if (ringfrom - charge > 2f)
                {
                    charge += 1;
                    Debug.Log("charge " + charge);

                    ctx.VFX.chargeCompletePS.Play();
                    ctx.VFX.ChargeEffectPulse(0.8f); // snap à 1, le sustain ramène à 0.5
                }
                else
                {
                    if (!willattack)
                    {
                        charge += 1;
                        willattack = true;

                        ctx.VFX.ChargeEffectPulse(1f);
                        ctx.VFX.ChargeEffectSustain(1f, 1f);
                     }
                }
            }
            ctx.VFX.attackGizmo.UpdatePlayerPose(ctx.Data.position + new PolarCoordinate(0f, -charge, 0f));

            ctx.Data.position += ctx.Data.velocity * Time.deltaTime;

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

    public void Exit(PlayerContext ctx)
    {
        dashComposer.ForceDone();
        ctx.VFX.attackGizmo.Validate();
        ctx.VFX.chargePS.Stop();
        ctx.VFX.DeactivateChargeCam();

        // On dit juste "fade out" — la library le fait à son rythme
        ctx.VFX.ChargeEffectFadeOut(20f);
        ctx.Data.position.y = 0f;
        ctx.Data.velocity.distance = -charge;
    }

    public void Enter(PlayerContext ctx, IPlayerState _fromState)
    {
        if(ctx.Data.velocity.angleMagnitude < ctx.Settings.moveangleMaxSpeed)
        {
            ctx.Data.velocity.angle = Mathf.Sign(ctx.Data.velocity.angle) * ctx.Settings.chargeDashVelocityBoost;
        }
        else
        {
            ctx.Data.velocity.angle = Mathf.Sign(ctx.Data.velocity.angle) * (ctx.Settings.moveangleMaxSpeed + (ctx.Settings.chargeDashVelocityBoost- ctx.Settings.moveangleMaxSpeed));
        }


        ctx.VFX.attackGizmo.Charge(ctx.Data.position);
        ctx.VFX.ActivateChargeCam();
        ctx.VFX.chargePS.Play();
        ctx.VFX.ChargeEffectKill();
        ctx.VFX.ChargeEffectSustain(0.25f, 2f);
        charge = 0;
        ringfrom = ctx.Data.position.distance;
        willattack = false;
    }

    public void ResetInternal()
    {

        return;
    }
}