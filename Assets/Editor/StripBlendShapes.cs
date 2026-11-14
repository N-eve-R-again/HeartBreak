using UnityEditor;
using UnityEngine;

// A placer dans un dossier nomme "Editor" (ex: Assets/Editor/).
// Retire les courbes de blendshapes que l'export FBX de Blender fabrique tout seul
// (Force Start/End Keying, Simplify=0) sur les clips importes, sans toucher au reste
// de l'animation (bones, root, etc.). Se relance automatiquement a chaque reimport.
public class StripBlendShapeCurves : AssetPostprocessor
{
    // Ne s'applique qu'aux assets sous ce dossier, pour ne pas casser les clips
    // ou tu VEUX garder une anim de blendshapes. Mets "" pour tout le projet.
    const string kFolderFilter = "Assets/Art/Characters/";

    // Optionnel : noms de blendshapes a CONSERVER malgre tout (ex: clignement auto).
    // Laisse vide pour tout retirer. Comparaison sur "blendShape.<nom>".
    static readonly string[] kKeep = { /* "blendShape.Blink", "blendShape.EyeClose" */ };

    void OnPostprocessAnimation(GameObject root, AnimationClip clip)
    {
        if (!string.IsNullOrEmpty(kFolderFilter) &&
            !assetPath.StartsWith(kFolderFilter, System.StringComparison.OrdinalIgnoreCase))
            return;

        int removed = 0;

        foreach (var binding in AnimationUtility.GetCurveBindings(clip))
        {
            // Les blendshapes sont des courbes float du SkinnedMeshRenderer,
            // nommees "blendShape.<nom>".
            bool isBlendShape =
                binding.type == typeof(SkinnedMeshRenderer) &&
                binding.propertyName.StartsWith("blendShape.", System.StringComparison.Ordinal);

            if (!isBlendShape)
                continue;

            if (System.Array.IndexOf(kKeep, binding.propertyName) >= 0)
                continue; // dans la liste a garder -> on n'y touche pas

            AnimationUtility.SetEditorCurve(clip, binding, null); // null = supprime la courbe
            removed++;
        }

        if (removed > 0)
            Debug.Log($"[StripBlendShapeCurves] {removed} courbe(s) de blendshape retiree(s) " +
                      $"de '{clip.name}'  ({assetPath})");
    }
}