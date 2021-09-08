using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace Orcas.Gragh.Editor
{
    public static class VisualElementExtensions
    {
        private static readonly string PackagePath =  "Packages/orcas.graph/Editor/";
        public static void LoadAndAddStyleSheet(this VisualElement visualElement, string sheetPath)
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PackagePath + sheetPath + ".uss");
            if (styleSheet == null)
                Debug.LogWarning(string.Format("Style sheet not found for path \"{0}\"", sheetPath));
            else
                visualElement.styleSheets.Add(styleSheet);
        }
    }
}