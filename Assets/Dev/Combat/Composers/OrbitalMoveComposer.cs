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

public class OrbitalDriftComposer
{
    private float friction;
    private float inputInfluence;
    private float driftLean;
    private AnimationCurve jumpCurve;
    private float jumpDuration;
    private float blend;

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

    public PolarCoordinate Step(PolarCoordinate velocity, float moveInput, float dt)
    {
        PolarCoordinate steered = PolarCoordinate.Lerp(velocity, PolarCoordinate.zero, friction * dt);
        if (IsDrifting(steered.angle, moveInput))
            steered = PolarCoordinate.Lerp(steered, PolarCoordinate.zero, inputInfluence * dt);

        return PolarCoordinate.Lerp(velocity, steered, blend);
    }

}