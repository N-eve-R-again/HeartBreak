using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class QTEManager : MonoBehaviour
{
    public QTEEvent currentQTEEvent;
    public QTECursor cursor;
    public QTETarget[] targets;
    public Volume PPvolume;
    public QTEEventTrigger trigger;
    public Image timergaugemask;
    public Image timergauge;
    
    public int targetProgressInWave = 0;
    public int totalTargetCountInWave = 0;
    public float timertillfail;
    public int currentWave = 0;
    public int totalWavesInEvent = 0;
    public bool readytoend = false;
    public bool success = false;

    public ParticleSystem timergaugeBurst;

    public AnimationCurve timerCurve;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            TargetCollected();
        }
    }

    public void StartupEvent(QTEEventTrigger _trigger)
    {
        trigger = _trigger;
    
        currentQTEEvent = trigger.GetQTEEvent();
        currentWave = 0;
        timertillfail = currentQTEEvent.timeToExectute;
        totalWavesInEvent = currentQTEEvent.nbOfWaves();

        
        StartCoroutine(QTEEventCR());
    }

    IEnumerator QTEEventCR()
    {
        float t = 0;
        timergaugemask.fillAmount = 0f;
        timergauge.color = new Color(1f, 1f, 1f, 0.5f);
        float timermax = timertillfail;
        while (t < 1f) {
            PPvolume.weight = t;
            t += Time.deltaTime * 1f;
            yield return null;
            timergaugemask.fillAmount = Mathf.Lerp(timergaugemask.fillAmount, 1.1f,Time.deltaTime*3f);
        }
        timergaugemask.fillAmount = 1f;
        cursor.gameObject.SetActive(true);
        cursor.PlaceCursor(currentQTEEvent.cursorStartPos);
        
        yield return new WaitForSeconds(0.8f);
        StartUpWave();
        yield return new WaitForSeconds(0.6f);
        cursor.StartMove();
        timergaugeBurst.Play();
        timergauge.color = new Color(1f, 1f, 1f, 1f);
        while (!readytoend) {

            if (readytoend) break;
            timertillfail -= Time.deltaTime;
            float ratio = timertillfail / timermax;

            timergaugemask.fillAmount = timerCurve.Evaluate(ratio);
            if (timertillfail < 0f)
            {
                Debug.Log("QTE FAILED NO TIME");
                DespawnWave();
                yield return new WaitForSeconds(0.75f);
                break;
            }

            if (targetProgressInWave >= totalTargetCountInWave)
            {
                Debug.Log("finished QTE Wave");
                currentWave++;
                
                if (currentWave == currentQTEEvent.nbOfWaves())
                {
                    timergauge.color = new Color(1f, 1f, 1f, 0.5f);
                    Debug.Log("Success");
                    readytoend = true;
                    success = true;
                    DespawnWave();
                    cursor.Stop();
                    yield return new WaitForSeconds(0.25f);
                    cursor.Success();
                    //disable

                }
                else
                {
                    DespawnWave();
                    yield return new WaitForSeconds(0.05f);
                    StartUpWave();
                }
            }

            yield return null;
        }
        t = 0;
        //yield return new WaitForSeconds(0.2f);

        while (t < 1f)
        {
            timergaugemask.fillAmount = Mathf.Lerp(timergaugemask.fillAmount, -0.1f, Time.deltaTime * 3f);
            PPvolume.weight = 1 - t;
            t += Time.deltaTime * 1f;
            
            yield return null;
        }
        if (success)
        {
            trigger.QTESuccess();
        }
        else
        {
            Debug.Log("NOT A SUCESS");
        }
        
        timergaugemask.fillAmount = 0f;
        yield return new WaitForSeconds(1f);
        cursor.gameObject.SetActive(false);
        yield return null;
    }
    public void StartUpWave()
    {
        targetProgressInWave = 0;
        totalTargetCountInWave = currentQTEEvent.nbOfTargetInWave(currentWave);
        SpawnWave();
    }

    public void TargetCollected()
    {
        targetProgressInWave++;

    }

    
    public void SpawnWave()
    {
        for (int j = 0; j < totalTargetCountInWave; j++)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].usable)
                {
                    Debug.Log("Spawned Target => " + j + " with target " + i);
                    targets[i].StartUp(currentQTEEvent.qTEEventWaves[currentWave].positions[j], currentQTEEvent.qTEEventWaves[currentWave].sizes[j]);
                    break;
                }

            }
        }

    }

    public void DespawnWave()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            Debug.Log("Despawned Target => " + i);
            targets[i].DespawnTarget();
            //targets[i].DespawnTarget();
        }
    }
}

[Serializable]
public class QTEEvent
{
    public QTEEventWave[] qTEEventWaves;
    public float timeToExectute;
    public Vector2 cursorStartPos;
    public int nbOfWaves()
    {
        return qTEEventWaves.Length;
    }
    public int nbOfTargetInWave(int wave)
    {
        return qTEEventWaves[wave].positions.Length;
    }
}

[Serializable]
public class QTEEventWave
{
    public Vector2[] positions;
    public float[] sizes;
}
