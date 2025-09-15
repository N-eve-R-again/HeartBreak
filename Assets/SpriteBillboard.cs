using UnityEngine;


public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] Transform pivot;
    [SerializeField] bool useWorldUp;
    [SerializeField] bool lockToWorldUp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    Vector3 AlignToPlane(Vector3 _vector)
    {
        Vector3 output = new Vector3(_vector.x, 0, _vector.z);
        return output;
    }
    // Update is called once per frame
    void Update()
    {
        if (pivot == null) return;
        Vector3 lookvector;
        if (lockToWorldUp)
        {
            lookvector = AlignToPlane(Camera.main.transform.position) - AlignToPlane(pivot.position);
        }
        else 
        {
            lookvector = Camera.main.transform.position - pivot.position;
        }

        lookvector = lookvector * -1f;
        lookvector.Normalize();
        if (useWorldUp)
        {
            pivot.rotation = Quaternion.LookRotation(lookvector, Vector3.up);
        }
        else
        {
            pivot.rotation = Quaternion.LookRotation(lookvector, Camera.main.transform.up);
        }
            
    }
}
