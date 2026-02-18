using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerController;

public class PlayerController : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dodgeAction;
    private InputAction blockAction;
    private InputAction attackAction;
    private InputAction dashAction;



    public enum PlayerState
    {
        None,
        Idle,
        Move,
        Jump,
        Dash

    }

    // Input data (instance unique, mise à jour chaque frame)
    [SerializeField] private PlayerInputData inputs = new PlayerInputData();

    // Entity data
    [SerializeField] private PlayerEntityData entityData;
    [SerializeField] private PlayerControllerSettings settings;
    public Transform CameraPivot;
    public float angle = 0;
    // State machine
    [SerializeField] private PlayerState currentState;
    private Dictionary<PlayerState, IPlayerAction> actions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Initialiser les actions
        InitializeInputActions();
        InitializeStateMachine();

        // Initialiser entity data
        entityData = new PlayerEntityData();
        currentState = PlayerState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputs();

        //PlayerInputData inputSnapshot = inputs.Clone();

        // 3. Exécuter l'action du state actuel
        actions[currentState].Execute(ref entityData, inputs);

        // 4. Appliquer les changements d'entity data


        // 5. Gérer les transitions de state
        if (entityData.nextState != currentState)
        {
            TransitionToState(entityData.nextState);
        }



        transform.position = Vector3.back * entityData.position.distance;
        transform.position = Quaternion.Euler(new Vector3(0f, entityData.position.angle, 0f)) * transform.position;
        transform.position = transform.position + Vector3.up * entityData.position.y;


        Vector2 bossToPlayer = Vector2.zero;// Vector2.zero - new Vector2(entityData.position.x, entityData.position.z);
        Vector2 fwd = new Vector2(0, 1f);
        float newangle = entityData.position.angle;// Vector2.SignedAngle(bossToPlayer.normalized, fwd);
        float dist = Mathf.Abs(angle - newangle);
        if(dist < 2f) dist = 0f;
        float speed = (dist > 15f) ? 2f : 0.1f;
        if (Mathf.Abs(angle - newangle) > 10f)
        {
            
            
        }
        angle = Mathf.LerpAngle(angle, newangle, Time.deltaTime * speed * dist);
        Quaternion newRotation = Quaternion.Euler(0, angle, 0);
        CameraPivot.transform.rotation = Quaternion.Lerp(CameraPivot.rotation, newRotation, Time.deltaTime * 10f);
    }
    private void TransitionToState(PlayerState newState)
    {
        // Cleanup du state précédent si nécessaire
        actions[currentState].Exit(ref entityData);
        actions[currentState].Enter(ref entityData,currentState);
        // Changer de state
        currentState = newState;
    }

    private void UpdateInputs()
    {
        inputs.Move = moveAction.ReadValue<Vector2>();// PlayerInputData.Get8DirectionInput(moveAction.ReadValue<Vector2>());
        inputs.Jump = inputs.Jump.Update(jumpAction.IsPressed(), Time.deltaTime);
        /*inputs.Dodge = inputs.Dodge.Update(dodgeAction.IsPressed(), Time.deltaTime);
        inputs.Block = inputs.Block.Update(blockAction.IsPressed(), Time.deltaTime);
        inputs.Attack = inputs.Attack.Update(attackAction.IsPressed(), Time.deltaTime);
        inputs.Dash = inputs.Dash.Update(dashAction.IsPressed(), Time.deltaTime);*/
    }

    private void InitializeInputActions()
    {
        // Récupérer les InputActions depuis ton Input Action Asset
        // Exemple avec PlayerInput component:
        var playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        
        jumpAction = playerInput.actions["Jump"];
        /*dodgeAction = playerInput.actions["Dodge"];
        blockAction = playerInput.actions["Block"];
        attackAction = playerInput.actions["Attack"];
        dashAction = playerInput.actions["Dash"];*/
    }

    private void InitializeStateMachine()
    {
        actions = new Dictionary<PlayerState, IPlayerAction>
        {
            { PlayerState.Idle, new PlayerActionIdle(settings) },
            { PlayerState.Move, new PlayerActionMove(settings) },
            { PlayerState.Jump, new PlayerActionNone(settings) },
            { PlayerState.Dash, new PlayerActionNone(settings) },
        };

    }

}


public interface IPlayerAction
{

    void Enter(ref PlayerEntityData _data, PlayerState _fromState);

    void Execute(ref PlayerEntityData _currentData,PlayerInputData _inputs);

    void Exit(ref PlayerEntityData _data);

}