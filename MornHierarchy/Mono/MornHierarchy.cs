using UnityEngine;
namespace MornHierarchy {
    public class MornHierarchy : MonoBehaviour {
#if UNITY_EDITOR
        public bool EnableColor = true;
        [ColorUsage(false)]public Color BackColor;
        public bool ApplyChildren;
        public Texture2D Icon;
        public bool IsLine;
        private void OnValidate() {
            UnityEditor.EditorApplication.RepaintHierarchyWindow();
        }
#endif
    }
}