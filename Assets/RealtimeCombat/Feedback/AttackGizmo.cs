using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class AttackGizmo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private PlayerControllerSettings settings => PlayerStateRegistry.GetSettings();

    public enum State
    {
        Hidden,
        DashCharge,
        Attack
    }

    public State state;

    public PolarCoordinate position;

    public float forcedAngle;
    public float easespeed;
    public PolarCoordinate targetPosition;
    public Vector3 worldpos;

    public float attackpreshot;
    public float gotopreshot;


    public Animator animator;

    public void UpdatePosition()
    {

        //lerp x

        position.angle = targetPosition.angle;
        position  = PolarCoordinate.Lerp(position, targetPosition, Time.deltaTime * easespeed);
        ApplyWorldPos();
        transform.position = worldpos;    }

    private void ApplyWorldPos()
    {
        float realDist = settings.ringToDistance.Evaluate(position.distance);
        worldpos = Vector3.back * realDist;
        worldpos = Quaternion.Euler(new Vector3(0f, position.angle, 0f)) * worldpos;
        worldpos = worldpos + Vector3.up * position.y;
        transform.position = worldpos;
    }

    public void Snap()
    {

        position = targetPosition;
        ApplyWorldPos();
    }

    public void Center()
    {
        worldpos = Vector3.up * 1.5f;
        transform.position = Vector3.Lerp(transform.position, worldpos, Time.deltaTime * easespeed* 2f);

    }

    public void Hide()
    {
        targetPosition = PolarCoordinate.zero;


        Snap();
        ApplyWorldPos();
    }

    void Start()
    {
        Hide();
    }

    public void Charge(PolarCoordinate snappos)
    {
        state = State.DashCharge;
        targetPosition = snappos;
        Snap();
        ApplyWorldPos();
        animator.SetTrigger("Activate");
        //animation
    }

    public void Attack()
    {
        if (state == State.Attack) return;
        state = State.Attack;
        animator.SetTrigger("Attack");
        Center();
        Snap();
        //animation

    }
    public void UpdatePlayerPose(PolarCoordinate snappos)
    {
        targetPosition = snappos;
    }

    public void MoveOneRing()
    {
        animator.SetTrigger("Move");
    }

    public void Validate()
    {
        animator.SetTrigger("Validate");
        state = State.Hidden;
    }


    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Hidden:

                break;
            case State.DashCharge:
                UpdatePosition();
                break;
            case State.Attack:
                Center();
                break;
        }


    }
}
