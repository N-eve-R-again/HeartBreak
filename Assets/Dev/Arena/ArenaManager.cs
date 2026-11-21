using UnityEditor;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    [SerializeField] private ArenaParameters arenaParameters;
    public static ArenaManager instance;

    private void Start()
    {
        instance = this;
    }

    public static ArenaParameters arena => instance.arenaParameters;

    public static bool InBounds(float distance)
    {
        float distanceRound = Mathf.Round(distance);
        return (distanceRound >= arena.bounds.x && distanceRound <= arena.bounds.y);
    }

    public static float GetMaxDist() => arena.maxDistance;
    public static float DistanceToWS(float distance) => arena.ringToDistance.Evaluate(distance);
    public static float DistanceToMult(float distance) => arena.ringToSpeed.Evaluate(distance);


}
