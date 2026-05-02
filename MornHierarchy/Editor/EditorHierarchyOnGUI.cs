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
            ResolveColor(gameObject,out var effectiveColor,out var fade);
            EraseNativeText(instanceID,selectionRect);
            if(effectiveColor != MornHierarchyColor.None) {
                var fullRect = selectionRect;
                fullRect.xMin = 0;
                fullRect.xMax = Mathf.Max(selectionRect.xMax,EditorGUIUtility.currentViewWidth);
                DrawGradient(fullRect,ToColor(effectiveColor),fade);
            }
            DrawLabel(selectionRect,gameObject,isLine);
            DrawIcon(selectionRect,gameObject);
        }
        private static void ResolveColor(GameObject gameObject,out MornHierarchyColor color,out float fade) {
            color = MornHierarchyColor.None;
            fade = 1f;
            if(gameObject.TryGetComponent<MornHierarchy>(out var own) && own.BackColor != MornHierarchyColor.None) {
                color = own.BackColor;
                return;
            }
            var ancestors = gameObject.GetComponentsInParent<MornHierarchy>(true);
            if(ancestors == null) return;
            foreach(var hi in ancestors) {
                if(hi.transform == gameObject.transform) continue;
                if(hi.BackColor == MornHierarchyColor.None) continue;
                if(hi.ApplyChildren == false) return;
                color = hi.BackColor;
                fade = GetKRecursion(hi.transform,gameObject.transform);
                return;
            }
        }
        private static void DrawGradient(Rect rect,Color baseColor,float fade) {
            const float solidWidth = 32f;
            var solid = baseColor;
            solid.a = fade;
            EditorGUI.DrawRect(new Rect(rect.x,rect.y,solidWidth,rect.height),solid);
            var gradStart = rect.x + solidWidth;
            var gradWidth = rect.xMax - gradStart;
            if(gradWidth <= 0) return;
            var prevColor = GUI.color;
            GUI.color = new Color(baseColor.r,baseColor.g,baseColor.b,fade);
            GUI.DrawTexture(new Rect(gradStart,rect.y,gradWidth,rect.height),GetGradientTex(),ScaleMode.StretchToFill,true);
            GUI.color = prevColor;
        }
        private static Texture2D s_gradientTex;
        private static Texture2D GetGradientTex() {
            if(s_gradientTex != null) return s_gradientTex;
            const int w = 128;
            s_gradientTex = new Texture2D(w,1,TextureFormat.RGBA32,false) {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                hideFlags = HideFlags.HideAndDontSave,
            };
            var pixels = new Color[w];
            for(var i = 0;i < w;i++) pixels[i] = new Color(1f,1f,1f,i / (float)(w - 1));
            s_gradientTex.SetPixels(pixels);
            s_gradientTex.Apply();
            return s_gradientTex;
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
        public static Color ToColor(MornHierarchyColor c) {
            switch(c) {
                case MornHierarchyColor.Red:     return new Color(1.00f,0.15f,0.15f);
                case MornHierarchyColor.Orange:  return new Color(1.00f,0.50f,0.05f);
                case MornHierarchyColor.Yellow:  return new Color(1.00f,0.90f,0.05f);
                case MornHierarchyColor.Lime:    return new Color(0.60f,1.00f,0.15f);
                case MornHierarchyColor.Green:   return new Color(0.10f,0.85f,0.25f);
                case MornHierarchyColor.Mint:    return new Color(0.20f,0.95f,0.70f);
                case MornHierarchyColor.Cyan:    return new Color(0.10f,0.85f,1.00f);
                case MornHierarchyColor.Blue:    return new Color(0.15f,0.45f,1.00f);
                case MornHierarchyColor.Indigo:  return new Color(0.35f,0.30f,0.95f);
                case MornHierarchyColor.Purple:  return new Color(0.70f,0.25f,0.95f);
                case MornHierarchyColor.Magenta: return new Color(1.00f,0.20f,1.00f);
                case MornHierarchyColor.Pink:    return new Color(1.00f,0.45f,0.75f);
                case MornHierarchyColor.Brown:   return new Color(0.75f,0.40f,0.15f);
                case MornHierarchyColor.Gray:    return new Color(0.70f,0.70f,0.70f);
                case MornHierarchyColor.White:   return new Color(1.00f,1.00f,1.00f);
                case MornHierarchyColor.Black:   return new Color(0.05f,0.05f,0.05f);
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
            style.normal.textColor = gameObject.activeInHierarchy ? Color.white : Color.gray;
            EditorGUI.LabelField(rect,gameObject.name,style);
        }
    }
}
