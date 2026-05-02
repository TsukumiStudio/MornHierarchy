using UnityEditor;
using UnityEngine;
namespace MornHierarchy {
    [CustomPropertyDrawer(typeof(MornHierarchyColor))]
    public class MornHierarchyColorDrawer : PropertyDrawer {
        private const int Cols = 8;
        private const float ButtonSize = 22f;
        private const float Spacing = 2f;
        public override float GetPropertyHeight(SerializedProperty property,GUIContent label) {
            var count = System.Enum.GetValues(typeof(MornHierarchyColor)).Length;
            var rows = Mathf.CeilToInt(count / (float)Cols);
            return EditorGUIUtility.singleLineHeight + rows * (ButtonSize + Spacing) + 2;
        }
        public override void OnGUI(Rect position,SerializedProperty property,GUIContent label) {
            var labelRect = new Rect(position.x,position.y,position.width,EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect,label);
            var values = System.Enum.GetValues(typeof(MornHierarchyColor));
            var gridX = position.x + EditorGUIUtility.labelWidth;
            var gridY = position.y + EditorGUIUtility.singleLineHeight + 2;
            for(var i = 0;i < values.Length;i++) {
                var col = i % Cols;
                var row = i / Cols;
                var rect = new Rect(gridX + col * (ButtonSize + Spacing),gridY + row * (ButtonSize + Spacing),ButtonSize,ButtonSize);
                var c = (MornHierarchyColor)values.GetValue(i);
                EditorGUI.DrawRect(rect,EditorHierarchyOnGUI.ToColor(c));
                if(property.enumValueIndex == i) DrawOutline(rect,Color.white,2);
                if(Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)) {
                    property.enumValueIndex = i;
                    Event.current.Use();
                    GUI.changed = true;
                }
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
