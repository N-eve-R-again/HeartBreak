
using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct ArenaRingData
{
    public float radius;
    public float movementMultiplier;
}

[CreateAssetMenu(fileName = "ArenaParameter", menuName = "HeartBreak/ArenaParameters")]
public class ArenaParameters : ScriptableObject
{


    [Header("Arena")]

    public AnimationCurve ringToDistance;
    public AnimationCurve ringToSpeed;
    public ArenaRingData[] rings;
    public float maxDistance => rings.Length -1;
    public Vector2 bounds = new Vector2(1f, 4f);

#if UNITY_EDITOR
    void OnValidate()
    {
        if (rings == null || rings.Length == 0) return;

        Keyframe[] keys = new Keyframe[rings.Length];
        for (int i = 0; i < rings.Length; i++)
        {
            keys[i] = new Keyframe(i, rings[i].radius);
        }
        ringToDistance = new AnimationCurve(keys);

        Keyframe[] keys2 = new Keyframe[rings.Length];
        for (int i = 0; i < rings.Length; i++)
        {
            keys2[i] = new Keyframe(i, rings[i].movementMultiplier);
        }

        ringToSpeed = new AnimationCurve(keys2);
        // Force des tangentes linéaires pour passer pile par chaque point
        for (int i = 0; i < keys.Length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(ringToDistance, i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(ringToDistance, i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyLeftTangentMode(ringToSpeed, i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(ringToSpeed, i, AnimationUtility.TangentMode.Linear);
        }
    }

#endif
}