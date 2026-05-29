using UnityEngine;

public class BossManager : MonoBehaviour
{
    public float maxHp;
    public float currentHP;

    public float[] segments;
    public bool segmentEmpty;
    public int currentSegment;

    public float weakSpotAngle;
    public float weakSpotAngleRange;


    public float ratio => currentHP / maxHp;

    public BossHpBar hpBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Damage(1f, false, 0f);
        }
    }

    public void Damage(float damage, bool melee, float angle)
    {
        if (segmentEmpty)
        {
            if (CheckBreak(angle) && melee)
            {
                //break
            }
            else
            {
                //blink
            }
            return;

        }

        currentHP -= damage; 
        if (currentHP < 0) Debug.Log("DEAD");

        if (currentHP > segments[currentSegment-1])
        {
            hpBar.UpdateHp(ratio);
        }
        else
        {
        }

    }

    private bool CheckBreak(float angle)
    {
        return (Mathf.Abs(weakSpotAngle - angle) < weakSpotAngleRange);
    }
}
