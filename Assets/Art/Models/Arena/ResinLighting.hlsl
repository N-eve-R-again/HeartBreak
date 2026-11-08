// =============================================================================
//  ResinLighting.hlsl
//  Éclairage "résine + paillettes" pour Shader Graph (target Unlit, URP).
//
//  Utilisé par un nœud Custom Function en mode "File" :
//      Name   : ResinLighting        (Shader Graph ajoute lui-même "_float")
//      Source : ce fichier
//
//  Ce que la fonction calcule, pour la lumière principale ET toutes les
//  lumières additionnelles, en UNE seule boucle :
//      Diffuse  : la lumière reçue par la résine (Lambert)
//      Specular : le reflet net d'une surface vernie (GGX, comme URP)
//      Sparkle  : un reflet plus large, qui sert à faire briller les paillettes
//
//  Tout le reste (couleurs, irisation, reflets, paillettes) est fait en nœuds.
// =============================================================================

// Garde d'inclusion : si deux nœuds Custom Function utilisent ce fichier,
// Unity ne l'inclut qu'une fois. Sans ça : erreur "function redefinition".
#ifndef RESIN_LIGHTING_INCLUDED
#define RESIN_LIGHTING_INCLUDED

// Unity 6.0 appelle le Forward+ "USE_FORWARD_PLUS", Unity 6.1+ "USE_CLUSTER_LIGHT_LOOP".
// On crée notre propre nom pour que le fichier marche avec les deux.
#if defined(USE_CLUSTER_LIGHT_LOOP)
    #define RESIN_CLUSTERED USE_CLUSTER_LIGHT_LOOP
#elif defined(USE_FORWARD_PLUS)
    #define RESIN_CLUSTERED USE_FORWARD_PLUS
#else
    #define RESIN_CLUSTERED 0
#endif

// Les vraies lumières n'existent que si URP a inclus Lighting.hlsl, ce qui est le
// cas dans la passe Forward d'un Unlit avec "Keep Lighting Variants" coché.
// Dans les previews de Shader Graph, on utilise une fausse lumière à la place.
#if defined(UNIVERSAL_LIGHTING_INCLUDED) && !defined(SHADERGRAPH_PREVIEW)
    #define RESIN_REAL_LIGHTING 1
#else
    #define RESIN_REAL_LIGHTING 0
#endif

// -----------------------------------------------------------------------------
//  Petites fonctions utilitaires
// -----------------------------------------------------------------------------

// Fresnel de Schlick : quelle part de la lumière est réfléchie selon l'angle.
// F0 = réflexion quand on regarde pile en face.
// 0.04 = valeur des diélectriques (verre, résine, plastique, eau...).
// C'est ce terme qui donne des reflets faibles de face et forts en rasant.
float ResinFresnel(float cosTheta)
{
    const float F0 = 0.04;
    float m = 1.0 - cosTheta;
    float m2 = m * m;
    return F0 + (1.0 - F0) * (m2 * m2 * m);   // m^5 sans appeler pow()
}

// Spéculaire GGX, même approximation que URP (DirectBRDFSpecular dans BRDF.hlsl).
// "roughness" = rugosité au carré (ce que URP appelle roughness),
// plus elle est petite, plus le reflet est petit et intense.
float ResinGGX(float NoH, float LoH, float roughness)
{
    float r2 = roughness * roughness;
    float d  = NoH * NoH * (r2 - 1.0) + 1.00001;          // dénominateur de la NDF
    float LoH2 = max(0.1, LoH * LoH);                      // évite la division par 0
    float normalization = roughness * 4.0 + 2.0;
    return r2 / ((d * d) * LoH2 * normalization);
}

#if RESIN_REAL_LIGHTING
// Contribution d'UNE lumière. On l'appelle pour la lumière principale puis
// pour chaque lumière additionnelle, et on additionne.
// "inout" = la fonction lit ET modifie ces variables (comme ref en C#).
void ResinAccumulateLight(Light light, float3 N, float3 V, float roughness,
                          float sparkleSharpness,
                          inout float3 diffuse, inout float3 specular, inout float3 sparkle)
{
    float3 L = light.direction;                    // vers la lumière
    float3 H = normalize(L + V);                   // "half vector" entre lumière et vue

    float NoL = saturate(dot(N, L));
    float NoH = saturate(dot(N, H));
    float LoH = saturate(dot(L, H));

    // Atténuation : distance (lumières ponctuelles/spots) x ombres
    float3 radiance = light.color * (light.distanceAttenuation * light.shadowAttenuation * NoL);

    diffuse  += radiance;
    specular += radiance * ResinGGX(NoH, LoH, roughness) * ResinFresnel(LoH);

    // Lobe large pour les paillettes : un simple Blinn-Phong.
    // Plus "sparkleSharpness" est grand, plus il faut être bien aligné pour que ça brille.
    sparkle  += radiance * pow(NoH, sparkleSharpness);
}
#endif

// -----------------------------------------------------------------------------
//  Fonction appelée par Shader Graph
//  Les paramètres "out" deviennent les sorties du nœud, les autres ses entrées.
//  Le suffixe _float correspond à la précision du graphe (Single).
// -----------------------------------------------------------------------------
void ResinLighting_float(float3 PositionWS, float3 NormalWS, float3 ViewWS,
                         float2 ScreenUV, float Smoothness, float SparkleSharpness,
                         out float3 Diffuse, out float3 Specular, out float3 Sparkle)
{
    Diffuse  = 0;
    Specular = 0;
    Sparkle  = 0;

#if !RESIN_REAL_LIGHTING
    // Dans les petites previews de Shader Graph il n'y a ni lumières ni ombres :
    // on simule une lumière blanche pour voir quelque chose.
    // Si tu vois cet éclairage-là EN JEU, c'est que "Keep Lighting Variants"
    // n'est pas coché dans le Graph Inspector.
    float3 L = normalize(float3(-0.5, 0.8, -0.3));
    float3 H = normalize(L + ViewWS);
    float  NoL = saturate(dot(NormalWS, L));
    Diffuse  = NoL;
    Specular = pow(saturate(dot(NormalWS, H)), 200.0) * NoL;
    Sparkle  = pow(saturate(dot(NormalWS, H)), SparkleSharpness) * NoL;
#else
    float3 N = normalize(NormalWS);
    float3 V = normalize(ViewWS);

    // Smoothness -> rugosité, exactement comme URP
    float perceptualRoughness = 1.0 - Smoothness;
    float roughness = max(perceptualRoughness * perceptualRoughness, 0.002);

    // Rendering Layers : une lumière peut être limitée à certains calques.
    uint meshRenderingLayers = GetMeshRenderingLayer();

    // ---- Lumière principale (avec ses ombres en cascade) ----
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    Light mainLight = GetMainLight(shadowCoord);
    #ifdef _LIGHT_LAYERS
    if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
    #endif
    {
        ResinAccumulateLight(mainLight, N, V, roughness, SparkleSharpness, Diffuse, Specular, Sparkle);
    }

    // ---- Lumières additionnelles ----
    // shadowMask = 1 : pas de shadowmask baké, on garde les ombres temps réel.
    // Cette version de GetAdditionalLight calcule aussi les ombres et les cookies.
    half4 shadowMask = half4(1, 1, 1, 1);

    // En Forward+ (cluster), URP range les lumières directionnelles
    // additionnelles à part : il faut les parcourir avant la boucle principale.
    #if RESIN_CLUSTERED
    [loop] for (uint dirIndex = 0; dirIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); dirIndex++)
    {
        Light dirLight = GetAdditionalLight(dirIndex, PositionWS, shadowMask);
        #ifdef _LIGHT_LAYERS
        if (IsMatchingLightLayer(dirLight.layerMask, meshRenderingLayers))
        #endif
        {
            ResinAccumulateLight(dirLight, N, V, roughness, SparkleSharpness, Diffuse, Specular, Sparkle);
        }
    }
    #endif

    // En Forward+, la macro LIGHT_LOOP_BEGIN a besoin de "inputData" pour savoir
    // dans quelle case de l'écran on est. En Forward classique elle l'ignore.
    InputData inputData = (InputData)0;
    inputData.normalizedScreenSpaceUV = ScreenUV;
    inputData.positionWS = PositionWS;

    // LIGHT_LOOP_BEGIN déclare elle-même la variable "lightIndex".
    uint pixelLightCount = GetAdditionalLightsCount();
    LIGHT_LOOP_BEGIN(pixelLightCount)
        Light light = GetAdditionalLight(lightIndex, PositionWS, shadowMask);
        #ifdef _LIGHT_LAYERS
        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        #endif
        {
            ResinAccumulateLight(light, N, V, roughness, SparkleSharpness, Diffuse, Specular, Sparkle);
        }
    LIGHT_LOOP_END
#endif
}

#endif // RESIN_LIGHTING_INCLUDED
