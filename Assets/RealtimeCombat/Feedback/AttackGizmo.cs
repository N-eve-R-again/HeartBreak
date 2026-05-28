using JetBrains.Annotations;
using UnityEngine;

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
    public float easespeed;
    public Vector3 targetPosition;

    public Animator animator;

    public void UpdatePosition()
    {
        CalculateWorldPos();


        //lerp x


        transform.position = Vector3.Slerp(transform.position, targetPosition, Time.deltaTime * easespeed);
    }

    private void CalculateWorldPos()
    {
        float realDist = settings.ringToDistance.Evaluate(position.distance);
        targetPosition = Vector3.back * realDist;
        targetPosition = Quaternion.Euler(new Vector3(0f, position.angle, 0f)) * targetPosition;
        targetPosition = targetPosition + Vector3.up * position.y;

    }

    public void Snap()
    {
        CalculateWorldPos();
        transform.position = targetPosition;

    }

    public void Center()
    {
        targetPosition = Vector3.up * 1.5f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * easespeed* 2f);
    }

    public void Hide()
    {
        targetPosition = Vector3.one * 100;
        Snap();
    }

    void Start()
    {
        Hide();
    }

    public void Goto(PolarCoordinate newpos)
    {
        position = newpos;
        animator.SetTrigger("Move");
        //animation
    }

    public void Charge(PolarCoordinate snappos)
    {
        state = State.DashCharge;
        position = snappos;
        Snap();
        animator.SetTrigger("Activate");
        //animation
    }

    public void Attack()
    {
        state = State.Attack;
        animator.SetTrigger("Attack");
        //animation

    }
    public void UpdatePlayerPose(PolarCoordinate snappos)
    {
        position = snappos;
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
