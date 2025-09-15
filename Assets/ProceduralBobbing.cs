using UnityEngine;

public class ProceduralBobbing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float steering = 0f;
    public float anglesteeringForce = 10f;
    public float anglesteeringLerp = 2.5f;
    public float bobspeed = 1f;
    public float bobAmplitude = 1f;
    public Vector3 originPos = Vector3.zero;
    void Start()
    {
        originPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = originPos + new Vector3(0, Mathf.Sin(Time.time * bobspeed)* bobAmplitude, 0);
        transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(0,0,steering * anglesteeringForce),Time.deltaTime * anglesteeringLerp);
    }
}
