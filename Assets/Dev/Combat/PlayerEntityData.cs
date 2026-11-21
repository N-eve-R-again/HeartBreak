using System;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


[Serializable]
public class PlayerEntityData
{
    public PolarCoordinate position = PolarCoordinate.back;
    public PolarCoordinate velocity;
    public Vector2 facing;

    public float time;
    public int currentCharges;
    public float recharge;

    public float iFrameTimer;
    public float hp;

    public bool TryConsumeEnergy()
    {
        if(currentCharges == 0) return false;
        recharge = 0f; //on reset le rechargement.
        currentCharges--;
        return true;
    }

    public bool Hit(float Damage)
    {
        hp -= Damage;
        if(hp < 0f)
        {
            hp = 0f;
            Debug.Log("DEAD");
            return true;
        }
        return false;
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
        return new PolarCoordinate(left.angle + right.angle, left.distance + right.distance, left.y + right.y).Solve();
    }

    public static PolarCoordinate operator *(PolarCoordinate polar, float multiplier)
    {
        return new PolarCoordinate(polar.angle * multiplier, polar.distance * multiplier, polar.y * multiplier).Solve();
    }

    public static PolarCoordinate Lerp(PolarCoordinate a, PolarCoordinate b, float time)
    {
        return new PolarCoordinate(
            Mathf.LerpAngle(a.angle, b.angle, time), 
            Mathf.Lerp(a.distance, b.distance, time),
            Mathf.Lerp(a.y, b.y, time) 
        );

    }


    public float magnitude => Mathf.Abs(angle + distance + y);
    public float angleMagnitude => Mathf.Abs(angle);

    public static PolarCoordinate zero => new PolarCoordinate(0, 0, 0);
    public static PolarCoordinate right => new PolarCoordinate(1f, 0f, 0f);
    public static PolarCoordinate left => new PolarCoordinate(-1f, 0f, 0f);
    public static PolarCoordinate forward => new PolarCoordinate(0f, -1f, 0f);
    public static PolarCoordinate back => new PolarCoordinate(0f, 1f, 0f);

    public PolarCoordinate(float angle, float distance, float y)
    {
        this.angle = angle;
        this.distance = distance;
        this.y = y;
    }

}

public static class PolarOperations
{
    public static PolarCoordinate Solve(this PolarCoordinate coordinate)
    {
        coordinate.angle = Mathf.Repeat(coordinate.angle, 360f);
        coordinate.y = Mathf.Max(coordinate.y, 0f);
        return coordinate;
    }

    public static Vector3 ToWorld(this PolarCoordinate coordinate)
    {
        float realDist = ArenaManager.DistanceToWS(coordinate.distance);

        Vector3 pos = Vector3.back * realDist;
        pos = Quaternion.Euler(new Vector3(0f, coordinate.angle, 0f)) * pos;
        return pos + Vector3.up * coordinate.y;
    }

}