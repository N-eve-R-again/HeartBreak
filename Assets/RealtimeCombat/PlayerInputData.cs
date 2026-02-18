using System;
using UnityEngine;

[Serializable]
public class ButtonState
{
    [SerializeField] private bool wasPressed = false;
    [SerializeField] private bool isPressed = false;
    [SerializeField] private float heldTime = 0f;

    public ButtonState(bool wasPressed, bool isPressed, float heldTime)
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
    public Vector2 Move;
    public ButtonState Jump;
    public ButtonState Dodge;
    public ButtonState Block;
    public ButtonState Attack;
    public ButtonState Dash;

    public PlayerInputData Clone()
    {
        return new PlayerInputData
        {
            Move = this.Move,
            Jump = this.Jump,
            Dodge = this.Dodge,
            Block = this.Block,
            Attack = this.Attack,
            Dash = this.Dash
        };
    }

    public static Vector2 Get8DirectionInput(Vector2 rawInput)
    {
        // Deadzone au repos
        if (rawInput.magnitude < 0.3f) return Vector2.zero;

        float angle = Mathf.Atan2(rawInput.y, rawInput.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        // 8 secteurs de 45° chacun
        if (angle >= 337.5f || angle < 22.5f) return Vector2.right;
        if (angle >= 22.5f && angle < 67.5f) return new Vector2(1, 1).normalized;
        if (angle >= 67.5f && angle < 112.5f) return Vector2.up;
        if (angle >= 112.5f && angle < 157.5f) return new Vector2(-1, 1).normalized;
        if (angle >= 157.5f && angle < 202.5f) return Vector2.left;
        if (angle >= 202.5f && angle < 247.5f) return new Vector2(-1, -1).normalized;
        if (angle >= 247.5f && angle < 292.5f) return Vector2.down;
        return new Vector2(1, -1).normalized;
    }
}
