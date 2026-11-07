using UnityEngine;

public class MainMovementResolver : IResolver
{
    public Transition Dispatch(PlayerContext ctx)
    {
        if(ctx.Data.position.y > 0)
        {
            Debug.Log("FAll");
        }

        //if(!ctx.grounded) => fall

        if (ctx.Inputs.MoveMagnitude <= 0.1f && ctx.Data.velocity.magnitude < 0.1f)
        {
            return Transition.GoTo<PlayerStateIdle>();
        }
        else
        {
            return Transition.GoTo<PlayerStateMove>();
        }

    }



}
