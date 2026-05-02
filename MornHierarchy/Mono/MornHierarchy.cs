using UnityEngine;
namespace MornHierarchy {
    public enum MornHierarchyColor {
        Red,
        Orange,
        Yellow,
        Lime,
        Green,
        Mint,
        Cyan,
        Blue,
        Indigo,
        Purple,
        Magenta,
        Pink,
        Brown,
        Gray,
        White,
        Black,
        None,
    }
    public class MornHierarchy : MonoBehaviour {
#if UNITY_EDITOR
        public bool IsLine;
        public Texture2D Icon;
        public MornHierarchyColor BackColor;
        public bool ColorAsText;
        public bool ApplyChildren;
        private void OnValidate() {
            UnityEditor.EditorApplication.RepaintHierarchyWindow();
        }
#endif
    }
}