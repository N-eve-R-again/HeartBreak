using UnityEngine;

public class PeelAttackProjectile : MonoBehaviour
{
    [Header("Refs")]
    public GameObject visual;
    public ParticleSystem trailPS;
    public ParticleSystem hitPS;

    [Header("Linger")]
    public float lingerTime = 0.5f;

    [HideInInspector] public float speed = 8f;
    [HideInInspector] public float damage = 0.25f;

    private PolarCoordinate position;
    private PlayerControllerSettings settings;
    private bool alive;
    private bool lingering;
    private float lingerTimer;

    public void Launch(PolarCoordinate from, float speed, float damage)
    {
        this.position = from;
        this.speed = speed;
        this.damage = damage;
        this.settings = PlayerStateRegistry.GetSettings();
        this.alive = true;
        this.lingering = false;
        gameObject.SetActive(true);
        visual.SetActive(true);
        if (trailPS != null) trailPS.Play();
        ApplyWorldPos();
    }

    void Update()
    {
        if (lingering)
        {
            lingerTimer -= Time.deltaTime;
            if (lingerTimer <= 0f) gameObject.SetActive(false);
            return;
        }

        if (!alive) return;

        position.distance -= speed * Time.deltaTime;

        if (position.distance <= 0.1f)
        {
            Hit();
            return;
        }

        ApplyWorldPos();
    }

    private void ApplyWorldPos()
    {
        float realDist = settings.ringToDistance.Evaluate(position.distance);
        Vector3 worldPos = Vector3.back * realDist;
        worldPos = Quaternion.Euler(0f, position.angle, 0f) * worldPos;
        worldPos.y = position.y;
        transform.position = worldPos;

        transform.rotation = Quaternion.Euler(0f, position.angle, 0f);
    }

    private void Hit()
    {
        alive = false;
        BossManager.instance.Damage(damage, false, position.angle);

        visual.SetActive(false);
        if (trailPS != null) trailPS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (hitPS != null) hitPS.Play();

        lingering = true;
        lingerTimer = lingerTime;
    }
}