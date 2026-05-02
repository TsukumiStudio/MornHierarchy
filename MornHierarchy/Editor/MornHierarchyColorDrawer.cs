using UnityEditor;
using UnityEngine;
namespace MornHierarchy {
    [CustomPropertyDrawer(typeof(MornHierarchyColor))]
    public class MornHierarchyColorDrawer : PropertyDrawer {
        private const float ButtonSize = 18f;
        private const float Spacing = 2f;
        private const int Total = 17;
        private static int CalcCols() {
            var avail = EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 30f;
            return Mathf.Max(1,Mathf.FloorToInt((avail + Spacing) / (ButtonSize + Spacing)));
        }
        public override float GetPropertyHeight(SerializedProperty property,GUIContent label) {
            var rows = Mathf.CeilToInt(Total / (float)CalcCols());
            return rows * ButtonSize + (rows - 1) * Spacing;
        }
        public override void OnGUI(Rect position,SerializedProperty property,GUIContent label) {
            var labelRect = new Rect(position.x,position.y,EditorGUIUtility.labelWidth,EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect,label);
            var gridX = position.x + EditorGUIUtility.labelWidth;
            var cols = CalcCols();
            var displayIdx = 0;
            DrawAt(displayIdx++,gridX,position.y,cols,MornHierarchyColor.None,property);
            foreach(MornHierarchyColor c in System.Enum.GetValues(typeof(MornHierarchyColor))) {
                if(c == MornHierarchyColor.None) continue;
                DrawAt(displayIdx++,gridX,position.y,cols,c,property);
            }
        }
        private static void DrawAt(int index,float gridX,float gridY,int cols,MornHierarchyColor c,SerializedProperty property) {
            var col = index % cols;
            var row = index / cols;
            var rect = new Rect(gridX + col * (ButtonSize + Spacing),gridY + row * (ButtonSize + Spacing),ButtonSize,ButtonSize);
            DrawSwatch(rect,c,property);
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
