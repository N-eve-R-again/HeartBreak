using UnityEngine;
using System.Collections;


public class ProceduralEye : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer skinnedMeshEye;
    [SerializeField] SkinnedMeshRenderer skinnedMeshIris;
    [SerializeField] string blendShapeName = "Blink";

    [Header("Timing (secondes)")]
    [SerializeField] Vector2 intervalRange = new Vector2(2f, 6f); // attente aléatoire entre deux clignements
    [SerializeField] float closeDuration = 0.06f;
    [SerializeField] float closedHold = 0.04f;
    [SerializeField] float openDuration = 0.10f;

    [SerializeField] float glintspeed = 4f;
    [SerializeField] float glintforce = 0.5f;

    [Header("Amplitude")]
    [SerializeField] float maxWeight = 100f; // 100 par défaut, 1 si ton mesh est importé en 0-1

    int blendShapeIndex = -1;
    int blendShapeIndexGlint = -1;
    [SerializeField] private float blinkglintmask;

    [SerializeField] Transform eyeBone;
    [SerializeField] float dartAmplitude = 0.0005f;              // à régler, voir plus bas
    [SerializeField] Vector2 dartInterval = new Vector2(0.5f, 3f);
    [SerializeField] Vector2 holdRange = new Vector2(0.05f, 0.25f);
    [SerializeField] float moveDuration = 0.03f;                  // 0 = snap

    Vector3 baseLocalPos;
    Vector3 actualpos;

    void Awake()
    {
        baseLocalPos = eyeBone.localPosition;
        actualpos = baseLocalPos;

        blendShapeIndexGlint = skinnedMeshIris.sharedMesh.GetBlendShapeIndex("Glint");
        if (skinnedMeshEye == null)
            skinnedMeshEye = GetComponent<SkinnedMeshRenderer>();

        if (skinnedMeshEye != null && skinnedMeshEye.sharedMesh != null)
            blendShapeIndex = skinnedMeshEye.sharedMesh.GetBlendShapeIndex(blendShapeName);

        if (blendShapeIndex < 0)
            Debug.LogError($"[EyeBlink] Blendshape '{blendShapeName}' introuvable.", this);
    }

    void OnEnable()
    {
        if (blendShapeIndex >= 0)
            StartCoroutine(BlinkLoop());
        StartCoroutine(DartLoop());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        if (blendShapeIndex >= 0)
            skinnedMeshEye.SetBlendShapeWeight(blendShapeIndex, 0f);
    }

    private void LateUpdate()
    {
        float value = (Mathf.Sin(Time.time * glintspeed) + 1f) * glintforce / 2f;
        value = value * 100f;


        blinkglintmask = skinnedMeshEye.GetBlendShapeWeight(blendShapeIndex) > 9f ? 1 : 0;
        value = blinkglintmask > 0f ? -300 : value;
        skinnedMeshIris.SetBlendShapeWeight(blendShapeIndexGlint, value) ;
        eyeBone.localPosition = actualpos;
    }

    IEnumerator BlinkLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(intervalRange.x, intervalRange.y));
            yield return Fade(8f, maxWeight, closeDuration);
            yield return new WaitForSeconds(closedHold);
            yield return Fade(maxWeight, 8f, openDuration);
        }
    }



    IEnumerator Fade(float from, float to, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float k = Mathf.SmoothStep(0f, 1f, t / duration);
            skinnedMeshEye.SetBlendShapeWeight(blendShapeIndex, Mathf.Lerp(from, to, k));
            yield return null;
        }
        skinnedMeshEye.SetBlendShapeWeight(blendShapeIndex, to);
    }

    IEnumerator DartLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(dartInterval.x, dartInterval.y));

            Vector2 r = Random.insideUnitCircle * dartAmplitude;
            yield return MoveTo(baseLocalPos + new Vector3(r.x, 0f, r.y));

            yield return new WaitForSeconds(Random.Range(holdRange.x, holdRange.y));
            yield return MoveTo(baseLocalPos);
        }
    }

    IEnumerator MoveTo(Vector3 target)
    {
        Vector3 start = actualpos;
        for (float t = 0f; t < moveDuration; t += Time.deltaTime)
        {
            actualpos = Vector3.Lerp(start, target, t / moveDuration);
            yield return null;
        }
        actualpos = target;
    }

}
