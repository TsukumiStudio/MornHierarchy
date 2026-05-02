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
    }
    public class MornHierarchy : MonoBehaviour {
#if UNITY_EDITOR
        public bool EnableColor = true;
        public MornHierarchyColor BackColor;
        public bool ApplyChildren;
        public Texture2D Icon;
        public bool IsLine;
        private void OnValidate() {
            UnityEditor.EditorApplication.RepaintHierarchyWindow();
        }
#endif
    }
}