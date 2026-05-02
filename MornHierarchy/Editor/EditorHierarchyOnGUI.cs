using UnityEditor;
using UnityEngine;
namespace MornHierarchy {
    public static class EditorHierarchyOnGUI {
        [InitializeOnLoadMethod]
        private static void AddHierarchyItemOnGUI() {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }
        private static void HierarchyWindowItemOnGUI(int instanceID,Rect selectionRect) {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if(gameObject == null) return;
            EraseNativeText(instanceID,selectionRect);
            DrawColor(selectionRect,gameObject);
            DrawLabel(selectionRect,gameObject);
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
