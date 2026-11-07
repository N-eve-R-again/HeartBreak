using UnityEngine;
using UnityEngine.UIElements;

public class OrbitalDashComposer
{
    private AnimationCurve curve;
    private float force;
    private float dir;
    private float blend;
    private float duration;
    private float timer;

    public bool Done;
    public bool Dashing => !Done;

    public OrbitalDashComposer(AnimationCurve _curve, float _force, float _dir, float _duration, float _blend)
    {
        curve = _curve;
        force = _force;
        dir = _dir;
        blend = _blend;
        duration = _duration;
        Done = true;
    }

    public void SetBlend(float _blend) { blend = _blend; }
    public void SetDir(float _dir) {
        _dir = Mathf.Clamp(_dir,-1f,1f);
        dir = _dir; }
    public float GetDir() => dir; 

    public Vector2 GetFacing(float blend = 1f)
    {
        return Vector2.Lerp(Vector2.up, Mathf.Abs(dir) >= 0.1f ? new Vector2(dir, 0f) : Vector2.up, blend);
    }

    public float Step(float dt)
    {
        if (Done) return 0;
        if (dir == 0) return 0;


        timer += dt;
        float ratio = Mathf.Clamp01(timer / duration);


        if (ratio >= 1f)
            Done = true;

        return curve.Evaluate(ratio) * force * dir;
    }

    public void Init(float _dir)
    {
        SetDir(_dir);
        timer = 0f;
        Done = false;
    }

    public void ForceDone()
    {
        SetDir(0f);
        timer = 0f;
        Done = true;
    }

    public float GetRatio() => Mathf.Clamp01(timer / duration);
}
