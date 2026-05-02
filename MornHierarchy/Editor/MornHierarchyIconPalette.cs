using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace MornHierarchy {
    public static class MornHierarchyIconPalette {
        private static Texture2D[] s_cache;
        public static Texture2D[] Icons {
            get {
                if(s_cache != null) return s_cache;
                var list = new List<Texture2D>();
                foreach(var guid in AssetDatabase.FindAssets("Icon_ t:Texture2D")) {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var name = Path.GetFileNameWithoutExtension(path);
                    if(name == null || name.StartsWith("Icon_") == false) continue;
                    var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    if(tex != null) list.Add(tex);
                }
                list.Sort((a,b) => string.Compare(a.name,b.name,System.StringComparison.Ordinal));
                s_cache = list.ToArray();
                return s_cache;
            }
        }
        public static void Invalidate() => s_cache = null;
    }
}
