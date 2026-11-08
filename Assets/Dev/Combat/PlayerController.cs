using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
    private PlayerInputManager inputsManager;
    [SerializeField] private PlayerVFXLibrary vfxLibrary;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private PlayerAnimationManager animationManager;
    [SerializeField] private PlayerUI ui;

    // Entity data
    public string State;
    private PlayerContext playerContext;

    [SerializeField] private PlayerEntityData playerEntityData = new();
    [SerializeField] private PlayerControllerSettings settings;
    public Transform cameraPivot;

    public IPlayerState currentState;
    private MainMovementResolver mainMovementResolver = new();

    public static float GetRing(float distance)
    {
        return ArenaManager.DistanceToWS(distance);
    }

    private string currentStateName => currentState.ToString();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Application.targetFrameRate = 165; 
        InitializeInputs();
        InitVFX();
        InitVisual();
        InitAnimator();
        InitializeStateMachine();
    }
    private void InitializeInputs()
    {
        inputsManager = GetComponent<PlayerInputManager>();
        inputsManager.InitializeInputActions();
    }

    private void InitVFX()
    {
        vfxLibrary = GetComponent<PlayerVFXLibrary>();
    }

    private void InitAnimator()
    {
        animationManager = GetComponent<PlayerAnimationManager>();
    }

    private void InitVisual()
    {
        playerVisual = GetComponent<PlayerVisual>();
        playerVisual.Init(settings);
    }

    private void InitializeStateMachine()
    {
        var types = Assembly.GetAssembly(typeof(IPlayerState))
        .GetTypes()
        .Where(t => typeof(IPlayerState).IsAssignableFrom(t) && !t.IsAbstract);

        playerContext = new PlayerContext(playerEntityData, inputsManager.inputs, settings, vfxLibrary,animationManager);

        foreach (var type in types)
        {
            var instance = (IPlayerState)Activator.CreateInstance(type);
            PlayerStateRegistry.Register(instance);
            instance.Init(playerContext);
        }

        currentState = PlayerStateRegistry.Get(typeof(PlayerStateIdle));

    }

    // Update is called once per frame
    void Update()
    {
        State = currentStateName.Replace("PlayerState","");
        inputsManager.UpdateInputs();

        //PlayerInputData inputSnapshot = inputs.Clone();

        // 3. Exécuter l'action du state actuel
        var transition = currentState.Execute(playerContext);

        switch (transition.Type)
        {
            case TransitionType.Stay: 
                //we stay in the state.
                break;
            case TransitionType.Restart: 
                RestartState(); 
                break;
            case TransitionType.Goto: 
                TransitionToState(currentState,transition.Target);
                break;
            case TransitionType.Done: 
                Resume();
                break;
        }

        playerContext.Data.time += Time.deltaTime;

        UpdateEnergy();
        ui.UpdateDashSlots(playerEntityData.currentCharges);

    }

    private void UpdateEnergy()
    {
        if (playerEntityData.currentCharges >= settings.maxCharges) return;
        if(playerEntityData.recharge < settings.timeToCharge)
        {
            playerEntityData.recharge += Time.deltaTime;
            ui.UpdateFill(playerEntityData.recharge /  settings.timeToCharge);
            return;
        }
        playerEntityData.recharge = 0;
        playerEntityData.currentCharges ++;
        vfxLibrary.energygained.Play();
        Debug.Log("recharge");

    }


    public void LateUpdate()
    {

        playerVisual.UpdatePosition(playerContext.Data.position);
        playerVisual.UpdateFacing(playerContext.Data.facing);

        cameraPivot.transform.rotation = Quaternion.Euler(0, playerContext.Data.position.angle, 0);
    }
    private void RestartState()
    {
        currentState.Exit(playerContext);
        currentState.ResetInternal();
        currentState.Enter(playerContext, currentState);
    }

    private void Resume() =>
        TransitionToState(currentState, mainMovementResolver.Dispatch(playerContext).Target);


    private void TransitionToState(IPlayerState from, IPlayerState to)
    {

        currentState = to;
        from.Exit(playerContext);
        playerContext.Data.time = 0f;
        currentState.ResetInternal();
        currentState.Enter(playerContext, from);
    }

}

public class PlayerContext
{
    public PlayerEntityData Data;
    public PlayerInputData Inputs;
    public PlayerControllerSettings Settings;
    public PlayerVFXLibrary VFX;
    public PlayerAnimationManager Animations;

    public PlayerContext(
        PlayerEntityData data, 
        PlayerInputData inputs,
        PlayerControllerSettings settings,
        PlayerVFXLibrary vfx,
        PlayerAnimationManager animations)
    {
        Data = data;
        Inputs = inputs;
        Settings = settings;
        VFX = vfx;
        Animations = animations;
    }
}
