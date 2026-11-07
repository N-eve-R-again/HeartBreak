using UnityEngine;

public class PlayerStateForwardDashCharge : IPlayerState
{
    public OrbitalDashComposer dashComposer;
    public OrbitalDriftComposer driftComposer;

    private float ringfrom;
    private int charge;
    private bool willattack;

    public void Init(PlayerContext ctx)
    {
        dashComposer = new OrbitalDashComposer(
            ctx.Settings.orbitalDashForceCurve, 
            ctx.Settings.orbitalDashForce * 1.5f,
            0f,
            0.15f,
            1f);

        driftComposer = new OrbitalDriftComposer(
            ctx.Settings.chargeDashFriction,
            ctx.Settings.chargeDashInputInfluence,
            0.5f,
            ctx.Settings.chargeDashJump,
            ctx.Settings.chargeDashJumpDuration,
            1f);
    }


    public Transition Execute(PlayerContext ctx)
    {
        if (ctx.Inputs.Attack.Held)
        {
            if (ctx.Inputs.Dash.Down && !dashComposer.Dashing && ctx.Data.TryConsumeEnergy())
            {
                float dir = ctx.Inputs.Move != 0 ? -ctx.Inputs.Move : Mathf.Sign(ctx.Data.velocity.angle);

                dashComposer.Init(dir);

                ctx.VFX.PlayOrbitalDashPS(Vector3.forward);
            }

            if (dashComposer.Dashing)
            {
                ctx.Data.velocity.angle = dashComposer.Step(Time.deltaTime);
                ctx.Data.facing = dashComposer.GetFacing();
            }
            else
            {
                ctx.Data.velocity = driftComposer.Step(ctx.Data.velocity, ctx.Inputs.Move, Time.deltaTime);
                ctx.Data.facing = driftComposer.GetFacing(ctx.Data.velocity.angle, ctx.Inputs.Move);
                if (charge == 0) ctx.Data.position.y = driftComposer.GetJumpHeight(ctx.Data.time);
            }


            bool nextStepIsAttack = (ringfrom - charge <= 2);
            if (ctx.Data.time > ctx.Settings.chargeDashChargeTime - ctx.VFX.attackGizmo.GetPreshot(nextStepIsAttack))
                ctx.VFX.attackGizmo.Advance(nextStepIsAttack);


            if (ctx.Data.time > ctx.Settings.chargeDashChargeTime)
            {
                ctx.Data.time = 0;

                if (nextStepIsAttack)
                {
                    if (!willattack)
                    {
                        charge += 1;
                        willattack = true;

                        ctx.VFX.ChargeEffectPulse(1f);
                        ctx.VFX.ChargeEffectSustain(1f, 1f);
                    }

                }
                else
                {
                    charge += 1;
                    Debug.Log("charge " + charge);

                    ctx.VFX.chargeCompletePS.Play();
                    ctx.VFX.ChargeEffectPulse(0.8f); // snap à 1, le sustain ramène à 0.5
                }
            }


            ctx.Data.position += ctx.Data.velocity * Time.deltaTime;
            ctx.VFX.attackGizmo.UpdatePlayerPose(ctx.Data.position + new PolarCoordinate(0f, -charge, 0f));

            return this.Stay();
        }

        if (charge < 1)
        {
            ctx.VFX.ChargeEffectKill();
            return this.Done();
        }
        else
        {
            if (willattack) return this.GoTo<PlayerStateAttackDash>();
            return this.GoTo<PlayerStateForwardDashExecute>();
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

        ringfrom = ctx.Data.position.distance;

    }

    public void ResetInternal()
    {
        ringfrom = 0;
        charge = 0;
        willattack = false;
    }
}