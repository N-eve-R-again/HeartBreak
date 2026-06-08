using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour 
{
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction dashAction;
    private InputAction backAction;
    private InputAction forwardAction;

    public PlayerInputData inputs;

    public void UpdateInputs()
    {
        inputs.Move = moveAction.ReadValue<float>();// PlayerInputData.Get8DirectionInput(moveAction.ReadValue<Vector2>());
        inputs.Jump = inputs.Jump.Update(jumpAction.IsPressed(), Time.deltaTime);
        inputs.Back = inputs.Back.Update(backAction.IsPressed(), Time.deltaTime);
        inputs.Forward = inputs.Forward.Update(forwardAction.IsPressed(), Time.deltaTime);
        inputs.Attack = inputs.Attack.Update(attackAction.IsPressed(), Time.deltaTime);
        inputs.Dash = inputs.Dash.Update(dashAction.IsPressed(), Time.deltaTime);

    }

    public void InitializeInputActions()
    {
        var playerInput = GetComponent<PlayerInput>();
        // Récupérer les InputActions depuis ton Input Action Asset
        // Exemple avec PlayerInput component:
        moveAction = playerInput.actions["Move"];

        jumpAction = playerInput.actions["Jump"];
        backAction = playerInput.actions["Backward"];
        forwardAction = playerInput.actions["Forward"];
        attackAction = playerInput.actions["Attack"];
        dashAction = playerInput.actions["Dash"];

    }
}
