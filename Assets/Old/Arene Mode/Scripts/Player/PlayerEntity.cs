using Unity.Mathematics;
using UnityEngine;

public class PlayerEntity : MonoBehaviour
{
    public int2 pos;
    public void ExecuteMoveAction(MoveData _moveData)
    {

        pos = _moveData.GetFinalPos();
        //check for damage;
    }

    void Update()
    {

        transform.position = ArenaCoordinateSystem.instance.GetPositionInWorld(pos);
    }
}
