using UnityEngine;

public class PlayerStateForwardDashExecute : IPlayerState
{

    private float ringfrom, ringto;

    private float timeToExecute;
    private float originalangularVel;

    private Vector2 originalfacing;

    public void Enter(PlayerContext ctx, IPlayerState fromState)
    {
        originalfacing = ctx.Data.facing;
        timeToExecute = Mathf.Abs(ctx.Settings.forwardDashSpeedByChargeIncr * ctx.Data.velocity.distance) + ctx.Settings.forwardDashSpeedByChargeBase;
        ringfrom = ctx.Data.position.distance;
        ringto = ctx.Data.position.distance + ctx.Data.velocity.distance;
        ctx.Data.velocity.distance = 0f;
        originalangularVel = ctx.Data.velocity.angle;

        ctx.VFX.shooter.ShootPeel(ctx.Data.position);
    }

    public Transition Execute(PlayerContext ctx)
    {

        float ratio = ctx.Data.time / timeToExecute;
        ctx.Data.position.distance = Mathf.LerpUnclamped(ringfrom, ringto, ctx.Settings.forwardDashEase.Evaluate(ratio));
        //_currentData.velocity.angle = Mathf.Lerp(_currentData.velocity.angle, settings.moveangleMaxSpeed * -inputs.Move, settings.forwardDashAngularRampUp.Evaluate(ratio) * Time.deltaTime);
        ctx.Data.position.angle += ctx.Data.velocity.angle * Time.deltaTime;
        //_currentData.facing = Vector2.Lerp(originalfacing, new Vector2(-_currentData.velocity.angle, 1f).normalized, settings.forwardDashAngularRampUp.Evaluate(ratio));

        if(Mathf.Sign(ctx.Inputs.Move) != Mathf.Sign(ctx.Data.velocity.angle)){
            ctx.Data.velocity.angle = Mathf.Lerp(ctx.Data.velocity.angle, originalangularVel * 0.45f, ratio);
            Debug.Log("what ?");
        }

        if (ctx.Data.time > timeToExecute)
        {


            if (Mathf.Sign(ctx.Inputs.Move) != Mathf.Sign(ctx.Data.velocity.angle))
            {
                ctx.Data.velocity.angle = ctx.Settings.moveangleMaxSpeed * -ctx.Inputs.Move * 1.25f;
            }
            else
            {
                ctx.Data.velocity.angle = ctx.Settings.moveangleMaxSpeed * -ctx.Inputs.Move;
            }

            return this.Done();

        }

        return this.Stay();
    }

    public void Exit(PlayerContext ctx)
    {
        ctx.Data.position.distance = ringto;

    }

    public void ResetInternal()
    {
        return;
    }
}
