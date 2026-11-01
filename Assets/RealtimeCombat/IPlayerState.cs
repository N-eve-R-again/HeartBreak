using System;

public interface IPlayerState
{
    void ResetInternal();
    void Enter(PlayerContext ctx, IPlayerState fromState);
    Transition Execute(PlayerContext ctx);
    void Exit(PlayerContext ctx);

    void Init(PlayerContext ctx) { return; }
}

public static class PlayerStateExtensions
{
    public static Transition Stay(this IPlayerState s) => Transition.Stay;
    public static Transition Restart(this IPlayerState s) => Transition.Restart;
    public static Transition GoTo<T>(this IPlayerState s) where T : IPlayerState => Transition.GoTo<T>();
    public static Transition WaitFor<T>(this IPlayerState s) where T : IPlayerState => Transition.WaitFor<T>();
}

public enum TransitionType { Stay, Restart, Goto, WaitFor, Done}

public readonly struct Transition
{
    public readonly TransitionType Type;
    public readonly IPlayerState Target;

    private Transition(TransitionType type, IPlayerState target)
    {
        Type = type; Target = target;
    }

    public static readonly Transition Stay = new Transition(TransitionType.Stay, null);
    public static readonly Transition Restart = new Transition(TransitionType.Restart, null);
    public static readonly Transition Done = new Transition(TransitionType.Done, null);
    public static  Transition GoTo<T>() where T : IPlayerState
        => new Transition(TransitionType.Goto, PlayerStateRegistry.Get(typeof(T))); 
    public static  Transition WaitFor<T>() where T : IPlayerState 
        => new Transition(TransitionType.WaitFor, PlayerStateRegistry.Get(typeof(T))); 
}