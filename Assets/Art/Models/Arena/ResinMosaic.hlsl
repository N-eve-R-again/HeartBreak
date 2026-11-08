// =============================================================================
//  ResinMosaic.hlsl
//  Mosaïque polaire pour styliser les reflets (traits façon reflet "glitché").
//
//  Nœud Custom Function en mode "File" :  Name = ResinMosaic   Source = ce fichier
//
//  Idée : le sol est découpé en ANNEAUX autour du centre de l'arène (les
//  "bandes"), chaque anneau est découpé en ARCS (les "rectangles").
//  Tous les pixels d'un même arc lisent le reflet au MÊME endroit : le centre
//  de l'arc. Résultat : des aplats allongés, perpendiculaires à la vue quand la
//  caméra orbite autour de l'arène, et fixes dans le monde (rien ne glisse).
//
//  Sorties :
//      ScreenUV   : UV écran où lire la RT de reflet (remplace Screen Position)
//      ReflectDir : direction où lire la reflection probe
//      Random     : valeur aléatoire 0..1 propre à chaque arc (pour varier la lumi/teinte)
// =============================================================================

#ifndef RESIN_MOSAIC_INCLUDED
#define RESIN_MOSAIC_INCLUDED

#define RESIN_TWO_PI 6.28318530718

// Hash : transforme 2 nombres en un nombre "aléatoire" entre 0 et 1.
// Pas vraiment aléatoire : toujours le même résultat pour la même entrée,
// c'est ce qui rend le motif stable d'une image à l'autre.
float ResinHash21(float2 p)
{
    p = frac(p * float2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return frac(p.x * p.y);
}

// Position monde -> UV écran, avec la même convention que le nœud
// Screen Position en mode Default (0..1, y retourné selon la plateforme).
float2 ResinWorldToScreenUV(float3 positionWS)
{
    float4 cs = TransformWorldToHClip(positionWS);
    float2 ndc = float2(cs.x, cs.y * _ProjectionParams.x) / cs.w;
    return ndc * 0.5 + 0.5;
}

void ResinMosaic_float(float3 PositionWS, float3 CenterWS, float3 NormalWS, float3 GeomNormalWS,
                       float RingWidth, float ArcLength, float Jitter, float Strength,
                       out float2 ScreenUV, out float3 ReflectDir, out float Random, out float3 BlockPositionWS)
{
    // ---------------------------------------------------------------------
    // 1. Coordonnées polaires autour du centre de l'arène (sur le plan XZ)
    // ---------------------------------------------------------------------
    float2 d = PositionWS.xz - CenterWS.xz;
    float  r = length(d);
    float  angle01 = atan2(d.y, d.x) / RESIN_TWO_PI + 0.5;     // 0..1 sur le tour complet

    // ---------------------------------------------------------------------
    // 2. Anneaux de largeur variable
    //    Chaque anneau de base (RingWidth) est redécoupé en 1, 2 ou 3 bandes
    //    selon un hash : ça donne des traits fins et des traits épais.
    // ---------------------------------------------------------------------
    float ringWidth = max(RingWidth, 0.001);
    float rr    = r / ringWidth;
    float ring  = floor(rr);
    float nSub  = 1.0 + floor(ResinHash21(float2(ring, 7.1)) * 3.0);   // 1..3
    float sub   = floor(frac(rr) * nSub);
    float band  = ring * 4.0 + sub;                                      // id unique de la bande
    float rc    = (ring + (sub + 0.5) / nSub) * ringWidth;               // rayon du milieu de la bande

    // ---------------------------------------------------------------------
    // 3. Arcs dans la bande
    //    Longueur moyenne différente pour chaque bande, et un nombre ENTIER
    //    d'arcs sur le tour : pas de couture à l'endroit où l'angle repasse de 1 à 0.
    // ---------------------------------------------------------------------
    float lenScale = lerp(0.4, 1.8, ResinHash21(float2(band, 3.7)));
    float nSeg     = max(1.0, round(RESIN_TWO_PI * rc / max(ArcLength * lenScale, 0.001)));
    float offset   = ResinHash21(float2(band, 1.3));                     // décale les arcs d'une bande à l'autre
    float segF     = (angle01 + offset) * nSeg;
    float seg      = floor(segF);
    float segId    = seg - nSeg * floor(seg / nSeg);     // même id des deux côtés de la "couture" du tour
    // Second découpage : certains arcs sont coupés en 2 ou 3 morceaux plus courts
    float nPiece   = 1.0 + floor(ResinHash21(float2(band, segId)) * 3.0);
    float piece    = floor(frac(segF) * nPiece);
    float centerSeg = seg + (piece + 0.5) / nPiece;                      // centre du morceau, en "unités d'arc"
    float aCenter  = (centerSeg / nSeg - offset - 0.5) * RESIN_TWO_PI;

    Random = ResinHash21(float2(band * 13.0 + piece, segId));

    // ---------------------------------------------------------------------
    // 4. Point où on lit le reflet pour tout ce morceau d'arc
    //    + petit décalage le long de l'arc (Jitter) pour l'effet "glitch"
    // ---------------------------------------------------------------------
    float2 dirC    = float2(cos(aCenter), sin(aCenter));
    float2 tangent = float2(-dirC.y, dirC.x);
    float  shift   = (ResinHash21(float2(segId, band * 5.0)) - 0.5) * Jitter;
    float3 blockWS = float3(CenterWS.x + dirC.x * rc + tangent.x * shift,
                            PositionWS.y,
                            CenterWS.z + dirC.y * rc + tangent.y * shift);

    // ---------------------------------------------------------------------
    // 5. Anti-scintillement : quand les bandes deviennent plus fines que
    //    quelques pixels (au loin), on revient doucement au reflet normal.
    //    fwidth = combien la position change d'un pixel à l'autre.
    // ---------------------------------------------------------------------
    float pixelSize = max(length(fwidth(PositionWS.xz)), 1e-5);
    float fade = smoothstep(1.5, 4.0, (ringWidth / 3.0) / pixelSize);
    float m = saturate(Strength) * fade;

#if defined(SHADERGRAPH_PREVIEW)
    // Pas de caméra fiable dans les previews : on montre juste le motif.
    ScreenUV   = lerp(PositionWS.xz, blockWS.xz, m) * 0.1;
    ReflectDir = reflect(float3(0, -1, 0), normalize(NormalWS));
    BlockPositionWS = lerp(PositionWS, blockWS, m);
#else
    float3 camPos = GetCameraPositionWS();

    // Reflet "normal" du pixel (avec la normale détaillée)
    float2 pixelUV   = ResinWorldToScreenUV(PositionWS);
    float3 pixelRefl = reflect(normalize(PositionWS - camPos), normalize(NormalWS));

    // Reflet du bloc (normale géométrique : le bloc reste un aplat)
    float2 blockUV   = ResinWorldToScreenUV(blockWS);
    float3 blockRefl = reflect(normalize(blockWS - camPos), normalize(GeomNormalWS));

    ScreenUV   = lerp(pixelUV, blockUV, m);
    ReflectDir = normalize(lerp(pixelRefl, blockRefl, m));
    BlockPositionWS = lerp(PositionWS, blockWS, m);
    
#endif
}

#endif // RESIN_MOSAIC_INCLUDED
