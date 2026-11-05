using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class Move : PlayerAction
{
    [SerializeField] private MoveData data;
    public Move(int2 _direction, int2 _origin) //Constructeur objet
    {
        data = new MoveData(_origin, _direction);
    }
    public override void Calculate()
    {
        ArenaCoordinateSystem.instance.SolveMove(ref data);

    }
    public override int2 GetPreviewPos() //Retourne la position pour la preview
    {
        return data.GetFinalPos();
    }
    public override int2 GetActionOrigin()
    {
        return data.origin;
    }

    public MoveData GetData()
    {
        return data;
    }
    public override void Execute(PlayerEntity _player) //Executer l'action
    {
        _player.ExecuteMoveAction(data);
    }

    //Fonctions Specifiques du type Move
    public bool IsValid() //Retourne si le déplacement est Valid
    {
        return data.IsValid();
    }

}

