using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControllerSettings", menuName = "HeartBreak/PlayerControllerSettings")]
public class PlayerControllerSettings : ScriptableObject
{
    [Header("Arena")]
    public float[] ringRadii;
    public AnimationCurve ringToDistance;

    void OnValidate()
    {
        if (ringRadii == null || ringRadii.Length == 0) return;

        Keyframe[] keys = new Keyframe[ringRadii.Length];
        for (int i = 0; i < ringRadii.Length; i++)
        {
            keys[i] = new Keyframe(i, ringRadii[i]);
        }
        ringToDistance = new AnimationCurve(keys);

        // Force des tangentes linéaires pour passer pile par chaque point
        for (int i = 0; i < keys.Length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(ringToDistance, i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(ringToDistance, i, AnimationUtility.TangentMode.Linear);
        }
    }

    [Header("Idle")]
    public float idleDecel;
    public float idleResetFacingDelay;

    [Header("Move")]
    public float moveangleMaxSpeed;
    public float movedistMaxSpeed;
    public float moveAccel;
    public float moveDecel;

    [Header("Backward Hop")]
    public float BackwardHopSpeed;
    public float BackwardHopSmallJumpAmplitude = 0.5f;
    public AnimationCurve backwardHopEase;
    public AnimationCurve backwardHopJumpCurve;

    [Header("Forward Dash")]
    public float forwardDashChargeTime;
    public float forwardDashSpeedByChargeIncr;
    public float forwardDashSpeedByChargeBase;
    public AnimationCurve forwardDashEase;
    public AnimationCurve forwardDashAngularRampUp;
    public AnimationCurve smalljump;
    public float smalljumptime;
}
