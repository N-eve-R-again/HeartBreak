using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ColoredHeadersData", menuName = "Editor/Colored Headers Data")]
public class COLOREDHEADERS_SO : ScriptableObject
{
    [Serializable]
    public class ColoredTitle
    {
        public string name;
        public Color color = Color.white;
    }

    [Header("Filtre")]
    [Tooltip("Seuls les objets dont le nom commence par ce préfixe sont colorés. Laisser vide pour tous les objets.")]
    public string prefix = "";

    [Header("Affichage")]
    [Tooltip("Espace entre le début de la colonne Name et le début du dessin, en pixels.")]
    [FormerlySerializedAs("markerLeft")]
    [Min(0f)] public float paddingLeft = 0f;

    [Tooltip("Espace entre le bord droit de la ligne et la fin du dessin, en pixels.")]
    [FormerlySerializedAs("afterTextPaddingRight")]
    [Min(0f)] public float paddingRight = 0f;

    [Tooltip("Espace entre le texte et la ligne, en pixels.")]
    [Min(0f)] public float textGap = 6f;

    [Tooltip("Raccourcit à droite les titres colorés par rapport aux composants Header, en pixels.")]
    [Min(0f)] public float titlePadding = 0f;

    [Tooltip("Épaisseur des lignes, centrées verticalement, en pixels.")]
    [FormerlySerializedAs("headerLineThickness")]
    [Min(1f)] public float thickness = 2f;

    [Tooltip("Largeur minimale du marqueur, en pixels.")]
    [Min(0f)] public float minMarkerWidth = 10f;

    [Tooltip("Longueur minimale de la bande du nom le plus long, en pixels.")]
    [Min(0f)] public float minTailWidth = 20f;

    [Tooltip("Opacité appliquée à la couleur.")]
    [Range(0f, 1f)] public float opacity = 1f;

    [Header("Composants Header (BennyKok)")]
    [Tooltip("Remplace les caractères du package par un titre centré et de vraies lignes.")]
    public bool styleHeaderComponents = true;

    [Tooltip("Largeur totale du header, en pixels. 0 = toute la largeur disponible.")]
    [Min(0f)] public float headerWidth = 0f;

    [Tooltip("Couleur des headers dont le titre ne correspond à aucune catégorie.")]
    public Color defaultHeaderColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    [Tooltip("Utilise la couleur de la ligne pour le titre.")]
    public bool tintHeaderTitle = true;

    [Tooltip("Couleur du titre si tintHeaderTitle est désactivé. Son alpha est multiplié par celui de la ligne.")]
    public Color headerTitleColor = Color.white;

    public bool headerUppercase = true;
    public bool hideHeaderIcon = true;

    [Header("Catégories")]
    public List<ColoredTitle> coloredTitles = new List<ColoredTitle>();

    public Color GetHeaderColor(string headerTitle)
    {
        Color color = defaultHeaderColor;

        if (!string.IsNullOrEmpty(headerTitle))
        {
            foreach (ColoredTitle title in coloredTitles)
            {
                if (title == null || string.IsNullOrEmpty(title.name))
                    continue;

                if (headerTitle.IndexOf(title.name, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    color = title.color;
                    break;
                }
            }
        }

        color.a *= opacity;
        return color;
    }

    public bool TryGetColor(string objectName, out Color color)
    {
        color = default;

        if (string.IsNullOrEmpty(objectName))
            return false;

        if (!string.IsNullOrEmpty(prefix) && !objectName.StartsWith(prefix, StringComparison.Ordinal))
            return false;

        foreach (ColoredTitle title in coloredTitles)
        {
            if (title == null || string.IsNullOrEmpty(title.name))
                continue;

            if (objectName.Contains(title.name))
            {
                color = title.color;
                color.a *= opacity;
                return true;
            }
        }

        return false;
    }
}