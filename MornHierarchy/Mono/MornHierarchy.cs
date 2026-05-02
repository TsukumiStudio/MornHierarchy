using UnityEngine;
namespace MornHierarchy {
    public class MornHierarchy : MonoBehaviour {
        [ColorUsage(false)]public Color BackColor;
        public bool ApplyChildren;
        public Texture2D Icon;
        public bool IsLine;
        #if UNITY_EDITOR
        private void OnValidate() {
            UnityEditor.EditorApplication.RepaintHierarchyWindow();
        }
        #endif
    }
}