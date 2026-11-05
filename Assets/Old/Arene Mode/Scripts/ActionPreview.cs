using Unity.Mathematics;
using UnityEngine;

public class ActionPreview : MonoBehaviour
{
    public enum VisualState
    {
        Hide,
        Pass,
        Move,
        Block,
        Attack
    }
    public VisualState state;

    public GameObject[] visuals;
    public float angle;

    public Vector3 GetTransformPosition()
    {
        return transform.position;
    }
    public bool IsActive()
    {
        return state != VisualState.Hide;
    }
    public void SetVisual(VisualState _visualState, float angle)
    {
        state = _visualState;
        
        UpdateVisual(angle);
    }

    public void SetPosition(int2 _pos)
    {
        transform.position = ArenaCoordinateSystem.instance.GetPositionInWorld(_pos);
    }

    private void UpdateVisual(float angle = 0f)
    {

        for (int i = 0; i < visuals.Length; i++)
        {
            visuals[i].SetActive(((int)state - 1) == i);
            
        }
        if (state != VisualState.Hide)
        {
            visuals[(int)state - 1].transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

}