using UnityEngine;

public interface IResolver
{
    public Transition Dispatch(PlayerContext ctx);
}
