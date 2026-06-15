using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// À mettre sur un GameObject quelconque (ou sur ta caméra principale).
// Principe : on crée une caméra "miroir" qu'on ne déplace JAMAIS via son transform.
// On lui injecte une worldToCameraMatrix = vue_de_la_vraie_cam * matrice_de_réflexion.
// Elle ne rend qu'un seul layer (ta protagoniste) dans une RenderTexture.
// Le shader du sol lira ensuite cette RT en coordonnées écran.

[ExecuteAlways]
public class MirrorReflection : MonoBehaviour
{
    [Header("Références")]
    public Camera mainCamera;            // ta caméra de jeu (vue du dessus)
    public Transform groundPlane;        // un objet dont la hauteur Y = niveau du sol
    public LayerMask reflectionLayers;   // mets SEULEMENT le layer de la protagoniste

    [Header("Réglages")]
    public Transform reflectionOrigin;    // pieds de la protag : d'où partent les bandes ET le fade
    public float clipPlaneOffset = 0.01f; // petit offset pour éviter le z-fighting au ras du sol
    [Range(0.1f, 1f)] public float resolutionScale = 0.5f; // 0.5 = quart de la res écran (moitié/axe)

    private Camera reflectionCamera;
    private RenderTexture reflectionRT;

    // Nom global que le shader du sol ira chercher (_ProtagReflectionTex)
    private static readonly int ReflectionTexID = Shader.PropertyToID("_ProtagReflectionTex");
    private static readonly int ReflectionOriginID = Shader.PropertyToID("_ReflectionOrigin");

    void OnEnable()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        RenderPipelineManager.beginCameraRendering += OnBeginCamera;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
        Cleanup();
    }

    void Cleanup()
    {
        if (reflectionCamera != null) DestroyImmediate(reflectionCamera.gameObject);
        if (reflectionRT != null) { reflectionRT.Release(); DestroyImmediate(reflectionRT); }
    }

    void OnBeginCamera(ScriptableRenderContext context, Camera cam)
    {
        // On ne réagit qu'au rendu de la vraie caméra (sinon récursion infinie).
        if (cam != mainCamera) return;
        if (groundPlane == null) return;

        EnsureResources();

        // 1) Le plan du sol, en (normale, distance) : ici normale = up, d = -hauteurSol.
        Vector3 normal = Vector3.up;
        float groundY = groundPlane.position.y;
        Vector3 pointOnPlane = new Vector3(0f, groundY, 0f);
        float d = -Vector3.Dot(normal, pointOnPlane) - clipPlaneOffset;
        Vector4 plane = new Vector4(normal.x, normal.y, normal.z, d);

        // 2) La matrice de réflexion (déterminant négatif = vrai miroir).
        Matrix4x4 reflection = CalculateReflectionMatrix(plane);

        // 3) On copie les paramètres de la vraie cam, MAIS on remplace la vue par vue * réflexion.
        //    >>> C'est ici que la magie opère. On ne bouge pas le transform. <<<
        reflectionCamera.CopyFrom(mainCamera);
        reflectionCamera.cullingMask = reflectionLayers;
        reflectionCamera.worldToCameraMatrix = mainCamera.worldToCameraMatrix * reflection;

        // 4) Near plane oblique aligné sur le sol : tout ce qui dépasse au-dessus du sol
        //    (la "tête" du reflet qui remonterait dans les airs) est clippé.
        Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pointOnPlane, normal, 1f);
        reflectionCamera.projectionMatrix = mainCamera.CalculateObliqueMatrix(clipPlane);

        // Fond transparent : la RT ne contient que la silhouette, le reste = alpha 0.
        reflectionCamera.clearFlags = CameraClearFlags.SolidColor;
        reflectionCamera.backgroundColor = new Color(0, 0, 0, 0);
        reflectionCamera.targetTexture = reflectionRT;

        // 5) Le déterminant négatif inverse le sens des faces -> on inverse le culling
        //    le temps du rendu, sinon la silhouette est "retournée".
        GL.invertCulling = true;

#if UNITY_2022_1_OR_NEWER
        // Unity 6 / URP récent : RenderRequest est la voie propre.
        var request = new UniversalRenderPipeline.SingleCameraRequest();
        if (RenderPipeline.SupportsRenderRequest(reflectionCamera, request))
        {
            request.destination = reflectionRT;
            RenderPipeline.SubmitRenderRequest(reflectionCamera, request);
        }
        else
        {
            // Fallback (déprécié à terme mais fonctionne sur URP 12-16) :
            UniversalRenderPipeline.RenderSingleCamera(context, reflectionCamera);
        }
#else
        UniversalRenderPipeline.RenderSingleCamera(context, reflectionCamera);
#endif

        GL.invertCulling = false;

        // 6) On rend la RT visible globalement pour le shader du sol.
        Shader.SetGlobalTexture(ReflectionTexID, reflectionRT);

        // Origine du reflet (pieds de la protag). Le shader fait partir les bandes
        // et le fade de CE point monde, au lieu d'un axe écran arbitraire.
        Vector3 origin = reflectionOrigin != null ? reflectionOrigin.position : pointOnPlane;
        Shader.SetGlobalVector(ReflectionOriginID, origin);
    }

    void EnsureResources()
    {
        int w = Mathf.Max(8, (int)(mainCamera.pixelWidth * resolutionScale));
        int h = Mathf.Max(8, (int)(mainCamera.pixelHeight * resolutionScale));

        if (reflectionRT == null || reflectionRT.width != w || reflectionRT.height != h)
        {
            if (reflectionRT != null) reflectionRT.Release();
            reflectionRT = new RenderTexture(w, h, 16, RenderTextureFormat.ARGB32);
            reflectionRT.name = "ProtagReflectionRT";
            reflectionRT.useMipMap = true;
            reflectionRT.autoGenerateMips = true;
            reflectionRT.filterMode = FilterMode.Trilinear;
        }

        if (reflectionCamera == null)
        {
            var go = new GameObject("ReflectionCamera (auto)");
            go.hideFlags = HideFlags.HideAndDontSave;
            reflectionCamera = go.AddComponent<Camera>();
            reflectionCamera.enabled = false; // on la pilote à la main, elle ne rend pas toute seule
        }
    }

    // Matrice qui réfléchit l'espace monde par rapport au plan (n.x, n.y, n.z, d).
    static Matrix4x4 CalculateReflectionMatrix(Vector4 p)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m.m00 = 1f - 2f * p.x * p.x; m.m01 = -2f * p.x * p.y; m.m02 = -2f * p.x * p.z; m.m03 = -2f * p.w * p.x;
        m.m10 = -2f * p.y * p.x; m.m11 = 1f - 2f * p.y * p.y; m.m12 = -2f * p.y * p.z; m.m13 = -2f * p.w * p.y;
        m.m20 = -2f * p.z * p.x; m.m21 = -2f * p.z * p.y; m.m22 = 1f - 2f * p.z * p.z; m.m23 = -2f * p.w * p.z;
        m.m30 = 0f; m.m31 = 0f; m.m32 = 0f; m.m33 = 1f;
        return m;
    }

    // Exprime le plan du sol dans l'espace caméra (pour le near plane oblique).
    static Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(pos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }
}