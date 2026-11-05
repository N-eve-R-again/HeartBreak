using System.Collections;
using UnityEngine;

public class ControllerRunner : MonoBehaviour
{
    public bool playable;
    public float xPos;
    public float forwardSpeed;
    public float multiplierFwSpeed = 1f;
    public float sideSpeed;
    public Transform hardfollowTarget;
    public bool hardFollow;
    public ProceduralBobbing procBob;
    public float easeoutTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hardFollow && hardfollowTarget != null)
        {
            transform.position = hardfollowTarget.position;
            xPos = hardfollowTarget.position.x/6f;
            return;
        }
        if (playable)
        {
            float inputX = Input.GetAxis("Horizontal");
            procBob.steering = inputX;
            xPos += inputX * sideSpeed * Time.deltaTime;
            xPos = Mathf.Clamp(xPos, -1f, 1f);
            transform.position = new Vector3(xPos * 6f, transform.position.y, transform.position.z);
            transform.position += Vector3.forward * forwardSpeed * Time.deltaTime;
        }
        else
        {
            /*transform.position += Vector3.forward * forwardSpeed * Time.deltaTime * multiplierFwSpeed;
            if (multiplierFwSpeed <= 0f) 
            { 
                multiplierFwSpeed = 0f; 
                return;
            }
            multiplierFwSpeed -= Time.deltaTime * 1f/easeoutTimer;*/
            
        }
    }

    public void StopAmae(float easeout = 0.1f)
    {
        playable = false;
        easeoutTimer = easeout;
        multiplierFwSpeed = 1f;
    }

    public void StartHardFollow(Transform _target)
    {
        hardFollow = true;
        hardfollowTarget = _target;
    }
    public void StopHardFollow()
    {
        hardFollow = false;
        hardfollowTarget = null;
    }
    public void SnapAmae(Transform target)
    {
        playable = false;
        StartCoroutine(SnapAmaeCR(target));

    }

    public AnimationCurve snapcurve;
    IEnumerator SnapAmaeCR(Transform target)
    {
        AnimationCurve curve = snapcurve;
        Vector3 startpos = transform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 0.8f;
            transform.position = Vector3.Lerp(startpos, target.position, curve.Evaluate(t));
            yield return null;
        }


    }

    public void StartAmae()
    {
        playable = true;
        multiplierFwSpeed = 1f;
    }
}
