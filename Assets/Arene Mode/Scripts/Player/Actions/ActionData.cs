using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class MoveData
{
    [SerializeField] public int2 origin;
    [SerializeField] public int2 direction;
    public Vector3 orientation;


    [SerializeField] private bool isValid = false;
    [SerializeField] private int2 finalPos;

    public MoveData(int2 _origin, int2 _direction)
    {
        origin = _origin;
        direction = _direction;
    }


    public void Finalize(bool _isValid,int2 _targetPos)
    {
        isValid = _isValid;
        finalPos = _targetPos;
    }

    public bool IsValid() { return isValid; }

    public int2 GetFinalPos() { return finalPos; }


}
