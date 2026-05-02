using UnityEditor;
using UnityEngine;
namespace MornHierarchy {
    [CustomEditor(typeof(MornHierarchy))]
    [CanEditMultipleObjects]
    public class MornHierarchyEditor : Editor {
        private const float ButtonSize = 24f;
        private const float Spacing = 2f;
        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MornHierarchy.IsLine)));
            DrawIconGrid(serializedObject.FindProperty(nameof(MornHierarchy.Icon)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MornHierarchy.BackColor)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MornHierarchy.ApplyChildren)));
            serializedObject.ApplyModifiedProperties();
        }
        private static void DrawIconGrid(SerializedProperty property) {
            var icons = MornHierarchyIconPalette.Icons;
            var total = icons.Length + 1;
            var rect = EditorGUILayout.GetControlRect(true,ButtonSize);
            var labelRect = new Rect(rect.x,rect.y + (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f,EditorGUIUtility.labelWidth,EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect,property.displayName);
            var gridX = rect.x + EditorGUIUtility.labelWidth;
            for(var i = 0;i < total;i++) {
                var br = new Rect(gridX + i * (ButtonSize + Spacing),rect.y,ButtonSize,ButtonSize);
                Texture2D tex = i == 0 ? null : icons[i - 1];
                if(tex == null) DrawNoneSwatch(br);
                else GUI.DrawTexture(br,tex,ScaleMode.ScaleToFit);
                if(property.objectReferenceValue == tex) DrawOutline(br,Color.white,2);
                if(Event.current.type == EventType.MouseDown && br.Contains(Event.current.mousePosition)) {
                    property.objectReferenceValue = tex;
                    Event.current.Use();
                    GUI.changed = true;
                }
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
