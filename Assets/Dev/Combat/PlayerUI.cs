using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image[] dashslots;
    public Image fill;

    public void UpdateDashSlots(int number)
    {
        for (int i = 0; i < dashslots.Length; i++)
        {

            dashslots[i].enabled = i < number;

        }

    }

    public void UpdateFill(float ratio)
    {
        fill.fillAmount = ratio;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
