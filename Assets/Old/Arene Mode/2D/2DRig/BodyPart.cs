using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class BodyPart : MonoBehaviour
{
    public SpriteResolver resolver;
    public string partName;
    public AngleDetector.Mode mode;
    public bool flipped = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SpriteDisplayData temp = AngleDetector.CalculateAngle(Camera.main, transform, mode);
        flipped = temp.flipped;
        resolver.SetCategoryAndLabel(partName, temp.label);
        if (flipped)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}

