using UnityEditor;
using UnityEngine;
namespace MornHierarchy {
    public static class EditorHierarchyOnGUI {
        [InitializeOnLoadMethod]
        private static void AddHierarchyItemOnGUI() {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }
        private const float IconColumnX = 32f;
        private const float IconColumnWidth = 16f;
        private const float IconSize = 12f;
        private static void HierarchyWindowItemOnGUI(int instanceID,Rect selectionRect) {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if(gameObject == null) return;
            var isLine = gameObject.TryGetComponent<MornHierarchy>(out var hi) && hi.IsLine;
            EraseNativeText(instanceID,selectionRect);
            DrawColor(selectionRect,gameObject);
            if(isLine) DrawLineDecoration(selectionRect);
            DrawLabel(selectionRect,gameObject,isLine);
            DrawIcon(selectionRect,gameObject);
        }
        private static void DrawLineDecoration(Rect selectionRect) {
            var lineRect = selectionRect;
            lineRect.xMin = 0;
            lineRect.xMax = Mathf.Max(lineRect.xMax,EditorGUIUtility.currentViewWidth);
            var upper = lineRect;
            upper.yMax = upper.yMin + 2;
            EditorGUI.DrawRect(upper,Color.black);
            var lower = lineRect;
            lower.yMin = lower.yMax - 2;
            EditorGUI.DrawRect(lower,Color.black);
        }
        private static void DrawIcon(Rect selectionRect,GameObject gameObject) {
            if(gameObject.TryGetComponent<MornHierarchy>(out var hi) == false) return;
            if(hi.Icon == null) return;
            var icon = hi.Icon;
            var rect = new Rect(
                IconColumnX + (IconColumnWidth - IconSize) * 0.5f,
                selectionRect.y + (selectionRect.height - IconSize) * 0.5f,
                IconSize,
                IconSize);
            GUI.DrawTexture(rect,icon,ScaleMode.ScaleToFit);
        }
        private static void EraseNativeText(int instanceID,Rect selectionRect) {
            var rect = selectionRect;
            rect.xMin += 18;
            EditorGUI.DrawRect(rect,GetRowBackgroundColor(instanceID,rect));
        }
        private static Color GetRowBackgroundColor(int instanceID,Rect rect) {
            var isFocused = EditorWindow.focusedWindow != null
                && EditorWindow.focusedWindow.GetType().Name == "SceneHierarchyWindow";
            if(Selection.Contains(instanceID)) {
                return isFocused ? new Color32(44,93,134,255) : new Color32(77,77,77,255);
            }
            if(rect.Contains(Event.current.mousePosition)) return new Color32(68,68,68,255);
            return new Color32(56,56,56,255);
        }
        private static void DrawColor(Rect selectionRect,GameObject gameObject) {
            var fullRect = selectionRect;
            fullRect.xMin = 0;
            fullRect.xMax = Mathf.Max(selectionRect.xMax,EditorGUIUtility.currentViewWidth);
            if(gameObject.TryGetComponent<MornHierarchy>(out var ownColor) && ownColor.BackColor != MornHierarchyColor.None) {
                DrawTransparentRect(fullRect,ToColor(ownColor.BackColor));
                return;
            }
            var mornHiArray = gameObject.GetComponentsInParent<MornHierarchy>(true);
            if(mornHiArray == null) return;
            foreach(var hi in mornHiArray) {
                if(hi.transform == gameObject.transform) continue;
                if(hi.BackColor == MornHierarchyColor.None) continue;
                if(hi.ApplyChildren == false) return;
                var kFront = GetKRecursion(hi.transform,gameObject.transform);
                DrawTransparentRect(fullRect,ToColor(hi.BackColor) * kFront);
                return;
            }
        }
        private static void DrawTransparentRect(Rect rect,Color color) {
            color.a = 0.3f;
            EditorGUI.DrawRect(rect,color);
        }
        public static Color ToColor(MornHierarchyColor c) {
            switch(c) {
                case MornHierarchyColor.Red:     return new Color(0.95f,0.30f,0.30f);
                case MornHierarchyColor.Orange:  return new Color(1.00f,0.55f,0.20f);
                case MornHierarchyColor.Yellow:  return new Color(1.00f,0.85f,0.20f);
                case MornHierarchyColor.Lime:    return new Color(0.65f,0.95f,0.30f);
                case MornHierarchyColor.Green:   return new Color(0.30f,0.80f,0.40f);
                case MornHierarchyColor.Mint:    return new Color(0.40f,0.85f,0.70f);
                case MornHierarchyColor.Cyan:    return new Color(0.30f,0.85f,0.95f);
                case MornHierarchyColor.Blue:    return new Color(0.30f,0.55f,1.00f);
                case MornHierarchyColor.Indigo:  return new Color(0.45f,0.40f,0.85f);
                case MornHierarchyColor.Purple:  return new Color(0.65f,0.40f,0.85f);
                case MornHierarchyColor.Magenta: return new Color(0.90f,0.40f,0.90f);
                case MornHierarchyColor.Pink:    return new Color(1.00f,0.55f,0.75f);
                case MornHierarchyColor.Brown:   return new Color(0.65f,0.45f,0.30f);
                case MornHierarchyColor.Gray:    return new Color(0.65f,0.65f,0.65f);
                case MornHierarchyColor.White:   return new Color(0.95f,0.95f,0.95f);
                case MornHierarchyColor.Black:   return new Color(0.15f,0.15f,0.15f);
                case MornHierarchyColor.None:    return Color.clear;
            }
            return Color.clear;
        }
        private static float GetKRecursion(Transform aim,Transform own) {
            if(aim == own) return 1f;
            const int offset = 2;
            var pare = own.parent;
            return Mathf.InverseLerp(pare.childCount + offset,-1,own.GetSiblingIndex()) * GetKRecursion(aim,pare);
        }
        private static void DrawLabel(Rect selectionRect,GameObject gameObject,bool centered) {
            var rect = selectionRect;
            if(centered) {
                rect.xMin = Mathf.Max(rect.xMin,32);
                rect.xMax = Mathf.Max(rect.xMax,EditorGUIUtility.currentViewWidth);
            } else {
                rect.xMin += 18;
            }
            var style = new GUIStyle(EditorStyles.label);
            style.alignment = centered ? TextAnchor.MiddleCenter : TextAnchor.MiddleLeft;
            style.normal.textColor = gameObject.activeInHierarchy ? EditorStyles.label.normal.textColor : Color.gray;
            EditorGUI.LabelField(rect,gameObject.name,style);
        }
    }
}
