using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public abstract class BossAction
{
    public abstract void Execute(BossEntity _boss);
    public abstract void Preview();
    public abstract void EndPreview();
    public abstract void Telegraph();

}

[Serializable]
public class RingAttack : BossAction
{
    private int targetRing;
    private int nb;
    public RingAttack(int _targetRing)
    {
        targetRing = _targetRing;
        nb = ArenaCoordinateSystem.instance.GetRingLength(targetRing) + 1;
    }
    public override void Execute(BossEntity _boss)
    {
        //_boss.ExectuteAttack(this)
    }

    public override void Preview()
    { 
        for (int i = 0; i < nb; i++)
        {
            int2 pos = new int2(i, targetRing);
            ArenaCoordinateSystem.instance.SetWarning(true, pos);
        }
    }

    public override void EndPreview()
    {
        for (int i = 0; i < nb; i++)
        {
            int2 pos = new int2(i, targetRing);
            ArenaCoordinateSystem.instance.SetWarning(false, pos);
        }
    }

    public override void Telegraph() {
        for (int i = 0; i < nb; i++)
        {
            int2 pos = new int2(i, targetRing);
            ArenaCoordinateSystem.instance.TriggerTeleraph(pos);
        }

    }
}
