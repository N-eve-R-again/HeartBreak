using System.Collections.Generic;
using System;
using static PlayerController;

public static class PlayerStateRegistry
{
    private static Dictionary<Type, IPlayerState> map = new();
    private static PlayerControllerSettings settings;
    public static void Register(IPlayerState state) => map[state.GetType()] = state;
    public static T Get<T>() where T : IPlayerState => (T)map[typeof(T)];
    public static IPlayerState Get(Type t) => map[t];

    public static PlayerControllerSettings GetSettings() => settings;
    public static void SetSettings(PlayerControllerSettings _settings) => settings = _settings; 
}
