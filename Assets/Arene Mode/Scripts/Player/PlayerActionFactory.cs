using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class PlayerActionFactory
{
    public static object CreateMove(int2 _direction, int2 _origin)
    {

        Move move = new Move(_direction, _origin);
        move.Calculate();
        if (!ArenaCoordinateSystem.instance.IsMoveComplex(_origin.y, _direction.y))
        {
            return move.IsValid() ? move : null;
        }

        List<PlayerAction> validMoves = new List<PlayerAction>();

        Move option1 = new Move(_direction, _origin);
        option1.Calculate();
        Move option2 = new Move(new int2(_direction.x + 1, _direction.y), _origin);
        option2.Calculate();

        if (option1.IsValid()) validMoves.Add(option1);
        if (option2.IsValid()) validMoves.Add(option2);

        if (validMoves.Count == 0) return null;

        return validMoves.Count == 1 ? validMoves[0] : new MoveChoice(validMoves);
    }

}
