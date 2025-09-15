using System.Collections;
using UnityEngine;

public class PlanificationManager : MonoBehaviour
{
    enum PlanificationState
    {
        WaitingForPlayer,     // + Boss Preview
        ActionValidated,      // Après input joueur  
        BossTelegraph,        // Flash rouge intense

        TransitionToExecution,// UI/Camera change
        ExecutionPhase        // HandOff à ExecutionManager
    }

    [Header("References")]
    [SerializeField] private PlayerBrain playerBrain;
    [SerializeField] private BossBrain bossBrain;
    [SerializeField] private ExecutionManager executionManager; // À créer

    [Header("Timing")]
    [SerializeField] private float actionValidationDelay = 0.5f;
    [SerializeField] private float bossPreviewDelay = 1.0f;
    [SerializeField] private float bossTelegraphDelay = 0.8f;

    [Header("Debug")]
    [SerializeField] private PlanificationState currentState;
    [SerializeField] private int currentActionIndex = 0;

    private Coroutine currentPhaseCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPlanningPhase();
    }

    // Update is called once per frame
    void Update()
    {
        //Mettre les inputs
    }


    public void StartPlanningPhase()
    {
        Debug.Log("=== PLANNING PHASE START ===");
        currentState = PlanificationState.WaitingForPlayer;
        currentActionIndex = 0;

        //Boss thinks

        playerBrain.ClearBrain();
        playerBrain.SyncBrainWithEntity();


        if (currentPhaseCoroutine != null)
            StopCoroutine(currentPhaseCoroutine);

        currentPhaseCoroutine = StartCoroutine(PlanningLoopCR());

    }

    private IEnumerator PlanningLoopCR()
    {
        while (!IsTimelineComplete())
        {
            yield return new WaitForEndOfFrame();
            Debug.Log($"- START TURN {currentActionIndex}-");
            yield return new WaitForSeconds(bossPreviewDelay);
            currentState = PlanificationState.WaitingForPlayer;
            bossBrain.actions[currentActionIndex].Preview();
            Debug.Log("- BOSS IS PREVIEWING HIS ATTACK -");
            yield return new WaitForSeconds(bossPreviewDelay);
            //Show boss preview
            Debug.Log("- PLAYER IS THINKING -");
            playerBrain.StartThinking();

            yield return new WaitUntil(() => playerBrain.GetActionReadyFlag());

            playerBrain.StopThinking();

            currentState = PlanificationState.ActionValidated;
            Debug.Log($"Action {currentActionIndex} validated by player");


            playerBrain.ResetActionReadyFlag();

            yield return new WaitForSeconds(actionValidationDelay);

            Debug.Log("- BOSS IS IN TELEGRAPH -");
            bossBrain.actions[currentActionIndex].Telegraph();
            bossBrain.actions[currentActionIndex].EndPreview();
            currentState = PlanificationState.BossTelegraph;
            //call BossActionTelegraph

            yield return new WaitForSeconds(bossTelegraphDelay);

            currentActionIndex++;
        }

    }

    private bool IsTimelineComplete()
    {
        return playerBrain.ReadyToCompleteTimeline();
    }
}