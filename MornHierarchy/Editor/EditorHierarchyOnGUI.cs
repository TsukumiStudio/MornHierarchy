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
            var hasDrawn = false;
            if(gameObject.TryGetComponent<MornHierarchy>(out var ownColor)) {
                DrawTransparentRect(fullRect,ownColor.BackColor);
                hasDrawn = true;
            }
            var mornHiArray = gameObject.GetComponentsInParent<MornHierarchy>(true);
            if(mornHiArray == null) return;
            var target = gameObject.transform;
            var depth = 0;
            for(var hiIndex = 0;hiIndex < mornHiArray.Length;) {
                var hi = mornHiArray[hiIndex];
                if(target == hi.transform) {
                    hiIndex++;
                    continue;
                }
                if(hi.ApplyChildren == false) {
                    hiIndex++;
                    target = target.parent;
                    depth++;
                    hasDrawn = true;
                    continue;
                }

                //Side
                var kBack = GetKRecursion(hi.transform,target.parent);
                var rectA = selectionRect;
                rectA.xMax = rectA.xMin - 14 * (depth);
                rectA.xMin = rectA.xMax - 14;
                DrawTransparentRect(rectA,hi.BackColor * kBack);

                //Main
                if(hasDrawn == false) {
                    var kFront = GetKRecursion(hi.transform,gameObject.transform);
                    DrawTransparentRect(fullRect,hi.BackColor * kFront);
                    hasDrawn = true;
                }
                target = target.parent;
                depth++;
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
