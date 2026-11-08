using System.Collections;
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

    public void ShootPeel(PolarCoordinate from, int number, float delay)
    {

        StartCoroutine(ProjectileCR(from, number, delay));
        /*
        var proj = GetFromPool();
        if (proj == null) return;
        proj.gameObject.SetActive(true);
        proj.Launch(from, projectileSpeed, projectileDamage);*/
    }

    IEnumerator ProjectileCR(PolarCoordinate from, int number, float delay)
    {
        for (int i = 0; i < number; i++) 
        {
            var proj = GetFromPool();
            if (proj == null) yield return null;
            proj.gameObject.SetActive(true);
            proj.Launch(from, projectileSpeed, projectileDamage);
            yield return new WaitForSeconds(delay);
        }

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