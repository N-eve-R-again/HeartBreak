using UnityEngine;

public class PlayerStateAttackDash : IPlayerState
{
    public float ringfrom;

    public bool temp = false;

    public Transition Execute(PlayerContext ctx)
    {
        float ratio = ctx.Data.time / ctx.Settings.attackDashDuration;
        ctx.Data.position.distance = Mathf.Lerp(ringfrom, 0, ctx.Settings.attackDashCurve.Evaluate(ratio));

        if(ratio > 1 && !temp)
        {
            temp = true;
            BossManager.instance.Damage(ctx.Settings.meleeDamage, true, ctx.Data.position.angle);
        }

        if (ratio > 1.1f) {

            return this.GoTo<PlayerStateBackHop>();
        
        
        }


        return this.Stay();
    }

    public void Exit(PlayerContext ctx)
    {
        return;
    }

    public void Enter(PlayerContext ctx, IPlayerState _fromState)
    {
        //vfx.ChargeEffectKill();
        temp = false;
        ringfrom = ctx.Data.position.distance;
        ctx.VFX.meleeEffect.Play(ctx.Data.position);
        ctx.Data.velocity = PolarCoordinate.zero;
        return;
    }

    public void ResetInternal()
    {
        return;
    }
}
