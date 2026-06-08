using System;
using UnityEngine;

[Serializable]
public class ButtonState
{
    [SerializeField] private bool wasPressed = false;
    [SerializeField] private bool isPressed = false;

    [SerializeField] private float heldTime = 0f;

    public ButtonState(bool wasPressed, bool isPressed,  float heldTime)
    {
        this.wasPressed = wasPressed;
        this.isPressed = isPressed;
        this.heldTime = heldTime;

    }

    public ButtonState Update(bool pressed, float deltaTime)
    {
        bool newWasPressed = isPressed;
        bool newIsPressed = pressed;
        float newHeldTime = pressed ? heldTime + deltaTime : 0f;

        return new ButtonState(newWasPressed, newIsPressed, newHeldTime);
    }

    public bool Down => isPressed && !wasPressed;
    public bool Up => !isPressed && wasPressed;
    public bool Held => isPressed;
    public float Duration => heldTime;
}

[Serializable]
public class PlayerInputData
{
    public float Move;
    public float MoveMagnitude => Math.Abs(Move);
    public ButtonState Back;
    public ButtonState Jump;

    public ButtonState Attack;
    public ButtonState Dash;
    public ButtonState Forward;

    public PlayerInputData Clone()
    {
        return new PlayerInputData
        {
            Move = this.Move,
            Jump = this.Jump,
            Attack = this.Attack,
            Dash = this.Dash,
            Back = this.Back
        };
    }

}
