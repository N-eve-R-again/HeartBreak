using UnityEngine;
public class MeleeEffect : MonoBehaviour
{
    public Animator animator;
    public PolarCoordinate spec;
    public Transform scaler;
    public ParticleSystem particleSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Play(PolarCoordinate ring)
    {

        scaler.localScale = new Vector3(1, 1, ArenaManager.DistanceToWS(ring.distance) / 4f);
        transform.rotation = Quaternion.Euler(0, ring.angle, 0f);
        animator.Play("A_Explode");
        particleSystem.Play();
    }
}
