using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControllerSettings", menuName = "HeartBreak/PlayerControllerSettings")]
public class PlayerControllerSettings : ScriptableObject
{
    [Header("Energy")]
    public float maxCharges;
    public float timeToCharge;


    [Header("Idle")]
    public float idleDecel;
    public float idleResetFacingDelay;

    [Header("Move")]
    public float moveangleMaxSpeed;
    public float movedistMaxSpeed;
    public float moveAccel;
    public float moveDecel;

    [Header("BackwardHop")]
    public float BackwardHopSpeed;
    public float BackwardHopSmallJumpAmplitude = 0.5f;

    public float backwardInterruptRatio;

    public AnimationCurve backwardHopEase;
    public AnimationCurve backwardHopJumpCurve;

    [Header("Hops")]
    public float hopInputBufferRatio = 0.5f;
    public AnimationCurve hopToMoveEasingCurve;
    public AnimationCurve facingEasingCurve;

    [Header("Forward Hop")]
    public float ForwardHopSpeed;
    public float forwardHopInteruptRatio;
    public AnimationCurve forwardHopEase;


    [Header("Charge Dash")]
    public AnimationCurve chargeDashJump;
    public float chargeDashJumpDuration;
    public float chargeDashChargeTime;
    public float chargeDashFriction;
    public float chargeDashInputInfluence;
    public float chargeDashBoostMin = 8f;   // boost si on part à l'arrêt
    public float chargeDashBoostMax = 15f;  // boost si on part à vitesse max

    [Header("Orbital Dash")]
    public float orbitalDashDuration;
    public float orbitalDashImmunityRatio;
    public AnimationCurve orbitalDashForceCurve;
    public float orbitalDashForce;


    [Header("Attack Dash")]
    public AnimationCurve attackDashCurve;
    public float attackDashDuration;
    public float afterAttackPauseTime;
    public float meleeDamage;


    [Header("Forward Dash")]
    public float forwardDashSpeedByChargeIncr;
    public float forwardDashSpeedByChargeBase;
    public AnimationCurve forwardDashEase;
    public AnimationCurve forwardDashAngularRampUp;



}
