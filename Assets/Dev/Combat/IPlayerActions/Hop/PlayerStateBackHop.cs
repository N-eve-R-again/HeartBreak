using UnityEngine;

public class PlayerStateBackHop : IPlayerState
{

    private float ringfrom, ringto;

    private bool wantstofwdash = false;
    private bool wantstobwdash = false;
    private bool wantstoorbitdash = false;

    private OrbitalMoveComposer moveComposer;

    public void Init(PlayerContext ctx)
    {
        moveComposer = new OrbitalMoveComposer(ctx.Settings.moveangleMaxSpeed, ctx.Settings.moveAccel, ctx.Settings.moveDecel, ctx.Settings.moveDecel, 1f);
    }
    public Transition Execute(PlayerContext ctx)
    {
        if(ringfrom == ringto) return this.GoTo<PlayerStateMove>();



        float ratio = ctx.Data.time / ctx.Settings.BackwardHopSpeed;

        if (ratio > ctx.Settings.hopInputBufferRatio)
        {

            if (ctx.Inputs.Forward.Down)
            {
                wantstofwdash = true;
                wantstobwdash = false;
            }
            if (ctx.Inputs.Back.Down)
            {
                wantstobwdash = true;
                wantstofwdash = false;
            }

            if (ctx.Inputs.Dash.Down)
            {
                wantstoorbitdash = true;
            }

        }


        ctx.Data.position.distance = Mathf.LerpUnclamped(ringfrom,ringto, ctx.Settings.backwardHopEase.Evaluate(ratio));

        ctx.Data.position.y = ctx.Settings.backwardHopJumpCurve.Evaluate(ratio) * ctx.Settings.BackwardHopSmallJumpAmplitude;
        moveComposer.UpdateBlend(ctx.Settings.hopToMoveEasingCurve.Evaluate(ratio));
        ctx.Data.velocity.angle = moveComposer.Step(ctx.Data.velocity.angle, ctx.Inputs.Move, Time.deltaTime);
        ctx.Data.position += ctx.Data.velocity * Time.deltaTime;
        ctx.Data.facing = moveComposer.GetFacing(ctx.Inputs.Move, ctx.Settings.facingEasingCurve.Evaluate(ratio));

        if (ratio > ctx.Settings.backwardInterruptRatio)
        {
            if (wantstobwdash) return this.Restart();
            if (wantstofwdash) return this.GoTo<PlayerStateForwardHop>();
            if (wantstoorbitdash) return this.GoTo<PlayerStateOrbitalDash>();
            if (ctx.Inputs.Attack.Held) return this.GoTo<PlayerStateForwardDashCharge>();
        }


        if (ratio > 1f) return this.Done();


        return this.Stay();
    }

    public void Exit(PlayerContext ctx)
    {
        ctx.Data.position.distance = ringto;
        return;
    }

    public void Enter(PlayerContext ctx, IPlayerState _fromState)
    {
        ctx.Data.time = 0f;
        ringfrom = ctx.Data.position.distance;
        ringto = Mathf.RoundToInt(ctx.Data.position.distance + 1f);
        if(ringto >= ArenaManager.arena.ringRadii.Length) ringto = ringfrom;
        return;
    }

    public void ResetInternal()
    {
        wantstofwdash = false;
        wantstobwdash = false;
        wantstoorbitdash = false;
    }
}