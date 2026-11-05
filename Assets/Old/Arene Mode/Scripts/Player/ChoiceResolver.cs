
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using System;

[Serializable]
public abstract class ChoiceResolver
{
    [SerializeReference] protected List<PlayerAction> options;
    [SerializeField] protected int selectedIndex = 0;

    public ChoiceResolver(List<PlayerAction> _validOptions)
    {
        options = _validOptions;
    }
    protected void SelectOption(int _index)
    {
        selectedIndex = Mathf.Clamp(_index, 0, options.Count - 1);
    }
    protected abstract bool IsValidInput(int2 input);
    protected int GetOptionCount() => options.Count;
    public PlayerAction GetSelectedOption()
    {
        return options[selectedIndex];
    }
    public abstract void ShowPreview(PlayerBrain _playerBrain);
    public abstract void HandleInput(int2 input, PlayerBrain _playerBrain);
}

[Serializable] 
public class MoveChoice : ChoiceResolver
{
    public MoveChoice(List<PlayerAction> validOptions) : base(validOptions){ }

    public override void HandleInput(int2 _input, PlayerBrain _playerBrain)
    {
        if (IsValidInput(_input))
        {
            int index = 0;
            if(_input.x == 1) index = 1;
            SelectOption(index);
            _playerBrain.CompleteComplexChoice();
        }
    }

    protected override bool IsValidInput(int2 _input)
    {
        if (_input.x != 0 && _input.y == 0) {
            return true;
        }
        return false;
    }

    public override void ShowPreview(PlayerBrain _playerBrain)
    {
        Vector3 choice0WS = ArenaCoordinateSystem.instance.GetPositionInWorld(options[0].GetPreviewPos());
        Vector3 choice1WS = ArenaCoordinateSystem.instance.GetPositionInWorld(options[1].GetPreviewPos());
        Vector3 inbetween = Vector3.Slerp(choice1WS, choice0WS, 0.5f);
        _playerBrain.SetChoicePosition(inbetween);
    }
}

