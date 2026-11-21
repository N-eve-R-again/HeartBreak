using UnityEngine;

public class PlayerStateHurt : IPlayerState
{
   
    public void Enter(PlayerContext ctx, IPlayerState fromState)
    {
        ctx.Data.iFrameTimer = ctx.Settings.hitIFrames;
        ctx.Data.velocity = PolarCoordinate.back;
        ctx.VFX.PlayHit();
        //animation
    }

    public Transition Execute(PlayerContext ctx)
    {
        if(ctx.Data.time > 0.05f) ctx.VFX.ResetTimescale();
        if (ctx.Data.time > ctx.Settings.stunDuration) return this.Done();

        ctx.Data.position.distance = Mathf.Round(ctx.Data.position.distance);

        return this.Stay();
    }

    public void Exit(PlayerContext ctx)
    {
        ctx.VFX.ResetMat();
        ctx.Data.velocity = PolarCoordinate.zero;
    }

    public void ResetInternal()
    {

    }
}
