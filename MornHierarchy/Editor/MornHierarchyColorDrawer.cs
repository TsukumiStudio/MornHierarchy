using UnityEditor;
using UnityEngine;
namespace MornHierarchy {
    [CustomPropertyDrawer(typeof(MornHierarchyColor))]
    public class MornHierarchyColorDrawer : PropertyDrawer {
        private const float ButtonSize = 18f;
        private const float Spacing = 2f;
        public override float GetPropertyHeight(SerializedProperty property,GUIContent label) {
            return Mathf.Max(EditorGUIUtility.singleLineHeight,ButtonSize);
        }
        public override void OnGUI(Rect position,SerializedProperty property,GUIContent label) {
            var labelRect = new Rect(position.x,position.y,EditorGUIUtility.labelWidth,EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect,label);
            var gridX = position.x + EditorGUIUtility.labelWidth;
            var gridY = position.y + (position.height - ButtonSize) * 0.5f;
            DrawSwatch(new Rect(gridX,gridY,ButtonSize,ButtonSize),MornHierarchyColor.None,property);
            var displayIdx = 1;
            var values = System.Enum.GetValues(typeof(MornHierarchyColor));
            foreach(MornHierarchyColor c in values) {
                if(c == MornHierarchyColor.None) continue;
                var rect = new Rect(gridX + displayIdx * (ButtonSize + Spacing),gridY,ButtonSize,ButtonSize);
                DrawSwatch(rect,c,property);
                displayIdx++;
            }
        }
        private static void DrawSwatch(Rect rect,MornHierarchyColor c,SerializedProperty property) {
            if(c == MornHierarchyColor.None) DrawNoneSwatch(rect);
            else EditorGUI.DrawRect(rect,EditorHierarchyOnGUI.ToColor(c));
            if((MornHierarchyColor)property.enumValueIndex == c) DrawOutline(rect,Color.white,2);
            if(Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)) {
                property.enumValueIndex = (int)c;
                Event.current.Use();
                GUI.changed = true;
            }
        }
        private static void DrawNoneSwatch(Rect rect) {
            EditorGUI.DrawRect(rect,new Color(0.2f,0.2f,0.2f));
            var red = new Color(1f,0.3f,0.3f);
            var n = Mathf.RoundToInt(rect.width);
            for(var t = 0;t < n;t++) {
                EditorGUI.DrawRect(new Rect(rect.x + t,rect.yMax - t - 1,2,1),red);
                EditorGUI.DrawRect(new Rect(rect.x + t,rect.y + t,2,1),red);
            }
        }
        private static void DrawOutline(Rect rect,Color color,float thickness) {
            EditorGUI.DrawRect(new Rect(rect.x,rect.y,rect.width,thickness),color);
            EditorGUI.DrawRect(new Rect(rect.x,rect.yMax - thickness,rect.width,thickness),color);
            EditorGUI.DrawRect(new Rect(rect.x,rect.y,thickness,rect.height),color);
            EditorGUI.DrawRect(new Rect(rect.xMax - thickness,rect.y,thickness,rect.height),color);
        }
    }
}
