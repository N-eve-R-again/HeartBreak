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
    public static float DistanceToWS(float distance) => arena.ringToDistance.Evaluate(distance);


}
