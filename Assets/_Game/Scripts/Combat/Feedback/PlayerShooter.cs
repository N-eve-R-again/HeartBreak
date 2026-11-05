using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public PeelAttackProjectile projectilePrefab;

    [Header("Peel Settings")]
    public float projectileSpeed = 8f;
    public float projectileDamage = 0.25f;

    // Pool simple — on pré-alloue N projectiles
    private PeelAttackProjectile[] pool;
    private int poolSize = 8;

    void Awake()
    {
        pool = new PeelAttackProjectile[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Instantiate(projectilePrefab, transform);
            pool[i].gameObject.SetActive(false);
        }
    }

    public void ShootPeel(PolarCoordinate from)
    {
        var proj = GetFromPool();
        if (proj == null) return;
        proj.gameObject.SetActive(true);
        proj.Launch(from, projectileSpeed, projectileDamage);
    }

    private PeelAttackProjectile GetFromPool()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].gameObject.activeSelf) return pool[i];
        }
        return null;
    }
}