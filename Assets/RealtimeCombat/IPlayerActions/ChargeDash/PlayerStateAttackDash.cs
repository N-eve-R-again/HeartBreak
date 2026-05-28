using UnityEngine;

public class PlayerStateAttackDash : IPlayerState
{
    public float ringfrom;

    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();

    public IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs)
    {
        _currentData.time += Time.deltaTime;
        float ratio = _currentData.time / settings.attackDashDuration;
        _currentData.position.distance = Mathf.Lerp(ringfrom, 0, settings.attackDashCurve.Evaluate(ratio));

        if (ratio > 1f) {
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
        _data.time = 0f;
        ringfrom = _data.position.distance;
        _data.velocity = PolarCoordinate.zero;
        return;
    }
}
