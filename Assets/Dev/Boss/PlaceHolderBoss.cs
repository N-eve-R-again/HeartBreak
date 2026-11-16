using UnityEngine;

public class PlaceHolderBoss : MonoBehaviour
{
    public Bullet projectilePrefab;
    public float rate;
    public float timer;

    private Bullet[] pool;
    private int poolSize = 50;

    void Awake()
    {
        pool = new Bullet[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Instantiate(projectilePrefab, transform);
            pool[i].Init();
        }
        timer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > rate) {
            Bullet bullet = GetFromPool();
            bullet.Fire(Mathf.Round(Random.Range(0f, 359f)));
            timer = 0f;
        }
    }

    private Bullet GetFromPool()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].active) return pool[i];
        }
        return null;
    }


}
