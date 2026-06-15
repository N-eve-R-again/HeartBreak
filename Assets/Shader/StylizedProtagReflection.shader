Shader "Custom/StylizedProtagReflection"
{
    Properties
    {
        _ReflectionColor ("Teinte du reflet", Color) = (1, 0.3, 0.8, 1)
        _BandScale ("Echelle des bandes (anneaux/m)", Float) = 2.0
        _BandSharpness ("Dureté des bandes", Range(0,1)) = 0.6
        _FadeDistance ("Distance de fade (m depuis les pieds)", Float) = 4.0
        _FadeSteps ("Paliers du fade (0 = lisse)", Float) = 0.0
        _Opacity ("Opacité globale", Range(0,1)) = 0.8
        // Optionnel : ramener un peu des vraies couleurs du perso (0 = silhouette pure)
        _SourceColorMix ("Mix couleur source", Range(0,1)) = 0.0
        _Posterize ("Paliers couleur source", Float) = 4
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_ProtagReflectionTex);
            SAMPLER(sampler_ProtagReflectionTex);

            // Poussée en global par MirrorReflection.cs : position monde des pieds.
            float4 _ReflectionOrigin;

            CBUFFER_START(UnityPerMaterial)
                float4 _ReflectionColor;
                float  _BandScale;
                float  _BandSharpness;
                float  _FadeDistance;
                float  _FadeSteps;
                float  _Opacity;
                float  _SourceColorMix;
                float  _Posterize;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 screenPos   : TEXCOORD0;
                float3 worldPos    : TEXCOORD1; // position monde du fragment de SOL
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float3 wpos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.worldPos = wpos;
                OUT.positionHCS = TransformWorldToHClip(wpos);
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                // ---- Silhouette : on lit la RT en UV écran, mais on ne garde que la COUVERTURE.
                float2 uv = IN.screenPos.xy / IN.screenPos.w;
                float4 src = SAMPLE_TEXTURE2D(_ProtagReflectionTex, sampler_ProtagReflectionTex, uv);
                float coverage = src.a;
                if (coverage < 0.01) discard;

                // ---- L'ORIGINE : distance horizontale entre ce point de sol et les pieds.
                //      Tout part de là : bandes ET fade.
                float dist = distance(IN.worldPos.xz, _ReflectionOrigin.xz);

                // ---- Bandes = anneaux concentriques émanant des pieds.
                float band = frac(dist * _BandScale);
                float bandMask = smoothstep(_BandSharpness - 0.05, _BandSharpness + 0.05, band);
                // pour des bandes FRANCHES (plus géométrique) : remplace par step(_BandSharpness, band)

                // ---- Fade qui meurt en s'éloignant des pieds.
                float fade = 1.0 - saturate(dist / max(_FadeDistance, 0.0001));
                if (_FadeSteps > 0.5) fade = floor(fade * _FadeSteps) / _FadeSteps; // paliers durs

                // ---- Couleur : silhouette pure par défaut. Pas de couleurs source = pas de bouillie.
                float3 srcPost = floor(src.rgb * _Posterize) / max(_Posterize, 1.0);
                float3 col = lerp(_ReflectionColor.rgb, srcPost * _ReflectionColor.rgb, _SourceColorMix);

                float alpha = coverage * bandMask * fade * _Opacity;
                return float4(col, alpha);
            }
            ENDHLSL
        }
    }
}
