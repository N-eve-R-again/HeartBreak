using UnityEngine;

public class SpriteDisplayData
{
    public bool flipped;
    public string label;
    public SpriteDisplayData(bool _flipped, string _label)
    {
        flipped = _flipped;
        label = _label;
    }
}
public static class AngleDetector
{
    public enum Mode { FourWay, EightWay, TwoWay, FourWayUnique }
    public static string[] suffixes8 = new string[] { "Front", "FrontLeft", "Left", "BackLeft", "Back" , "BackRight", "Right", "FrontRight"};
    public static string[] suffixes4 = new string[]{"Front","Left","Back","Right"};

    
    public static SpriteDisplayData CalculateAngle(Camera cam, Transform inputTransform, Mode mode)
    {
        var toCamera = (cam.transform.position - inputTransform.position).normalized;
        toCamera.y = 0;

        // Angle entre forward et caméra (-180 à 180)
        float angle = Vector3.SignedAngle(inputTransform.forward, toCamera, Vector3.up);

        int temp = GetAngleIndex(angle, mode);
        return GetSpriteLabelByAngle(mode, temp);
    }

    private static int GetAngleIndex(float angle, Mode mode)
    {
        // Normaliser 0-360
        angle = (angle + 360) % 360;

        if (mode == Mode.EightWay)
        {
            // 8 directions: 0=F, 1=FL, 2=L, 3=BL, 4=B, 5=BR, 6=R, 7=FR
            return Mathf.RoundToInt(angle / 45f) % 8;
        }
        else
        {
            // 4 directions: 0=F, 1=L, 2=B, 3=R
            return Mathf.RoundToInt(angle / 90f) % 4;
        }
    }



    private static SpriteDisplayData GetSpriteLabelByAngle(Mode mode, int index)
    {
        string label = "";
        bool flip = false;



        if (mode == Mode.EightWay) // 8 directions: 0=F, 1=FL, 2=L, 3=BL, 4=B, 5=BR, 6=R, 7=FR
        {

                flip = (index >= 5);
                label = suffixes8[index];

        }
        else // 4 directions: 0=F, 1=L, 2=B, 3=R
        {
            if (mode == Mode.FourWay)
            {
                label = suffixes4[index];
                flip = (index > 2);
            }
            else if (mode == Mode.TwoWay)
            {
                label = suffixes4[index];
                flip = (index >= 2);
            }else if (mode == Mode.FourWayUnique)
            {
                label = suffixes4[index];
            }

        }

        SpriteDisplayData data = new SpriteDisplayData(flip, label);
        return data;
    }

}