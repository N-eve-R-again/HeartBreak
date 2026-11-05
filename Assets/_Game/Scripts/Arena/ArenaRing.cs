using System;
using Unity.Mathematics;
using UnityEngine;

public class ArenaRing : MonoBehaviour
{
    [SerializeField] private ArenaSpace[] Spaces;
    [SerializeField] private RingTransition GoToInnerTransition;
    [SerializeField] private RingTransition GoToOuterTransition;

    public Vector3 GetSpacePos(int _index)
    {
        return Spaces[_index].GetCenter();
    }

    public Vector3 GetSpaceAxis(int _index, int2 _dir)
    {
        return Spaces[_index].GetCaseAxis(_dir);
    }

    public void SetSpaceDanger(int _index, bool _newState) 
    {
        Spaces[_index].SetWarningObjectState(_newState);
    }
    public void TriggerTelegraph(int _index)
    {
        Spaces[_index].TriggerTelegraph();
    }

    public int GetRingMax()
    {
        return Spaces.Length-1;
    }

    public int ClampX(int _xToClamp)
    {
        if(_xToClamp > GetRingMax()) _xToClamp = 0;
        if (_xToClamp < 0) _xToClamp = GetRingMax();

        return Mathf.Clamp(_xToClamp, 0, GetRingMax());
    }
    
    public bool IsSpaceAvailable(int _index)
    {
        return Spaces[_index].GetAvailability();
    }

    public RingTransition GetTransition(bool _goToInner)
    {
        return _goToInner ? GoToInnerTransition : GoToOuterTransition;
    }
}

[Serializable]
public class RingTransition
{
    [SerializeField] private float multiplyFactor = 1;
    [SerializeField] private bool isComplex;
    [SerializeField] private int offset;
    [SerializeField] private float transitionMask = 1;
    public bool SolveValidMove(int _currentX)
    {

        if ((transitionMask == 0))
        {
            Debug.Log($"Failed mask test: {transitionMask} is 0");
            return false;
        }
        if (_currentX % transitionMask != 0f)
        {
            Debug.Log($"Failed mask test: {_currentX} % {transitionMask} != 0");
            return false;
        }

        return true;
    }

    public bool IsComplex()
    {
        return isComplex;
    }

    public int SolveNewX(int _inXpos)
    {
        int outXpos = _inXpos;

        float temp = outXpos * multiplyFactor + offset;
        outXpos = Mathf.FloorToInt(temp);
        return outXpos;
    }

}
