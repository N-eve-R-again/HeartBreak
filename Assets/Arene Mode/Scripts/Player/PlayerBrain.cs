using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    [SerializeField] private bool actionReadyFlag;
    [SerializeField] private bool isThinking = false;

    public GameObject previs;
    [SerializeField] private float lerpSpeed = 3f;
    [SerializeField] private int2 imaginedPos;

    [Space]

    [SerializeField] private bool inChoice;
    [SerializeField] private Vector3 choiceWS;

    [Space]

    public PlayerEntity playerEntity;
    [SerializeReference] private ChoiceResolver choiceResolverCache;
    [SerializeReference] private PlanificationPlayerPreview playerPreview;
    [SerializeReference] public List<PlayerAction> plannedActions = new List<PlayerAction>(4);



    void Update()
    {
        UpdateVisual();

        if (!isThinking) return;

        if (inChoice)
        {

            ListeningForResolution();
        }
        else
        {

            PlanningMoveAction();
        }
        
    }

    private void UpdateVisual()
    {
        Vector3 previsPosWS = Vector3.zero;

        if (inChoice)
        {
            previsPosWS = choiceWS;
        }
        else
        {
            previsPosWS = ArenaCoordinateSystem.instance.GetPositionInWorld(imaginedPos);
        }
        playerPreview.UpdateTargetPos(previsPosWS);
    }

    public void StartThinking() => isThinking = true;
    public void StopThinking() => isThinking = false;
    
    public void SyncBrainWithEntity()
    {
        imaginedPos = playerEntity.pos;
        playerPreview.ResetPreview(ArenaCoordinateSystem.instance.GetPositionInWorld(imaginedPos));
    }

    public void ClearBrain()
    {
        inChoice = false;
        plannedActions.Clear();
        playerPreview.ResetPreview(ArenaCoordinateSystem.instance.GetPositionInWorld(imaginedPos));
    }

    public bool ReadyToCompleteTimeline()
    {
        return plannedActions.Count >= 4;
    }

    #region Action Ready Flag Functions

    public bool GetActionReadyFlag()
    {
        return actionReadyFlag;
    }
    private void SetActionReadyFlag()
    {
        actionReadyFlag = true;
    }
    public void ResetActionReadyFlag()
    {
        actionReadyFlag = false;
    }

    #endregion


    private void PlanningMoveAction()
    {
        int xDir = 0, yDir = 0;

        if (Input.GetKeyDown(KeyCode.W)) yDir = -1;
        if (Input.GetKeyDown(KeyCode.S)) yDir = 1;
        if (Input.GetKeyDown(KeyCode.A)) xDir = +1;
        if (Input.GetKeyDown(KeyCode.D)) xDir = -1;

        int2 input = new int2(xDir, yDir);

        if ((xDir != 0 || yDir != 0) && !ReadyToCompleteTimeline())
        {

            var temp = PlayerActionFactory.CreateMove(input, imaginedPos);

            if (temp == null) return;

            if (temp is MoveChoice choice)
            {
                BeginComplexAction(choice);
                return;
            }

            SetActionReadyFlag();

            if (temp is Move move) 
            {
                plannedActions.Add(move);
                imaginedPos = move.GetPreviewPos();
                playerPreview.AddActionPreview(plannedActions.Count - 1,move);
            }

        }
    }

    private void ListeningForResolution()
    {
        
        int2 input = new int2();
        if (Input.GetKeyDown(KeyCode.W)) input.y = -1;
        if (Input.GetKeyDown(KeyCode.S)) input.y = 1;
        if (Input.GetKeyDown(KeyCode.A)) input.x = +1;
        if (Input.GetKeyDown(KeyCode.D)) input.x = -1;

        if(input.x != 0 || input.y != 0)
        {

            choiceResolverCache.HandleInput(input, this);
        }
    }


    #region Choice Resolve Functions
    public void SetChoicePosition(Vector3 _position)
    {
        choiceWS = _position;
    }

    private void BeginComplexAction(ChoiceResolver choice)
    {
        choiceResolverCache = choice;
        inChoice = true;
        choice.ShowPreview(this);
    }

    public void CompleteComplexChoice()
    {
        PlayerAction action = choiceResolverCache.GetSelectedOption();

        
        imaginedPos = action.GetPreviewPos();
        
        plannedActions.Add(action);
        playerPreview.AddActionPreview(plannedActions.Count - 1, action);
        SetActionReadyFlag();
        choiceResolverCache = null;
        inChoice = false;
    }

    #endregion
}

