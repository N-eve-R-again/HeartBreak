using UnityEngine;
using static PlayerController;

public interface IPlayerState
{
    void Enter(ref PlayerEntityData _data, IPlayerState fromState);
    IPlayerState Execute(ref PlayerEntityData _currentData, PlayerInputData _inputs);
    void Exit(ref PlayerEntityData _data);
}

public static class PlayerStateExtensions
{
    public static IPlayerState Stay(this IPlayerState s) => s;
    public static IPlayerState GoTo<T>(this IPlayerState s) where T : IPlayerState
        => PlayerStateRegistry.Get<T>();


}