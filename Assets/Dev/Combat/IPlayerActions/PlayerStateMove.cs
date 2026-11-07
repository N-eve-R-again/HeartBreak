using UnityEditor;
using UnityEngine;

public class PlayerStateMove : IPlayerState
{

    private OrbitalMoveComposer moveComposer;

    public void Init(PlayerContext ctx)
    {
        moveComposer = new OrbitalMoveComposer(ctx.Settings.moveangleMaxSpeed, ctx.Settings.moveAccel, ctx.Settings.moveDecel, ctx.Settings.moveDecel, 1f);
    }


    public void Enter(PlayerContext ctx, IPlayerState _fromState)
    {
        ctx.Animations.ReturnToBase();
        return;
    }
 
    public Transition Execute(PlayerContext ctx)
    {
        PolarCoordinate targetVelocity = PolarCoordinate.zero;

        if (ctx.Inputs.Back.Down) return this.GoTo<PlayerStateBackHop>();

        if (ctx.Inputs.Forward.Down) return this.GoTo<PlayerStateForwardHop>();

        if (ctx.Inputs.Attack.Down) return this.GoTo<PlayerStateForwardDashCharge>();

        if (ctx.Inputs.Dash.Down) return this.GoTo<PlayerStateOrbitalDash>();

        ctx.Data.velocity.angle = moveComposer.Step(ctx.Data.velocity.angle, ctx.Inputs.Move, Time.deltaTime);

        // Appliquer
        ctx.Data.position += ctx.Data.velocity * Time.deltaTime;
        ctx.Animations.UpdateMove(Mathf.Abs(ctx.Inputs.MoveMagnitude));
        // Transition vers Idle
        if (ctx.Inputs.MoveMagnitude <= 0.1f && ctx.Data.velocity.magnitude < 0.1f)
        {
            ctx.Data.velocity = PolarCoordinate.zero;
            ctx.Data.facing = Vector2.up;
            ctx.Animations.UpdateMove(0);
            return this.GoTo<PlayerStateIdle>();
        }

        ctx.Data.facing = moveComposer.GetFacing(ctx.Inputs.Move);

        return this.Stay();
        
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

