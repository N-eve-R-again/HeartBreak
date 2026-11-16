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

    public float Step(float angVel, float moveInput, float dt, float polardistance)
    {
        float mult = ArenaManager.DistanceToMult(polardistance);
        bool pressing = Mathf.Abs(moveInput) > 0.01f;
        float target = pressing ? -moveInput * maxSpeed * mult : 0f;
        float rate = pressing
            ? (Mathf.Abs(angVel) < Mathf.Abs(target) ? accel : decel)
            : neutralDecel;
        float steered = Mathf.Lerp(angVel, target, rate * dt);
        return angVel = Mathf.Lerp(angVel, steered, blend);
    }
}

public class OrbitalDriftComposer
{
    private float friction;
    private float inputInfluence;
    private float driftLean;
    private AnimationCurve jumpCurve;
    private float jumpDuration;
    private float blend;
    private float minSpeed = 5f;

    public OrbitalDriftComposer(float _friction, float _inputInfluence, float _driftLean,
                                AnimationCurve _jumpCurve, float _jumpDuration, float _blend)
    {
        friction = _friction;
        inputInfluence = _inputInfluence;
        driftLean = _driftLean;
        jumpCurve = _jumpCurve;
        jumpDuration = _jumpDuration;
        blend = _blend;
    }

    public void UpdateBlend(float newBlend) => blend = newBlend;

    public bool IsDrifting(float angVel, float moveInput)
    {
        if (Mathf.Abs(moveInput) <= 0.01f) return false;
        return Mathf.Sign(moveInput) == Mathf.Sign(angVel);
    }

    public Vector2 GetFacing(float angVel, float moveInput, float blend = 1f)
    {
        Vector2 side = angVel < 0f ? Vector2.left : Vector2.right;
        float lean = IsDrifting(angVel, moveInput) ? driftLean : 0f;
        return Vector2.Lerp(Vector2.up, Vector2.up + side * lean, blend);
    }

    public float GetJumpHeight(float time)
    {
        if (jumpDuration <= 0f) return 0f;
        return jumpCurve.Evaluate(time / jumpDuration);
    }

    public float Step(float angleVelocity, float moveInput, float dt)
    {
        float appliedFriction = Mathf.Abs(moveInput) > 0 ? friction * 5f : 1f;
        float steered = Mathf.Lerp(angleVelocity, minSpeed * -moveInput, appliedFriction * dt);
        if (IsDrifting(steered, moveInput))
            steered = Mathf.Lerp(steered, 0f, inputInfluence * dt);

        return Mathf.Lerp(angleVelocity, steered, blend);
    }

}