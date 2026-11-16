using UnityEngine;

public class PlaceHolderBoss : MonoBehaviour
{
    public Bullet projectilePrefab;
    public float rate;
    public float timer;

    private Bullet[] pool;
    private int poolSize = 100;

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
        if (timer >= 0f) {
            int nb = Random.Range(0, 20);
            switch (nb)
            {

                case 0:
                    SemiCircleAttack();
                    break;
                case 1:
                case 2:
                case 3:
                    ArcAttack();
                    break;
                default:
                    SingleBullet();
                    break;
            }


        }
    }
    private void SingleBullet()
    {
        InstantiateBullet(Mathf.Round(Random.Range(0f, 359f)));
        timer = RateCalc(1);
    }
    private void ArcAttack()
    {
        int nb = Random.Range(3, 8);
        float startAngle = Mathf.Round(Random.Range(0f, 359f));
        for (int i = 0; i < nb; i++)
        {
            InstantiateBullet(startAngle + i * 10f);
        }
        timer = RateCalc(6);
    }
    
    private float RateCalc(float nb) => -rate * nb;
    private void SemiCircleAttack()
    {
        int nb = Random.Range(5, 1);

        float startAngle = Mathf.Round(Random.Range(0f, 359f));
        for (int i = 0; i < 36 - nb; i++)
        {
            InstantiateBullet(startAngle + i * 10f);
        }

        timer = RateCalc(8);
    }

    private void InstantiateBullet(float angle)
    {
        Bullet bullet = GetFromPool();
        if (bullet == null) return;
        bullet.Fire(angle);
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
