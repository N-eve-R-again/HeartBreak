using UnityEngine;

public class PlayerStateIdle : IPlayerState
{

    public void Enter(PlayerContext ctx, IPlayerState _fromState)
    {
        ctx.Data.facing = Vector2.up;
        return;
    }

    public Transition Execute(PlayerContext ctx)
    {

        if (ctx.Inputs.Back.Down) return this.GoTo<PlayerStateBackHop>();

        if (ctx.Inputs.Forward.Down) return this.GoTo<PlayerStateForwardHop>();

        if (ctx.Inputs.Attack.Down) return this.GoTo<PlayerStateForwardDashCharge>();

        if (ctx.Inputs.Dash.Down) return this.GoTo<PlayerStateOrbitalDash>();

        ctx.Data.velocity = PolarCoordinate.Lerp(ctx.Data.velocity, PolarCoordinate.zero, ctx.Settings.idleDecel * Time.deltaTime);
        if (ctx.Data.velocity.magnitude < 0.1f) ctx.Data.velocity = PolarCoordinate.zero;
        ctx.Data.position += ctx.Data.velocity * Time.deltaTime;


        if (ctx.Inputs.MoveMagnitude < 0.1f)
        {
            return this.Stay();
        }
        else
        {
            return this.GoTo<PlayerStateMove>();
        }
    }

    public void Exit(PlayerContext ctx)
    {
        return;
    }

    public void ResetInternal()
    {
        return;
    }
}
