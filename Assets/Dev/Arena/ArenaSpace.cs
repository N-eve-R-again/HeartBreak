using UnityEngine;
using Unity.Mathematics;
public class ArenaSpace : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private GameObject warningObject;
    [SerializeField] private Animator telegraph;
    [SerializeField] private bool available = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool GetAvailability()
    {
        return available;
    }

    public Vector3 GetCenter()
    {
        return center.position;
    }


    public Vector3 GetCaseAxis(int2 _dir)
    {
        _dir = -_dir;
        
        return (center.right * _dir.x + center.forward * _dir.y).normalized;
    }

    public void SetWarningObjectState(bool _state)
    {
        warningObject.SetActive(_state);
    }

    public void TriggerTelegraph()
    {
        telegraph.SetTrigger("Telegraph");
    }
}
