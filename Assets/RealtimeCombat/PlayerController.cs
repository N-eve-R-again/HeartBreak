using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    private PlayerInputManager inputsManager;
    private PlayerVFXLibrary vfxLibrary;
    // Entity data
    [SerializeField] private PlayerEntityData entityData;
    [SerializeField] private PlayerControllerSettings settings;
    public Transform cameraPivot;
    public Transform visualPivot;
    public float angle = 0;

    public IPlayerState currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Application.targetFrameRate = 165;
        InitializeSettings();
        InitializeInputs();
        InitializeStateMachine();
        InitVFX();
    }
    private void InitializeSettings()
    {
        PlayerStateRegistry.SetSettings(settings);
    }
    private void InitializeInputs()
    {
        inputsManager = GetComponent<PlayerInputManager>();
        inputsManager.InitializeInputActions();
    }

    private void InitVFX()
    {
        vfxLibrary = GetComponent<PlayerVFXLibrary>();
        PlayerStateRegistry.SetLibrary(vfxLibrary);
    }

    private void InitializeStateMachine()
    {
        var types = Assembly.GetAssembly(typeof(IPlayerState))
        .GetTypes()
        .Where(t => typeof(IPlayerState).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var type in types)
        {
            var instance = (IPlayerState)Activator.CreateInstance(type);
            PlayerStateRegistry.Register(instance);
        }

        currentState = PlayerStateRegistry.Get(typeof(PlayerStateIdle));
    }

    // Update is called once per frame
    void Update()
    {
        inputsManager.UpdateInputs();

        //PlayerInputData inputSnapshot = inputs.Clone();

        // 3. Exécuter l'action du state actuel
        var previous = currentState;
        currentState = currentState.Execute(ref entityData, inputsManager.inputs);
        entityData.time += Time.deltaTime;

        // 5. Gérer les transitions de state
        if (previous != currentState) TransitionToState(previous);


        float realDist = settings.ringToDistance.Evaluate(entityData.position.distance);

        transform.position = Vector3.back * realDist;
        transform.position = Quaternion.Euler(new Vector3(0f, entityData.position.angle, 0f)) * transform.position;
        transform.position = transform.position + Vector3.up * entityData.position.y;


        Vector2 bossToPlayer = Vector2.zero;// Vector2.zero - new Vector2(entityData.position.x, entityData.position.z);
        Vector2 fwd = new Vector2(0, 1f);
        float newangle = entityData.position.angle;// Vector2.SignedAngle(bossToPlayer.normalized, fwd);
        float dist = Mathf.Abs(angle - newangle);
        if(dist < 2f) dist = 0f;
        float speed = (dist > 15f) ? 2f : 0.1f;

        angle = newangle;
        //angle = Mathf.LerpAngle(angle, newangle, Time.deltaTime * speed * dist);
        Quaternion newRotation = Quaternion.Euler(0, angle, 0);
        //CameraPivot.transform.rotation = Quaternion.Lerp(CameraPivot.rotation, newRotation, Time.deltaTime * 10f);
        cameraPivot.transform.rotation = newRotation;

        Vector3 radial = -transform.position;
        radial.y = 0; radial.Normalize();
        Vector3 tangent = Vector3.Cross(Vector3.up, radial);

        Vector3 worldDir = tangent * entityData.facing.x + radial * entityData.facing.y;

        visualPivot.transform.rotation = Quaternion.Lerp(visualPivot.transform.rotation, Quaternion.LookRotation(worldDir, Vector3.up), Time.deltaTime *8f);
    }
    private void TransitionToState(IPlayerState previous)
    {
        previous.Exit(ref entityData);
        entityData.time = 0f;
        currentState.Enter(ref entityData, previous);
    }

    


}


