using UnityEngine;

public class PlayerStateOrbitalDash : IPlayerState
{
    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();
    private PlayerVFXLibrary vfx => PlayerStateRegistry.GetVfxLibrary();

    float dir;

    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {
        float ratio = _currentData.time / settings.orbitalDashDuration;


        _currentData.velocity.angle  = settings.orbitalDashForceCurve.Evaluate(ratio) * settings.orbitalDashForce * dir;
        _currentData.position += _currentData.velocity * Time.deltaTime;


        if (ratio > 1f) { return this.GoTo<PlayerStateMove>(); }
        return this.Stay();
    }

    public void Exit(ref PlayerEntityData _data)
    {
        return;
    }

    public void Enter(ref PlayerEntityData _data, IPlayerState _fromState)
    {
        dir = Mathf.Sign(_data.velocity.angle);
        vfx.PlayOrbitalDashPS(Vector3.forward);
        return;
    }
}
