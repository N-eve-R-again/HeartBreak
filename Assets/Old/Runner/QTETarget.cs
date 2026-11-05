using System;
using UnityEngine;

public class QTETarget : MonoBehaviour
{
    public QTEManager manager;
    public bool validated;
    public Animator animator;
    public bool usable = true;
    public bool activated = false;

    public ParticleSystem psWaiting;
    public ParticleSystem psCollectBurst;
    public ParticleSystem startEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartUp(Vector2 _spawnpos, float size)
    {
        validated = false;
        usable=false;
        activated = true;
        animator.SetTrigger("Spawn");
        startEffect.Play();
        psWaiting.Play();
        gameObject.GetComponent<RectTransform>().localPosition = _spawnpos;
        gameObject.GetComponent<RectTransform>().localScale = Vector3.one * size;
    }
    public void Despawned()
    {
        usable = true;
        validated = false;
        psWaiting.Stop();

    }
    public void DespawnTarget()
    {
        if (validated)
        {
            animator.SetTrigger("Despawn");
        }
        else if(activated)
        {
            animator.SetTrigger("Failed");
        }


    }
    public void Collect()
    {
        if (!activated) return;
        psWaiting.Stop();
        validated = true;
        //playanim
        manager.TargetCollected();
        animator.SetTrigger("Activate");
        psCollectBurst.Play();
        psWaiting.Stop();
        //play effect here
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
