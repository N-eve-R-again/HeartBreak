using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControllerSettings", menuName = "HeartBreak/PlayerControllerSettings")]
public class PlayerControllerSettings : ScriptableObject
{
    [Header("Arena")]
    public float minDist = 1.0f;
    public float maxDist = 5.0f;

    [Header("Idle")]
    public float idleDecel;
    public float idleResetFacingDelay;
    [Header("Move")]
    public float moveangleMaxSpeed;
    public float movedistMaxSpeed;
    public float moveAccel;
    public float moveDecel;


}
