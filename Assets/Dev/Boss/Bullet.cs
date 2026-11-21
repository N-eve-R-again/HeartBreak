using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool active = false;
    public PolarCoordinate pos;
    public float speed;

    public AnimationCurve curve;
    public float pausetime;
    public float stepSize = 1f;
    public float stepTime = 0.48f;

    public bool flag;

    public void Init()
    {
        active = false;
        transform.localScale = Vector3.zero;
        transform.position = Vector3.one * 100f;
        pos.distance = -1;
        flag = false;
    }

    public void Reset()
    {
        StopAllCoroutines();
        active = false;
        transform.DOScale(Vector3.zero, stepTime).SetEase(Ease.InOutCubic);
        flag = true;
    }

    [ContextMenu("FIRE")]
    public void Fire(float angle)
    {
        active = true;
        pos.angle = angle;
        
        transform.DOScale(Vector3.one * 0.78f, stepTime).SetEase(Ease.OutBack);
        StartCoroutine(WaveCR());
    }

    // Update is called once per frame
    void Update()
    {
        if(active) 
            UpdateWorldPos();

        if(transform.localScale.magnitude < 0.01f && flag)
        {
            Init();
        }
    }


    private IEnumerator WaveCR()
    {
        float max = ArenaManager.GetMaxDist();

        while (pos.distance < max)
        {
            float from = pos.distance;
            yield return AdvanceCR(from, from + stepSize); // waits until the step is finished
            yield return new WaitForSeconds(pausetime);
        }

        // reached the end: same cleanup you had in Update
        Reset();
    }

    private IEnumerator AdvanceCR(float from, float to)
    {
        float t = 0;
        while(t < stepTime)
        {

            t += Time.deltaTime;
            float ratio = t / stepTime;
            pos.distance = Mathf.Lerp(from, to, curve.Evaluate(ratio));
            yield return null;

        }
        pos.distance = to;

    }

    private void UpdateWorldPos()
    {
        transform.position = pos.ToWorld();
    }
}
