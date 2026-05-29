using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : MonoBehaviour
{
    public Image hpBar;
    public Image comboHpBar;

    public Color restHpBarColor;
    public Color BlinkHpBarColor;


    public float comboMaxTimer;
    public float currentComboTimer;
    public bool inCombo;

    public float restHp;
    public float dynamicHp;

    public BossHpBarSegment[] segments;


    public void UpdateHp(float newHp)
    {
        dynamicHp = newHp;
        currentComboTimer = comboMaxTimer;
        inCombo = true;
        BlinkHp();
    }


    public void EndCombo()
    {
        restHp = dynamicHp;
        inCombo = false;
        currentComboTimer = 0;
    }

    private void CheckForComboEnd()
    {
        if (inCombo)
        {
            currentComboTimer -= Time.deltaTime;
            if (currentComboTimer < 0)
            {
                EndCombo();
            }
        }

    }

    private void BlinkHp()
    {
        hpBar.color = BlinkHpBarColor;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        restHp = 1f;
        dynamicHp = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForComboEnd();

        hpBar.color = Color.Lerp(hpBar.color, restHpBarColor, Time.deltaTime * 5f);
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, dynamicHp, Time.deltaTime * 4f);
        comboHpBar.fillAmount = Mathf.Lerp(comboHpBar.fillAmount, restHp, Time.deltaTime * 10f);
    }
}

