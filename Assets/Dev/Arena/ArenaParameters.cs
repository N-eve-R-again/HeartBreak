
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ArenaParameter", menuName = "HeartBreak/ArenaParameters")]
public class ArenaParameters : ScriptableObject
{

    [Header("Arena")]
    public int maxRing => ringRadii.Length - 1;
    public float[] ringRadii;
    public AnimationCurve ringToDistance;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (ringRadii == null || ringRadii.Length == 0) return;

        Keyframe[] keys = new Keyframe[ringRadii.Length];
        for (int i = 0; i < ringRadii.Length; i++)
        {
            keys[i] = new Keyframe(i, ringRadii[i]);
        }
        ringToDistance = new AnimationCurve(keys);

        // Force des tangentes linéaires pour passer pile par chaque point
        for (int i = 0; i < keys.Length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(ringToDistance, i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(ringToDistance, i, AnimationUtility.TangentMode.Linear);
        }
    }

#endif
}