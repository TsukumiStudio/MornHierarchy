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
        private static void HierarchyWindowItemOnGUI(int instanceID,Rect selectionRect) {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if(gameObject == null) return;
            if(gameObject.TryGetComponent<MornHierarchy>(out var hi) && hi.IsLine) {
                DrawLine(instanceID,selectionRect,gameObject);
                return;
            }
            EraseNativeText(instanceID,selectionRect);
            DrawColor(selectionRect,gameObject);
            DrawLabel(selectionRect,gameObject);
            DrawIcon(selectionRect,gameObject);
        }
        private static void DrawLine(int instanceID,Rect selectionRect,GameObject gameObject) {
            var fullRect = selectionRect;
            fullRect.xMin = 0;
            fullRect.xMax = Mathf.Max(selectionRect.xMax,EditorGUIUtility.currentViewWidth);
            EditorGUI.DrawRect(fullRect,GetRowBackgroundColor(instanceID,fullRect));
            var lineRect = fullRect;
            lineRect.xMin = 32;
            var upper = lineRect;
            upper.yMax = upper.yMin + 2;
            EditorGUI.DrawRect(upper,Color.black);
            var lower = lineRect;
            lower.yMin = lower.yMax - 2;
            EditorGUI.DrawRect(lower,Color.black);
            var style = new GUIStyle(EditorStyles.label);
            style.alignment = TextAnchor.MiddleCenter;
            EditorGUI.LabelField(lineRect,gameObject.name,style);
        }
        private static void DrawIcon(Rect selectionRect,GameObject gameObject) {
            if(gameObject.TryGetComponent<MornHierarchy>(out var hi) == false) return;
            if(hi.Icon == null) return;
            var rect = new Rect(IconColumnX,selectionRect.y,IconColumnWidth,selectionRect.height);
            GUI.DrawTexture(rect,hi.Icon,ScaleMode.ScaleToFit);
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
            if(gameObject.TryGetComponent<MornHierarchy>(out var ownColor)) {
                DrawTransparentRect(fullRect,ownColor.BackColor);
                return;
            }
            var mornHiArray = gameObject.GetComponentsInParent<MornHierarchy>(true);
            if(mornHiArray == null) return;
            foreach(var hi in mornHiArray) {
                if(hi.transform == gameObject.transform) continue;
                if(hi.ApplyChildren == false) return;
                var kFront = GetKRecursion(hi.transform,gameObject.transform);
                DrawTransparentRect(fullRect,hi.BackColor * kFront);
                return;
            }
        }
        private static void DrawTransparentRect(Rect rect,Color color) {
            color.a = 0.3f;
            EditorGUI.DrawRect(rect,color);
        }
        private static float GetKRecursion(Transform aim,Transform own) {
            if(aim == own) return 1f;
            const int offset = 2;
            var pare = own.parent;
            return Mathf.InverseLerp(pare.childCount + offset,-1,own.GetSiblingIndex()) * GetKRecursion(aim,pare);
        }
        private static void DrawLabel(Rect selectionRect,GameObject gameObject) {
            selectionRect.xMin += 18;
            var style = new GUIStyle(EditorStyles.label);
            style.normal.textColor = gameObject.activeInHierarchy ? EditorStyles.label.normal.textColor : Color.gray;
            EditorGUI.LabelField(selectionRect,gameObject.name,style);
        }
    }
}
