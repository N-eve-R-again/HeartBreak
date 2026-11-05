using UnityEditor;
using UnityEngine;

public class PlayerStateMove : IPlayerState
{

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

        if (Mathf.Abs(ctx.Inputs.Move) > 0)
        {
            // Velocity directe basée sur inputd
            targetVelocity.angle = -ctx.Inputs.Move * (ctx.Settings.moveangleMaxSpeed);// / settings.ringToDistance.Evaluate(_currentData.position.distance));
            targetVelocity.distance = 0; // -_inputs.Move.y * settings.movedistMaxSpeed;

            //_currentData.facing = new Vector2(MoveInputCameraSpace.x, MoveInputCameraSpace.z);
        }

        // Un seul lerp : velocity vers target
        float lerpSpeed = ctx.Data.velocity.magnitude < targetVelocity.magnitude ? ctx.Settings.moveAccel : ctx.Settings.moveDecel;
        ctx.Data.velocity = PolarCoordinate.Lerp(ctx.Data.velocity, targetVelocity, lerpSpeed * Time.deltaTime);

        // Appliquer
        ctx.Data.position += ctx.Data.velocity * Time.deltaTime;
        ctx.Animations.UpdateMove(Mathf.Abs(ctx.Inputs.MoveMagnitude));
        // Transition vers Idle
        if (-ctx.Inputs.MoveMagnitude <= 0.1f && ctx.Data.velocity.magnitude < 0.1f)
        {
            ctx.Data.velocity = PolarCoordinate.zero;
            ctx.Data.facing = Vector2.up;
            ctx.Animations.UpdateMove(0);
            return this.GoTo<PlayerStateIdle>();
        }

        ctx.Data.facing = Mathf.Abs(ctx.Inputs.Move) >= 0.1f ? new Vector2(ctx.Inputs.Move, 0f) : Vector2.up;

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

