#if UNITY_EDITOR && UNITY_6000_5_OR_NEWER
using System.Collections.Generic;
using Unity.Hierarchy;
using Unity.Hierarchy.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class HierarchyColorMarkers
{
    const string k_AssetPath = "Assets/Editor/COLOREDHEADERS/ColoredHeadersData.asset";
    const string k_MarkerName = "colored-header-marker";
    const string k_TailName = "colored-header-tail";
    const string k_HeaderOverlayName = "colored-header-overlay";
    const string k_HeaderContentName = "colored-header-content";
    const string k_HeaderTitleName = "colored-header-title";
    const string k_HeaderLeftLineName = "colored-header-line-left";
    const string k_HeaderRightLineName = "colored-header-line-right";

    static readonly HashSet<HierarchyView> s_PendingTailUpdates = new HashSet<HierarchyView>();
    static readonly List<(VisualElement tail, float left)> s_VisibleTails = new List<(VisualElement, float)>();

    static COLOREDHEADERS_SO colorTitlesData;

    [InitializeOnLoadMethod]
    static void Initialize()
    {
        HierarchyWindow.BindViewItem += OnBindViewItem;
    }

    static void OnBindViewItem(HierarchyWindow window, HierarchyView view, HierarchyViewItem item)
    {
        Refresh(item);
    }

    static void Refresh(HierarchyViewItem item)
    {
        VisualElement marker = GetOrCreateElement(item, k_MarkerName);
        VisualElement tail = GetOrCreateTail(item);
        VisualElement overlay = GetOrCreateHeaderOverlay(item);

        // Les lignes sont recyclées : on repart toujours d'un état neutre.
        marker.style.display = DisplayStyle.None;
        tail.style.display = DisplayStyle.None;
        overlay.style.display = DisplayStyle.None;
        item.Name.style.visibility = StyleKeyword.Null;
        item.Icon.style.visibility = StyleKeyword.Null;

        if (item.Handler is not HierarchyGameObjectHandler handler)
            return;

        GameObject gameObject = handler.GetGameObject(item.Node);
        if (gameObject == null)
            return;

        if (colorTitlesData == null)
            colorTitlesData = AssetDatabase.LoadAssetAtPath<COLOREDHEADERS_SO>(k_AssetPath);
        if (colorTitlesData == null)
            return;

        if (colorTitlesData.styleHeaderComponents
            && gameObject.TryGetComponent(out BK.HierarchyHeader.Header header))
        {
            DrawHeader(item, overlay, header.title, colorTitlesData);
            item.schedule.Execute(() => UpdatePositions(item));
            return;
        }

        if (!colorTitlesData.TryGetColor(gameObject.name, out Color color))
            return;

        ShowLine(marker, color, colorTitlesData.thickness);
        ShowLine(tail, color, colorTitlesData.thickness);

        // Le texte et sa position ne sont pas forcément prêts pendant le bind.
        item.schedule.Execute(() => UpdatePositions(item));
    }

    static void DrawHeader(HierarchyViewItem item, VisualElement overlay, string title, COLOREDHEADERS_SO data)
    {
        Color color = data.GetHeaderColor(title);
        bool hasTitle = !string.IsNullOrEmpty(title);

        // Le nom du GameObject reste intact : il est seulement masqué à l'affichage.
        item.Name.style.visibility = Visibility.Hidden;
        if (data.hideHeaderIcon)
            item.Icon.style.visibility = Visibility.Hidden;

        // Le bord gauche est calculé dans UpdatePositions.
        overlay.style.right = data.paddingRight;

        VisualElement content = overlay.Q(k_HeaderContentName);
        if (data.headerWidth > 0f)
            content.style.width = data.headerWidth;
        else
            content.style.width = Length.Percent(100);

        Label label = overlay.Q<Label>(k_HeaderTitleName);
        label.text = hasTitle && data.headerUppercase ? title.ToUpperInvariant() : title;
        label.style.color = GetTitleColor(color, data);
        label.style.marginLeft = data.textGap;
        label.style.marginRight = data.textGap;
        label.style.display = hasTitle ? DisplayStyle.Flex : DisplayStyle.None;

        StyleHeaderLine(overlay.Q(k_HeaderLeftLineName), color, data.thickness);
        StyleHeaderLine(overlay.Q(k_HeaderRightLineName), color, data.thickness);

        overlay.style.display = DisplayStyle.Flex;
        overlay.BringToFront();
    }

    static Color GetTitleColor(Color lineColor, COLOREDHEADERS_SO data)
    {
        if (data.tintHeaderTitle)
            return lineColor;

        // L'alpha de la ligne contient déjà l'opacité et l'alpha de la catégorie.
        Color titleColor = data.headerTitleColor;
        titleColor.a *= lineColor.a;
        return titleColor;
    }

    static void StyleHeaderLine(VisualElement line, Color color, float thickness)
    {
        line.style.height = thickness;
        line.style.backgroundColor = color;
    }

    static VisualElement GetOrCreateElement(HierarchyViewItem item, string elementName)
    {
        VisualElement element = item.RowContainer.Q(elementName);
        if (element != null)
            return element;

        element = new VisualElement { name = elementName, pickingMode = PickingMode.Ignore };
        element.style.position = Position.Absolute;

        item.RowContainer.Add(element);
        return element;
    }

    static VisualElement GetOrCreateTail(HierarchyViewItem item)
    {
        VisualElement tail = item.RowContainer.Q(k_TailName);
        if (tail != null)
            return tail;

        tail = GetOrCreateElement(item, k_TailName);

        // Enregistrés une seule fois, à la création de la ligne recyclée.
        item.Name.RegisterCallback<GeometryChangedEvent>(_ => UpdatePositions(item));
        item.RowContainer.RegisterCallback<GeometryChangedEvent>(_ => UpdatePositions(item));

        // Un renommage ne relance pas forcément le bind. On attend un tick pour que
        // le titre du composant Header soit bien à jour.
        item.Name.RegisterCallback<ChangeEvent<string>>(_ => item.schedule.Execute(() => Refresh(item)));

        return tail;
    }

    static VisualElement GetOrCreateHeaderOverlay(HierarchyViewItem item)
    {
        VisualElement overlay = item.RowContainer.Q(k_HeaderOverlayName);
        if (overlay != null)
            return overlay;

        overlay = GetOrCreateElement(item, k_HeaderOverlayName);
        overlay.style.top = 0;
        overlay.style.bottom = 0;
        overlay.style.flexDirection = FlexDirection.Row;

        // Le contenu ne dépasse jamais de la ligne si la fenêtre est étroite.
        var content = new VisualElement { name = k_HeaderContentName, pickingMode = PickingMode.Ignore };
        content.style.flexDirection = FlexDirection.Row;
        content.style.alignItems = Align.Center;
        content.style.maxWidth = Length.Percent(100);
        content.style.flexShrink = 1;
        overlay.Add(content);

        content.Add(CreateHeaderLine(k_HeaderLeftLineName));

        var label = new Label { name = k_HeaderTitleName, pickingMode = PickingMode.Ignore };
        label.style.flexShrink = 1;
        label.style.minWidth = 0;
        label.style.paddingLeft = 0;
        label.style.paddingRight = 0;
        label.style.overflow = Overflow.Hidden;
        label.style.textOverflow = TextOverflow.Ellipsis;
        label.style.whiteSpace = WhiteSpace.NoWrap;
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        content.Add(label);

        content.Add(CreateHeaderLine(k_HeaderRightLineName));

        return overlay;
    }

    static VisualElement CreateHeaderLine(string elementName)
    {
        // Les deux lignes se partagent l'espace restant : le titre reste centré.
        var line = new VisualElement { name = elementName, pickingMode = PickingMode.Ignore };
        line.style.flexGrow = 1;
        line.style.flexBasis = 0;
        return line;
    }

    static void UpdatePositions(HierarchyViewItem item)
    {
        if (colorTitlesData == null)
            return;

        float left = GetNameColumnLeft(item) + colorTitlesData.paddingLeft;

        VisualElement overlay = item.RowContainer.Q(k_HeaderOverlayName);
        if (overlay != null && overlay.style.display.value != DisplayStyle.None)
            overlay.style.left = left;

        VisualElement marker = item.RowContainer.Q(k_MarkerName);
        if (marker != null && marker.style.display.value != DisplayStyle.None)
        {
            marker.style.left = left;
            UpdateMarkerWidth(item, marker, left);
        }

        RequestTailsUpdate(item.View);
    }

    static float GetNameColumnLeft(HierarchyViewItem item)
    {
        // L'élément de la ligne démarre dans la colonne Name, après Visibility et Picking.
        float left = item.ChangeCoordinatesTo(item.RowContainer, Vector2.zero).x;
        return float.IsNaN(left) ? 0f : Mathf.Max(0f, left);
    }

    static void UpdateMarkerWidth(HierarchyViewItem item, VisualElement marker, float left)
    {
        // Sans enfant, il n'y a pas de flèche : le marqueur va jusqu'à l'icône.
        VisualElement anchor = HasChildren(item) ? item.Toggle : item.Icon;

        float arrowLeft = anchor.ChangeCoordinatesTo(item.RowContainer, Vector2.zero).x;
        if (float.IsNaN(arrowLeft))
            return;

        float width = arrowLeft - colorTitlesData.textGap * 0.5f - left;
        marker.style.width = Mathf.Max(colorTitlesData.minMarkerWidth, width);
    }

    static bool HasChildren(HierarchyViewItem item)
    {
        if (item.Handler is not HierarchyGameObjectHandler handler)
            return false;

        GameObject gameObject = handler.GetGameObject(item.Node);
        return gameObject != null && gameObject.transform.childCount > 0;
    }

    static void RequestTailsUpdate(HierarchyView view)
    {
        // Plusieurs lignes peuvent changer d'un coup : un seul calcul par tick suffit.
        if (view == null || !s_PendingTailUpdates.Add(view))
            return;

        view.schedule.Execute(() =>
        {
            s_PendingTailUpdates.Remove(view);
            UpdateTails(view);
        });
    }

    static void UpdateTails(HierarchyView view)
    {
        if (colorTitlesData == null)
            return;

        float right = colorTitlesData.paddingRight + colorTitlesData.titlePadding;
        float sharedEnd = float.MinValue;

        s_VisibleTails.Clear();

        view.Query<HierarchyViewItem>().ForEach(item =>
        {
            VisualElement tail = item.RowContainer.Q(k_TailName);
            if (tail == null || tail.style.display.value == DisplayStyle.None)
                return;

            float rowWidth = item.RowContainer.layout.width;
            if (float.IsNaN(rowWidth) || !TryGetTailLeft(item, out float left))
                return;

            // Le point d'arrêt commun : le bord droit, ou la fin du nom le plus long.
            sharedEnd = Mathf.Max(sharedEnd, rowWidth - right, left + colorTitlesData.minTailWidth);
            s_VisibleTails.Add((tail, left));
        });

        foreach ((VisualElement tail, float left) in s_VisibleTails)
        {
            tail.style.left = left;
            tail.style.width = sharedEnd - left;
        }

        s_VisibleTails.Clear();
    }

    static bool TryGetTailLeft(HierarchyViewItem item, out float left)
    {
        left = 0f;

        Label label = item.Name;
        if (string.IsNullOrEmpty(label.text))
            return false;

        // On mesure le texte : le Label peut être plus large que son contenu.
        float textWidth = label.MeasureTextSize(
            label.text,
            0f, VisualElement.MeasureMode.Undefined,
            0f, VisualElement.MeasureMode.Undefined).x;

        Vector2 textEnd = label.ChangeCoordinatesTo(
            item.RowContainer,
            new Vector2(label.contentRect.x + textWidth, 0f));

        if (float.IsNaN(textEnd.x))
            return false;

        left = textEnd.x + colorTitlesData.textGap;
        return true;
    }

    static void ShowLine(VisualElement line, Color color, float thickness)
    {
        // Trait centré verticalement dans la ligne.
        line.style.top = Length.Percent(50);
        line.style.height = thickness;
        line.style.marginTop = -thickness * 0.5f;
        line.style.backgroundColor = color;
        line.style.display = DisplayStyle.Flex;

        // Premier enfant de la ligne : dessiné sous la flèche, l'icône et le nom.
        line.SendToBack();
    }
}
#endif