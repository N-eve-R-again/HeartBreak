using UnityEngine;

public class OrbitalMoveComposer
{
    private float maxSpeed;
    private float accel;
    private float decel;
    private float neutralDecel;
    private float blend;

    public OrbitalMoveComposer(float _maxSpeed, float _accel, float _decel, float _neutralDecel, float _blend)
    {
        maxSpeed = _maxSpeed;
        accel = _accel;
        decel = _decel;
        neutralDecel = _neutralDecel;
        blend = _blend;
    }

    public Vector2 GetFacing(float orbitalvel, float blend = 1f)
    {
        return Vector2.Lerp(Vector2.up,Mathf.Abs(orbitalvel) >= 0.1f ? new Vector2(orbitalvel, 0f) : Vector2.up,blend);
    }
    public void UpdateBlend(float newBlend) => blend = newBlend;

    public float Step(float angVel, float moveInput, float dt)
    {
        bool pressing = Mathf.Abs(moveInput) > 0.01f;
        float target = pressing ? -moveInput * maxSpeed : 0f;
        float rate = pressing
            ? (Mathf.Abs(angVel) < Mathf.Abs(target) ? accel : decel)
            : neutralDecel;
        float steered = Mathf.Lerp(angVel, target, rate * dt);
        return angVel = Mathf.Lerp(angVel, steered, blend);
    }
}
