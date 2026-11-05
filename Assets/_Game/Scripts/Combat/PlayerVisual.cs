using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private PlayerControllerSettings settings;
    [SerializeField] private Transform visualPivot;
    // Update is called once per frame

    public void Init(PlayerControllerSettings settings)=> this.settings = settings;

    public void UpdatePosition(PolarCoordinate position)
    {
        float realDist = ArenaManager.DistanceToWS(position.distance);

        transform.position = Vector3.back * realDist;
        transform.position = Quaternion.Euler(new Vector3(0f, position.angle, 0f)) * transform.position;
        transform.position = transform.position + Vector3.up * position.y;
    }

    public void UpdateFacing(Vector2 facing)
    {
        Vector3 radial = -transform.position;
        radial.y = 0; radial.Normalize();
        Vector3 tangent = Vector3.Cross(Vector3.up, radial);

        Vector3 worldDir = tangent * facing.x + radial * facing.y;

        visualPivot.transform.rotation = Quaternion.Lerp(visualPivot.transform.rotation, Quaternion.LookRotation(worldDir, Vector3.up), Time.deltaTime * 8f);

    }
}
