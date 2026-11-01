using UnityEngine;
using UnityEngine.UIElements;

public class PlayerStateOrbitalDash : IPlayerState
{
    float dir;

    public Transition Execute(PlayerContext ctx)
    {


        float ratio = ctx.Data.time / ctx.Settings.orbitalDashDuration;
        if (ratio < 0.15f)
        {
            if(dir == ctx.Inputs.Move)
            {
                dir = -dir;
                Debug.Log("inverted dash ");
            }
        }

        ctx.Data.velocity.angle  = ctx.Settings.orbitalDashForceCurve.Evaluate(ratio) * ctx.Settings.orbitalDashForce * dir;
        ctx.Data.position += ctx.Data.velocity * Time.deltaTime;


        if (ratio > 1f) {
            return this.GoTo<PlayerStateMove>(); }
        return this.Stay();
    }


    public void Enter(PlayerContext ctx, IPlayerState _fromState)
    {
        ctx.Animations.PlayJump();
        dir = Mathf.Sign(ctx.Data.velocity.angle);
        if(ctx.Inputs.MoveMagnitude > 0.5f) dir = -ctx.Inputs.Move;

        ctx.VFX.PlayOrbitalDashPS(Vector3.forward);
        return;
    }

    public void ResetInternal()
    {
        return;
    }

    public void Exit(PlayerContext ctx)
    {
        return;
    }
}
