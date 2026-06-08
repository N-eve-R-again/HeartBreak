using UnityEngine;

public class PlayerStateAttackDash : IPlayerState
{
    public float ringfrom;
    private PlayerVFXLibrary vfx => PlayerStateRegistry.GetVfxLibrary();

    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();

    public bool temp = false;

    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {
        float ratio = _currentData.time / settings.attackDashDuration;
        _currentData.position.distance = Mathf.Lerp(ringfrom, 0, settings.attackDashCurve.Evaluate(ratio));

        if(ratio > 1 && !temp)
        {
            temp = true;
            BossManager.instance.Damage(settings.meleeDamage, true, _currentData.position.angle);
        }

        if (ratio > 1.1f) {

            return this.GoTo<PlayerStateBackHop>();
        
        
        }


        return this.Stay();
    }

    public void Exit(ref PlayerEntityData _data)
    {
        return;
    }

    public void Enter(ref PlayerEntityData _data, IPlayerState _fromState)
    {
        //vfx.ChargeEffectKill();
        temp = false;
        ringfrom = _data.position.distance;
        vfx.meleeEffect.Play(_data.position);
        _data.velocity = PolarCoordinate.zero;
        return;
    }
}
