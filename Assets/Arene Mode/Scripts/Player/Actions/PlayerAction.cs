using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public abstract class PlayerAction
{
    public abstract void Execute(PlayerEntity _player);
    public abstract void Calculate();
    public abstract int2 GetPreviewPos();
    public abstract int2 GetActionOrigin();
}

[Serializable]
public class NoAction : PlayerAction
{
    public override void Execute(PlayerEntity _player) { }
    public override void Calculate() { }
    public override int2 GetPreviewPos() => new int2();

    public override int2 GetActionOrigin() => new int2();

}
