using UnityEngine;

public class PlanificationPlayerPreview : MonoBehaviour
{
    [SerializeField] private Transform currentPosTransform;
    [SerializeField] private Vector3 targetPos;
    [SerializeField] private float lerpSpeed = 15f;

    [SerializeField] private ActionPreview[] previews = new ActionPreview[4];
    public TrailRenderer trail;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void ResetPreview(Vector3 _newPos)
    {

        
        for (int i = 0; i < previews.Length; i++)
        {
            previews[i].SetVisual(ActionPreview.VisualState.Hide,0f);
        }
        trail.Clear();
        Teleport(_newPos);
    }
    public void AddActionPreview(int index, PlayerAction _action)
    {
        ActionPreview actionPreview = previews[index];
        if(_action is Move move)
        {
            MoveData data = move.GetData();
            actionPreview.SetPosition(data.origin);
            float angle = ArenaCoordinateSystem.instance.DirToAngle(data.origin, data.orientation);
            Debug.Log($"ACTION PREVIEW ANGLE {angle}");
            actionPreview.SetVisual(ActionPreview.VisualState.Move, angle);

        }
        if(_action is NoAction noAction)
        {

        }


    }
    private void Update()
    {
        currentPosTransform.position = Vector3.Slerp(currentPosTransform.position, targetPos, Time.deltaTime * lerpSpeed);
    }

    private void RebuildLine()
    {

    }

    // Update is called once per frame
    public void UpdateTargetPos(Vector3 _newPos)
    {
        targetPos = _newPos;
    }

    public void Teleport(Vector3 _newPos)
    {
        currentPosTransform.position = _newPos;
    }
}

