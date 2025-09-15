using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class QTEEventTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public UnityEvent triggerEvent;
    public UnityEvent successEvent;
    public UnityEvent resetEvent;
    public QTEEvent qtEEvent;
    public bool canBeActivated = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public QTEEvent GetQTEEvent()
    {
        return qtEEvent;
    }

    public void QTESuccess()
    {
        successEvent.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canBeActivated) return;
        if (other.CompareTag("Player"))
        {
            canBeActivated = false;
            triggerEvent.Invoke();
        }
    }
}
