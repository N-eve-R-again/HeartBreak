using UnityEngine;
using static PlayerController;

public interface IPlayerState
{
    void Enter(ref PlayerEntityData data, IPlayerState fromState);
    IPlayerState Execute(ref PlayerEntityData data, PlayerInputData inputs);
    void Exit(ref PlayerEntityData data);
}

public static class PlayerStateExtensions
{
    public static IPlayerState Stay(this IPlayerState s) => s;
    public static IPlayerState GoTo<T>(this IPlayerState s) where T : IPlayerState
        => PlayerStateRegistry.Get<T>();


}