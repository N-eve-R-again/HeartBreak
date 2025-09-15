using UnityEngine;

public class BossBrain : MonoBehaviour
{
    [SerializeReference] public BossAction[] actions = new BossAction[4];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actions[0] = new RingAttack(0);
        actions[1] = new RingAttack(1);
        actions[2] = new RingAttack(2);
        actions[3] = new RingAttack(3);
    }


}
