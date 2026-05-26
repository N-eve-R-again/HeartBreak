using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerVFXLibrary : MonoBehaviour
{
    [Header("Dashing Charge")]
    public Volume chargeVolume;
    public ParticleSystem chargePS;
    public ParticleSystem chargeCompletePS;
    public CinemachineCamera cameraCharge;
    public CinemachineCamera cameraBase;
    public CinemachineBrain cameraBrain;
    public CanvasGroup screenPowerUp;

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
