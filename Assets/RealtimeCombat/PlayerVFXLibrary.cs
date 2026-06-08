using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerVFXLibrary : MonoBehaviour
{
    [Header("Dashing Charge")]
    public AttackGizmo attackGizmo;
    public Volume chargeVolume;
    public ParticleSystem chargePS;
    public ParticleSystem chargeCompletePS;
    public CinemachineCamera cameraCharge;
    public CinemachineCamera cameraBase;
    public CinemachineBrain cameraBrain;
    public CanvasGroup screenPowerUp;

    [Header("OrbitalDash")]
    public ParticleSystem psOrbitalDash;
    


    [Header("Attack")]
    public MeleeEffect meleeEffect;
    public PlayerShooter shooter;

    // --- Charge effect (self-driven) ---
    [Header("Charge Effect Settings")]
    private float chargeEffectCurrent;
    private float chargeEffectTarget;
    private float chargeEffectSpeed;

    public float ChargeEffect => chargeEffectCurrent;

    /// <summary> Sustain continu — appelé par la charge state chaque frame. </summary>
    public void ChargeEffectSustain(float target, float speed)
    {
        chargeEffectTarget = target;
        chargeEffectSpeed = speed;
    }


    public void PlayOrbitalDashPS(Vector3 direction)
    {
        //psOrbitalDash.transform.LookAt(direction, Vector3.up);
        psOrbitalDash.Play();

    }


    /// <summary> Pulse instantané (snap à une valeur haute, le sustain reprend après). </summary>
    public void ChargeEffectPulse(float value)
    {
        chargeEffectCurrent = value;
    }

    /// <summary> Fade out autonome — les states n'ont plus besoin de s'en occuper. </summary>
    public void ChargeEffectFadeOut(float fadeOutSpeed = 2f)
    {
        chargeEffectTarget = 0f;
        chargeEffectSpeed = fadeOutSpeed;
    }

    /// <summary> Kill immédiat sans fade (reset, respawn, etc). </summary>
    public void ChargeEffectKill()
    {
        chargeEffectCurrent = 0f;
        chargeEffectTarget = 0f;
        ApplyChargeEffect();
    }

    private void ApplyChargeEffect()
    {
        chargeVolume.weight = chargeEffectCurrent;
        screenPowerUp.alpha = chargeEffectCurrent;
    }

    void Update()
    {
        UpdateChargeEffect();
    }

    private void UpdateChargeEffect()
    {
        chargeEffectCurrent = Mathf.Lerp(chargeEffectCurrent, chargeEffectTarget, chargeEffectSpeed * Time.deltaTime);
        if (chargeEffectCurrent < 0.01f && chargeEffectTarget == 0f) chargeEffectCurrent = 0f;
        ApplyChargeEffect();
    }

    public void ActivateChargeCam() => cameraCharge.Priority = 20;
    public void DeactivateChargeCam()
    {
        if (cameraBrain.ActiveBlend != null)
        {
            //cameraBase.ForceCameraPosition(cameraBrain.transform.position, cameraBrain.transform.rotation);
        }
        cameraCharge.Priority = -1;
    }

    public void SetScreenPowerUpAlpha(float alpha)
    {
        screenPowerUp.alpha = alpha;
    }

}
