using System;
using UnityEngine;

[Serializable]
public class PlayerEntityData
{
    public PlayerController.PlayerState nextState;

    public PolarCoordinate position;
    public PolarCoordinate velocity;
    public Vector2 direction;
    public Vector2 facing;

    public float speed;
    public float ySpeed;

    public float time;

    public static PolarCoordinate Solve(PolarCoordinate coordinate, float minDist, float maxDist)
    {
        if (coordinate.angle < 0f) { coordinate.angle += 360f; }
        if (coordinate.angle > 360f) { coordinate.angle -= 360f; }

        coordinate.distance = Mathf.Clamp(coordinate.distance, minDist, maxDist);

        if (coordinate.y < 0f) { coordinate.y = 0; }


        return coordinate;
    }


}

[System.Serializable]
public struct PolarCoordinate
{
    public float angle;
    public float distance;
    public float y;

    public static PolarCoordinate operator +(PolarCoordinate left, PolarCoordinate right)
    {
        return new PolarCoordinate(left.angle + right.angle, left.distance + right.distance, left.y + right.y);
    }

    public static PolarCoordinate operator *(PolarCoordinate polar, float multiplier)
    {
        return new PolarCoordinate(polar.angle * multiplier, polar.distance * multiplier, polar.y * multiplier);
    }

    public static PolarCoordinate Lerp(PolarCoordinate a, PolarCoordinate b, float time)
    {
        return new PolarCoordinate(
            Mathf.Lerp(a.angle, b.angle, time), 
            Mathf.Lerp(a.distance, b.distance, time),
            Mathf.Lerp(a.y, b.y, time) 
        );

    }


    public float magnitude => Mathf.Abs(angle + distance + y);

    public static PolarCoordinate zero => new PolarCoordinate(0, 0, 0);


    public PolarCoordinate(float angle, float distance, float y)
    {
        this.angle = angle;
        this.distance = distance;
        this.y = y;
    }
}