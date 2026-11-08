using UnityEngine;
public class PlayerStateOrbitalDash : IPlayerState
{
    public OrbitalDashComposer dashComposer;
    public bool invalid;

    public void Init(PlayerContext ctx)
    {
        dashComposer = new OrbitalDashComposer(ctx.Settings.orbitalDashForceCurve, ctx.Settings.orbitalDashForce, 0f, ctx.Settings.orbitalDashDuration, 1f);
    }

    public Transition Execute(PlayerContext ctx)
    {
        if (invalid) return this.Done();

        float ratio = dashComposer.GetRatio();
        if (ratio < 0.15f)
        {
            float currentdir = dashComposer.GetDir();
            if (currentdir == ctx.Inputs.Move)
            {
                dashComposer.SetDir(-currentdir);
            }
        }



        //ctx.Data.velocity.angle  = ctx.Settings.orbitalDashForceCurve.Evaluate(ratio) * ctx.Settings.orbitalDashForce * dir;
        
        ctx.Data.velocity.angle = dashComposer.Step(Time.deltaTime);
        ctx.Data.position += ctx.Data.velocity * Time.deltaTime;


        if (dashComposer.Done)
        {
            if (ctx.Inputs.Attack.Held)
            {
                return this.GoTo<PlayerStateForwardDashCharge>();
            }
            return this.Done();
        }


        return this.Stay();
    }


    public void Enter(PlayerContext ctx, IPlayerState _fromState)
    {
        invalid = !ctx.Data.TryConsumeEnergy();
        if (invalid) return;

        ctx.Animations.PlayJump();
        float startdir = Mathf.Sign(ctx.Data.velocity.angle);
        if (ctx.Inputs.MoveMagnitude > 0.5f) startdir = -ctx.Inputs.Move;

        dashComposer.Init(startdir);

        ctx.VFX.PlayOrbitalDashPS(Vector3.forward);
        return;
    }

    public void ResetInternal()
    {
        return;
    }

    public void Exit(PlayerContext ctx)
    {
        dashComposer.ForceDone();
        return;
    }
}
