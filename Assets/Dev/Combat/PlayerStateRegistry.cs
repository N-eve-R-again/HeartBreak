using System.Collections.Generic;
using System;
public static class PlayerStateRegistry
{
    private static Dictionary<Type, IPlayerState> map = new();
    public static void Register(IPlayerState state) => map[state.GetType()] = state;
    public static T Get<T>() where T : IPlayerState => (T)map[typeof(T)];
    public static IPlayerState Get(Type t) => map[t];
}
