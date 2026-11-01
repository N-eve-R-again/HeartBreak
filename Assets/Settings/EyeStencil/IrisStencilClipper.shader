Shader "Custom/IrisStencilClipper"
{
    Properties
    {
        [Header(Stencil)]
        [IntRange] _StencilRef ("Stencil Ref (1-15)", Range(1,15)) = 1

        [Header(Mask  garde la zone uv_x inferieur SplitX ET uv_y superieur SplitY)]
        _SplitX ("Split X", Range(0,1)) = 0.5
        _SplitY ("Split Y", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "IrisStencilWrite"
            // LightMode requis pour que la feature Render Objects ramasse ce pass
            Tags { "LightMode"="UniversalForward" }

            ColorMask 0      // n'ecrit aucune couleur (l'iris est deja peint par ton ShaderGraph)
            ZWrite Off       // ne touche pas la depth (l'iris l'a deja ecrite)
            ZTest LEqual     // stencil seulement la ou l'iris est REELLEMENT visible -> occlusion monde OK
            Cull Back        // mets Off ou Front si ton mesh a une autre orientation

            Stencil
            {
                Ref [_StencilRef]
                Comp Always
                Pass Replace
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            // SRP Batcher : toutes les proprietes materiau dans un seul UnityPerMaterial
            CBUFFER_START(UnityPerMaterial)
                float _StencilRef;
                float _SplitX;
                float _SplitY;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // bord NET : on garde le quadrant haut-gauche, le reste est discard
                float inside = (IN.uv.x < _SplitX && IN.uv.y > _SplitY) ? 1.0 : 0.0;
                clip(inside - 0.5);
                return 0;   // ColorMask 0 -> seul le stencil est ecrit
            }
            ENDHLSL
        }
    }
}
